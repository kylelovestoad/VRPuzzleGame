from heapq import heapify, heappop

import numpy as np

from piece import NUM_PIECE_POINTS, Piece
from transformation import get_transformation, IDENTITY_TRANSFORMATION, Transformation

SEGMENT_COMPARISON_POINTS = 128
ANCHOR_SEGMENT_EXTENSION = 1/12
ANCHOR_DIFF_CANDIDATES = np.linspace(-1/16, 1/16, 4)


class PieceConnectionCandidate:
    def __init__(
        self,
        socket_out_piece: Piece,
        socket_in_piece: Piece,
        transformation: Transformation,
        score: float
    ):
        self.socket_out_piece = socket_out_piece
        self.socket_in_piece = socket_in_piece
        self.transformation = transformation
        self.score = score

    def __lt__(self, other):
        return self.score < other.score

    def __repr__(self):
        return f"PieceConnectionCandidate: ({self.transformation}, {self.score})"


def _best_fit_inside(piece, other_piece):
    arc_length = piece.spline.arc_length
    other_arc_length = other_piece.spline.arc_length
    arc_length_ratio = arc_length / other_arc_length

    min_distance_diff_sum = float("inf")
    best_transformation = None
    max_hull_points = 0

    for hull_section in piece.piece_hull_sections:
        start_idx = hull_section.start_idx
        end_idx = hull_section.end_idx + 1
        end_idx += NUM_PIECE_POINTS * (start_idx > end_idx)

        max_hull_points = max(max_hull_points, end_idx - start_idx)

        for cave in other_piece.piece_caves:

            for point_idx in range(hull_section.start_idx, end_idx):
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

                    from_points = piece.transform_segment(
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
    print(f"Max hull: {max_hull_points}")

    piece_connection = PieceConnectionCandidate(piece, other_piece, best_transformation, min_distance_diff_sum)

    return piece_connection


def _connections_heap(puzzle):
    heap = []

    for piece0 in puzzle:
        for piece1 in puzzle:
            if piece0 != piece1:
                connection = _best_fit_inside(piece0, piece1)
                heap.append(connection)

    heapify(heap)

    return heap


def _attach_piece_state(piece, chunk_idx):
    piece.chunk_idx = chunk_idx
    piece.transformation = IDENTITY_TRANSFORMATION


def _single_piece_chunks(puzzle):
    chunks = []

    for i, piece in enumerate(puzzle):
        _attach_piece_state(piece, i)
        chunks.append([piece])

    return chunks



def solve(puzzle):
    connections_heap = _connections_heap(puzzle)
    chunks = _single_piece_chunks(puzzle)
    piece_count = puzzle.piece_count()

    while 1:
        cand_conn = heappop(connections_heap)

        out_chunk_idx = cand_conn.socket_out_piece.chunk_idx
        in_chunk_idx = cand_conn.socket_in_piece.chunk_idx

        if out_chunk_idx == in_chunk_idx:
            continue

        big_chunk_idx = out_chunk_idx
        small_chunk_idx = in_chunk_idx
        big_chunk = chunks[out_chunk_idx]
        small_chunk = chunks[in_chunk_idx]

        if len(big_chunk) < len(small_chunk):
            big_chunk_idx, small_chunk_idx = small_chunk_idx, big_chunk_idx
            big_chunk, small_chunk = small_chunk, big_chunk

        for piece in small_chunk:
            piece.chunk_idx = big_chunk_idx
            big_chunk.append(piece)

        print(cand_conn)

        if len(big_chunk) == piece_count:
            print("Solved")
            return
