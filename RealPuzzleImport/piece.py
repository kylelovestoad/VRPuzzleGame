# Credit: https://agniva.me/scipy/2016/10/25/contour-smoothing.html
from dataclasses import dataclass

import cv2
import numpy as np
from scipy.interpolate import make_splprep


NUM_PIECE_POINTS = 1024
MIN_CAVE_DEPTH = 10
SEGMENT_COMPARISON_POINTS = 256
ANCHOR_SEGMENT_EXTENSION = 1/12
ANCHOR_DIFF_CANDIDATES = np.linspace(-1/16, 1/16, 4)


class Piece:
    def __init__(self, spline, piece_hull_sections, piece_caves):
        self.spline = spline
        self.piece_hull_sections = piece_hull_sections
        self.piece_caves = piece_caves

    def best_fit_inside(self, other_piece):
        arc_length = self.spline.arc_length
        other_arc_length = other_piece.spline.arc_length
        arc_length_ratio = arc_length / other_arc_length

        min_distance_diff_sum = float("inf")
        best_transformation = None

        for hull_section in self.piece_hull_sections:

            for cave in other_piece.piece_caves:
                start_idx = hull_section.start_idx
                end_idx = hull_section.end_idx + 1
                end_idx += NUM_PIECE_POINTS * (start_idx > end_idx)

                for point_idx in range(hull_section.start_idx, end_idx):
                    point_idx %= NUM_PIECE_POINTS
                    from_anchor_start_param = point_idx / NUM_PIECE_POINTS
                    to_anchor_start_param = cave.deepest_idx / NUM_PIECE_POINTS

                    for anchor_diff in ANCHOR_DIFF_CANDIDATES:
                        from_anchor_start_point = self.spline(from_anchor_start_param)
                        to_anchor_start_point = other_piece.spline(to_anchor_start_param)

                        from_anchor_end_param = from_anchor_start_param + anchor_diff
                        to_anchor_end_param = to_anchor_start_param - anchor_diff * arc_length_ratio
                        from_anchor_end_point = self.spline(from_anchor_end_param)
                        to_anchor_end_point = other_piece.spline(to_anchor_end_param)

                        from_min_param = from_anchor_start_param - ANCHOR_SEGMENT_EXTENSION
                        from_max_param = from_anchor_start_param + ANCHOR_SEGMENT_EXTENSION
                        from_param = np.linspace(from_min_param, from_max_param, SEGMENT_COMPARISON_POINTS)

                        to_min_param = to_anchor_start_param + ANCHOR_SEGMENT_EXTENSION * arc_length_ratio
                        to_max_param = to_anchor_start_param - ANCHOR_SEGMENT_EXTENSION * arc_length_ratio
                        to_param = np.linspace(to_min_param, to_max_param, SEGMENT_COMPARISON_POINTS)

                        transformation = _get_transformation(
                            from_anchor_start_point,
                            from_anchor_end_point,
                            to_anchor_start_point,
                            to_anchor_end_point
                        )

                        from_points = self.transform_segment(
                            from_param,
                            transformation
                        )
                        to_points = other_piece.spline(to_param)

                        diff = from_points - to_points
                        curr_distance_diff_sum = np.sum(np.linalg.norm(diff, axis=0))

                        if curr_distance_diff_sum < min_distance_diff_sum:
                            min_distance_diff_sum = curr_distance_diff_sum
                            best_transformation = transformation

        assert best_transformation is not None
        print(best_transformation)
        print(min_distance_diff_sum)

        return PieceConnection(self, other_piece, best_transformation)


    def transform_segment(self, param, transformation):
        segment = self.spline(param)

        segment_complex = segment[0] + 1j * segment[1]
        from_offset = segment_complex - transformation.from_point
        transformed_complex = transformation.to_point + transformation.rotation * from_offset
        transformed = np.array([transformed_complex.real, transformed_complex.imag])

        return transformed

    def to_contour(self):
        return self.spline.to_contour()


@dataclass
class Transformation:
    from_point: complex
    to_point: complex
    rotation: complex


def _get_transformation(from_start, from_end, to_start, to_end):
    to_vec = to_end - to_start

    from_len = np.linalg.norm(from_end - from_start)
    to_len = np.linalg.norm(to_vec)

    to_end = to_start + (from_len / to_len) * to_vec

    from_start_complex = complex(*from_start)
    from_end_complex = complex(*from_end)
    to_start_complex = complex(*to_start)
    to_end_complex = complex(*to_end)

    rotation = (to_end_complex - to_start_complex) / (from_end_complex - from_start_complex)

    return Transformation(from_start_complex, to_start_complex, rotation)


@dataclass
class PieceConnection:
    socket_out_piece: Piece
    socket_in_piece: Piece
    transformation: Transformation


class PieceSpline:
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


def _smooth_parameter_mapping(spline, noisy_params):
    x, y = spline(noisy_params)

    diffs_x = np.diff(x)
    diffs_y = np.diff(y)
    smooth_distances = np.hypot(diffs_x, diffs_y)

    cumulative_smooth_distances = np.concatenate([[0], np.cumsum(smooth_distances)])
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


def piece_from_contour(contour):
    piece_spline = _get_piece_spline(contour)
    piece_hull_sections, piece_caves = _get_piece_sections(piece_spline)

    return Piece(piece_spline, piece_hull_sections, piece_caves)
