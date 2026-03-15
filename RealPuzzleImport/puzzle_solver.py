from heapq import heapify, heappop

import cv2
import numpy as np

from piece import NUM_PIECE_POINTS, Piece
from puzzle import Puzzle
from transformation import get_transformation, Transformation


MAX_HULL_CANDIDATE_POINTS = 32
SEGMENT_COMPARISON_POINTS = 64
ANCHOR_SEGMENT_EXTENSION = 1/12
ANCHOR_DIFF_CANDIDATES = np.linspace(-1/16, 1/16, 4)


class PieceConnectionCandidate:
    socket_out_piece: Piece
    out_hull_index: int
    socket_in_piece: Piece
    in_cave_index: int
    transformation: Transformation
    score: float
    spline_score: float
    color_score: float

    def __init__(
        self,
        socket_out_piece: Piece,
        out_hull_index: int,
        socket_in_piece: Piece,
        in_cave_index: int,
        transformation: Transformation,
        score: float,
        spline_score: float,
        color_score: float
    ):
        self.socket_out_piece = socket_out_piece
        self.out_hull_index = out_hull_index
        self.socket_in_piece = socket_in_piece
        self.in_cave_index = in_cave_index
        self.transformation = transformation
        self.score = score
        self.spline_score = spline_score
        self.color_score = color_score

    def __lt__(self, other):
        return self.score < other.score

    def __repr__(self):
        return f"PieceConnectionCandidate: ({self.transformation}, {self.score})"


min_color_score = 10000
max_color_score = 0


def _color_gradient(piece, piece_param):
    outline_indices = (piece_param % 1 * NUM_PIECE_POINTS).astype(int)
    piece_colors = piece.outline_colors[outline_indices]

    return piece_colors


def _get_color_score(piece, piece_param, other_piece, other_piece_param):
    from_colors = _color_gradient(piece, piece_param)
    to_colors = _color_gradient(other_piece, other_piece_param)

    color_diffs = np.linalg.norm(from_colors - to_colors, axis=1)
    color_score = color_diffs.mean()

    global min_color_score
    global max_color_score

    min_color_score = min(min_color_score, color_score)
    max_color_score = max(max_color_score, color_score)

    return color_score


def _get_spline_score(from_points, to_points):
    spline_diffs = from_points - to_points
    spline_score = np.mean(np.linalg.norm(spline_diffs, axis=0))

    return spline_score


def _get_score(piece, piece_param, other_piece, other_piece_param, transformed_from_points, to_points):
    spline_score = _get_spline_score(transformed_from_points, to_points)
    color_score = _get_color_score(piece, piece_param, other_piece, other_piece_param)

    score = spline_score, color_score

    return score


def _best_fit_inside_allow_t_joint(piece, other_piece):
    arc_length = piece.spline.arc_length
    other_arc_length = other_piece.spline.arc_length
    arc_length_ratio = arc_length / other_arc_length

    min_distance_diff_sum = float("inf")
    best_transformation = None
    best_hull_index = None
    best_cave_index = None

    for hull_index, hull_section in enumerate(piece.piece_hull_sections):

        start_idx = hull_section.start_idx
        end_idx = hull_section.end_idx + 1
        end_idx += NUM_PIECE_POINTS * (start_idx > end_idx)

        for cave_index, cave in enumerate(other_piece.piece_caves):
            step = max(1, (end_idx - hull_section.start_idx) // MAX_HULL_CANDIDATE_POINTS)

            for point_idx in range(hull_section.start_idx, end_idx, step):
                point_idx %= NUM_PIECE_POINTS
                from_anchor_start_param = point_idx / NUM_PIECE_POINTS
                to_anchor_start_param = cave.deepest_idx / NUM_PIECE_POINTS

                for anchor_diff in ANCHOR_DIFF_CANDIDATES:
                    from_anchor_start_point = piece.spline(from_anchor_start_param)
                    to_anchor_start_point = other_piece.spline(to_anchor_start_param)

                    from_anchor_end_param = from_anchor_start_param + anchor_diff
                    to_anchor_end_param = to_anchor_start_param - anchor_diff * arc_length_ratio
                    from_anchor_end_point = piece.spline(from_anchor_end_param)
                    to_anchor_end_point = other_piece.spline(to_anchor_end_param)

                    from_min_param = from_anchor_start_param - ANCHOR_SEGMENT_EXTENSION
                    from_max_param = from_anchor_start_param + ANCHOR_SEGMENT_EXTENSION
                    from_param = np.linspace(from_min_param, from_max_param, SEGMENT_COMPARISON_POINTS)

                    to_min_param = to_anchor_start_param + ANCHOR_SEGMENT_EXTENSION * arc_length_ratio
                    to_max_param = to_anchor_start_param - ANCHOR_SEGMENT_EXTENSION * arc_length_ratio
                    to_param = np.linspace(to_min_param, to_max_param, SEGMENT_COMPARISON_POINTS)

                    transformation = get_transformation(
                        from_anchor_start_point,
                        from_anchor_end_point,
                        to_anchor_start_point,
                        to_anchor_end_point
                    )

                    from_points = piece.spline(from_param)
                    transformed_from_points = transformation(from_points)

                    to_points = other_piece.spline(to_param)

                    spline_score, color_score = _get_score(
                        piece,
                        from_param,
                        other_piece,
                        to_param,
                        transformed_from_points,
                        to_points
                    )
                    score = spline_score

                    if score < min_distance_diff_sum:
                        min_distance_diff_sum = score
                        best_transformation = transformation
                        best_hull_index = hull_index
                        best_cave_index = cave_index
                        min_spline_score = spline_score
                        min_color_score = color_score

    assert best_transformation is not None

    piece_connection = PieceConnectionCandidate(
        piece,
        best_hull_index,
        other_piece,
        best_cave_index,
        best_transformation,
        min_distance_diff_sum,
        min_spline_score,
        min_color_score
    )

    return piece_connection


def _connections_heap(puzzle):
    heap = []

    for i, piece0 in enumerate(puzzle):
        for j, piece1 in enumerate(puzzle):
            if piece0 != piece1:
                connection = _best_fit_inside_allow_t_joint(piece0, piece1)
                heap.append(connection)

    heapify(heap)

    return heap


def _single_piece_chunks(puzzle):
    chunks = []

    for i, piece in enumerate(puzzle):
        piece.attach_state(i)
        chunks.append([piece])

    return chunks


def solve(puzzle: Puzzle):
    connections_heap = _connections_heap(puzzle)
    chunks = _single_piece_chunks(puzzle)
    piece_count = puzzle.piece_count()
    connect = 0

    while len(connections_heap):
        cand_conn = heappop(connections_heap)

        out_piece = cand_conn.socket_out_piece
        out_hull_index = cand_conn.out_hull_index
        in_piece = cand_conn.socket_in_piece
        in_cave_index = cand_conn.in_cave_index

        out_chunk_idx = cand_conn.socket_out_piece.chunk_idx
        in_chunk_idx = in_piece.chunk_idx

        out_chunk = chunks[out_chunk_idx]
        in_chunk = chunks[in_chunk_idx]

        if (out_chunk_idx == in_chunk_idx
            or out_piece.used_piece_hull_sections[out_hull_index]
            or in_piece.used_caves[in_cave_index]):
            continue

        connect += 1
        print("Candidate", cand_conn.transformation.from_point, cand_conn.transformation.to_point)
        print("Scoring", cand_conn.score, cand_conn.color_score, cand_conn.spline_score)

        out_piece.used_piece_hull_sections[out_hull_index] = True
        in_piece.used_caves[in_cave_index] = True

        out_chunk_transformation = in_piece.transformation(
            cand_conn.transformation(
                out_piece.transformation.invert()
            )
        )

        for piece in out_chunk:
            piece.chunk_idx = in_chunk_idx
            piece.transformation = out_chunk_transformation(piece.transformation)
            in_chunk.append(piece)

        if len(in_chunk) == piece_count:
            print("Solved")
            return

    raise RuntimeError("Failed to Solve Puzzle")
