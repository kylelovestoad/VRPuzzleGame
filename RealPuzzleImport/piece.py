# Credit: https://agniva.me/scipy/2016/10/25/contour-smoothing.html
from dataclasses import dataclass

import cv2
import numpy as np
from numpy import ndarray
from scipy.interpolate import make_splprep, BSpline

from transformation import Transformation, IDENTITY_TRANSFORMATION

NUM_PIECE_POINTS = 1024
MIN_CAVE_DEPTH = 10
COLOR_BLOB_RADIUS = 4


class PieceSpline:
    spline: BSpline
    smooth_params: ndarray
    noisy_params: ndarray
    arc_length: float

    def __init__(self, spline, smooth_params, noisy_params, arc_length):
        self.spline = spline
        self.smooth_params = smooth_params
        self.noisy_params = noisy_params
        self.arc_length = arc_length

    def __call__(self, params):
        params = np.interp(params % 1, self.smooth_params, self.noisy_params)
        xy = self.spline(params)

        return xy

    def to_contour(self):
        x, y = self(self.noisy_params)
        res_array = [[[int(xi), int(yi)]] for xi, yi in zip(x, y)]
        contour = np.asarray(res_array, dtype=np.int32)

        return contour


@dataclass
class HullSection:
    start_idx: int
    end_idx: int


@dataclass
class CaveSection:
    start_idx: int
    end_idx: int
    deepest_idx: int


class Piece:
    spline: PieceSpline
    outline_colors: ndarray
    piece_hull_sections: list[HullSection]
    piece_caves: list[CaveSection]

    # only added later when computing solution
    chunk_idx: int
    transformation: Transformation
    used_piece_hull_sections: list[bool]
    used_caves: list[bool]

    def __init__(
        self,
        spline: PieceSpline,
        outline_colors: ndarray,
        outline_points,
        piece_hull_sections: list[HullSection],
        piece_caves: list[CaveSection]
    ):
        self.spline = spline
        self.outline_colors = outline_colors
        self.outline_points = outline_points
        self.piece_hull_sections = piece_hull_sections
        self.piece_caves = piece_caves

    def get_placement(self, param):
        return self.transform_segment(param, self.transformation)

    def transform_segment(self, segment_param, transformation):
        points = self.spline(segment_param)
        transformed = transformation(points)

        return transformed

    def to_contour(self):
        return self.spline.to_contour()

    def attach_state(self, chunk_idx):
        self.chunk_idx = chunk_idx
        self.transformation = IDENTITY_TRANSFORMATION
        self.used_piece_hull_sections = [False] * len(self.piece_hull_sections)
        self.used_caves = [False] * len(self.piece_caves)


def _get_cumulative_smooth_distances(x, y):
    diffs_x = np.diff(x)
    diffs_y = np.diff(y)
    smooth_distances = np.hypot(diffs_x, diffs_y)

    cumulative_smooth_distances = np.concatenate([[0], np.cumsum(smooth_distances)])

    return cumulative_smooth_distances


def _smooth_parameter_mapping(spline, noisy_params):
    x, y = spline(noisy_params)

    cumulative_smooth_distances = _get_cumulative_smooth_distances(x, y)
    smooth_arc_length = cumulative_smooth_distances[-1]

    smoothed_params = cumulative_smooth_distances / smooth_arc_length

    return smoothed_params, smooth_arc_length


def _get_piece_spline(contour):
    x = contour[:, 0, 0].tolist()
    x.append(x[0])
    y = contour[:, 0, 1].tolist()
    y.append(y[0])
    contour_points = len(x)

    spline, _ = make_splprep([x, y], s=contour_points)

    initial_param = np.linspace(0, 1, NUM_PIECE_POINTS + 1)
    smooth_param, arc_length = _smooth_parameter_mapping(spline, initial_param)

    piece_spline = PieceSpline(spline, smooth_param, initial_param, arc_length)

    return piece_spline


def _get_piece_caves(convex_defects):
    piece_caves = []

    for cave in convex_defects:
        start_idx, end_idx, deepest_idx, scaled_depth = cave[0]
        depth = scaled_depth / 256.0

        if depth > MIN_CAVE_DEPTH:
            cave_section = CaveSection(start_idx, end_idx, deepest_idx)
            piece_caves.append(cave_section)

    return piece_caves


def _get_piece_hull_sections(piece_caves):
    cave_count = len(piece_caves)
    piece_hull_sections = []

    for i in range(cave_count):
        prev_cave = piece_caves[(i or cave_count) - 1]
        curr_cave = piece_caves[i]

        hull_section = HullSection(prev_cave.end_idx, curr_cave.start_idx)
        piece_hull_sections.append(hull_section)

    return piece_hull_sections


def _get_piece_sections(piece_spline):
    contour = piece_spline.to_contour()

    convex_hull = cv2.convexHull(contour, returnPoints=False)
    convex_defects = cv2.convexityDefects(contour, convex_hull)

    piece_caves = _get_piece_caves(convex_defects)
    piece_hull_sections = _get_piece_hull_sections(piece_caves)

    return piece_hull_sections, piece_caves


def _get_piece_outline_colors(image, piece_mask, piece_spline):
    contour = piece_spline.to_contour()

    rows, cols = image.shape[:2]
    outline_colors = np.zeros((len(contour), 3), dtype=np.uint8)
    outline_points = []

    for i, point in enumerate(contour):
        x, y = point[0]

        x0 = max(0, x - COLOR_BLOB_RADIUS)
        x1 = min(cols, x + COLOR_BLOB_RADIUS + 1)
        y0 = max(0, y - COLOR_BLOB_RADIUS)
        y1 = min(rows, y + COLOR_BLOB_RADIUS + 1)

        color_blob = image[y0:y1, x0:x1][piece_mask[y0:y1, x0:x1] > 0]

        if len(color_blob) > 0:
            median_color = np.median(color_blob, axis=0)
        else:
            median_color = image[y, x]

        outline_colors[i] = median_color
        outline_points.append((x, y))

    return outline_colors, outline_points


def piece_from_contour(image, piece_mask, contour):
    print(len(contour))

    piece_spline = _get_piece_spline(contour)
    piece_hull_sections, piece_caves = _get_piece_sections(piece_spline)
    outline_colors, outline_points = _get_piece_outline_colors(image, piece_mask, piece_spline)

    return Piece(piece_spline, outline_colors, outline_points, piece_hull_sections, piece_caves)
