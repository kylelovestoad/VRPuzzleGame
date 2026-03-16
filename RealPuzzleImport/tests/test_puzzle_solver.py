import unittest

import cv2

from puzzle import puzzle_from_image
from puzzle_solver import solve


class TestPuzzleSolver(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.image_matrix = cv2.imread("/home/joe/PycharmProjects/VRPuzzleGame/RealPuzzleImport/images/moon.png")
        cls.puzzle = puzzle_from_image(cls.image_matrix)

    def test_correct_number_of_pieces(self):
        expected_pieces = 4

        self.assertEqual(self.puzzle.piece_count(), expected_pieces)

    def test_solve_without_error(self):
        try:
            solve(self.puzzle)
        except Exception as e:
            self.fail(e)

    def test_pieces_all_connected(self):
        try:
            puzzle = self.puzzle

            solve(puzzle)
            chunk_idx = puzzle.pieces[0].chunk_idx

            for piece in puzzle:
                self.assertEqual(piece.chunk_idx, chunk_idx)

        except Exception as e:
            self.fail(e)

    def test_piece_state_fields_set(self):
        try:
            puzzle = self.puzzle

            solve(puzzle)

            for piece in puzzle:
                self.assertIsNotNone(piece.chunk_idx)
                self.assertIsNotNone(piece.transformation)
                self.assertIsNotNone(piece.used_piece_hull_sections)
                self.assertIsNotNone(piece.used_caves)
                self.assertIsNotNone(piece.solution_location)
                self.assertIsNotNone(piece.local_border_points)

        except Exception as e:
            self.fail(e)


if __name__ == "__main__":
    unittest.main()