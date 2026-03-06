# Credit: https://agniva.me/scipy/2016/10/25/contour-smoothing.html
from dataclasses import dataclass

import cv2
import numpy as np
from scipy.interpolate import make_splprep


NUM_PIECE_POINTS = 1024
MIN_CAVE_DEPTH = 10


class Piece:
    def __init__(self, spline, piece_hull_sections, piece_caves):
        self.spline = spline
        self.piece_hull_sections = piece_hull_sections
        self.piece_caves = piece_caves

    def as_contour(self):
        return self.spline.contour


class PieceSpline:
    def __init__(self, spline, smooth_param, noisy_param, contour):
        self.spline = spline
        self.smoothed_params = smooth_param
        self.noisy_param = noisy_param
        self.contour = contour

    def __call__(self, *args):
        params = np.interp(*args, self.smoothed_params, self.noisy_param)

        return self.spline(params)


@dataclass
class HullSection:
    start_idx: int
    end_idx: int


@dataclass
class CaveSection:
    start_idx: int
    end_idx: int
    deepest_idx: int


def _smooth_parameter_mapping(spline):
    noisy_params = np.linspace(0, 1, NUM_PIECE_POINTS)
    x, y = spline(noisy_params)

    diffs_x = np.diff(x)
    diffs_y = np.diff(y)
    smooth_distances = np.hypot(diffs_x, diffs_y)

    cumulative_smooth_distances = np.concatenate([[0], np.cumsum(smooth_distances)])
    smooth_arc_length = cumulative_smooth_distances[-1]

    smoothed_params = cumulative_smooth_distances / smooth_arc_length

    return smoothed_params, noisy_params


def _spline_contour(spline):
    param = np.linspace(0, 1, NUM_PIECE_POINTS)
    x, y = spline(param)

    res_array = [[[int(xi), int(yi)]] for xi, yi in zip(x, y)]
    return np.asarray(res_array, dtype=np.int32)


def _get_piece_spline(contour):
    x = contour[:, 0, 0].tolist()
    x.append(x[0])
    y = contour[:, 0, 1].tolist()
    y.append(y[0])
    contour_points = len(x)

    spline, _ = make_splprep([x, y], s=contour_points)
    smoothed_params, fixed_params = _smooth_parameter_mapping(spline)
    contour = _spline_contour(spline)

    piece_spline = PieceSpline(spline, smoothed_params, fixed_params, contour)

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
        prev_cave = piece_caves[i or cave_count - 1]
        curr_cave = piece_caves[i]

        hull_section = HullSection(prev_cave.end_idx, curr_cave.start_idx)
        piece_hull_sections.append(hull_section)

    return piece_hull_sections


def _get_piece_sections(piece_spline):
    contour = piece_spline.contour

    convex_hull = cv2.convexHull(contour, returnPoints=False)
    convex_defects = cv2.convexityDefects(contour, convex_hull)

    piece_caves = _get_piece_caves(convex_defects)
    piece_hull_sections = _get_piece_hull_sections(piece_caves)

    return piece_hull_sections, piece_caves


def piece_from_contour(contour):
    piece_spline = _get_piece_spline(contour)
    piece_hull_sections, piece_caves = _get_piece_sections(piece_spline)

    return Piece(piece_spline, piece_hull_sections, piece_caves)
