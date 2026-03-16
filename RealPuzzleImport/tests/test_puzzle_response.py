import unittest

import cv2

from puzzle import puzzle_from_image
from puzzle_response import get_puzzle_response
from puzzle_solver import solve


class TestPuzzleResponse(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.image_matrix = cv2.imread("/home/joe/PycharmProjects/VRPuzzleGame/RealPuzzleImport/images/moon.png")
        cls.puzzle = puzzle_from_image(cls.image_matrix)
        solve(cls.puzzle)

    def test_response_fields_set(self):
        res = get_puzzle_response(self.puzzle)

        self.assertIsNotNone(res.solved_puzzle_image)
        self.assertIsNotNone(res.initial_piece_cuts)

    def test_correct_number_of_piece_cuts(self):
        res = get_puzzle_response(self.puzzle)

        self.assertEqual(self.puzzle.piece_count(), len(res.initial_piece_cuts))
