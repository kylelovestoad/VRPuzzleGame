import unittest

import cv2
import numpy as np

from piece_response import get_piece_response
from puzzle import puzzle_from_image
from puzzle_solver import solve


class TestPieceResponse(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.image_matrix = cv2.imread("images/moon.png")

        print(cls.image_matrix)

        cls.puzzle = puzzle_from_image(cls.image_matrix)
        cls.test_piece = cls.puzzle.pieces[0]
        solve(cls.puzzle)

    def test_response_fields_set(self):
        res = get_piece_response(self.test_piece)

        self.assertIsNotNone(res.solution_location)
        self.assertIsNotNone(res.border_points)
        self.assertIsNotNone(res.piece_index)
        self.assertIsNotNone(res.neighbor_indices)

    def test_piece_is_clockwise(self):
        border_points = self.test_piece.local_border_points

        x = border_points[0, :]
        y = border_points[1, :]

        clockwise = np.sum(x * np.roll(y, -1) - np.roll(x, -1) * y) < 0

        self.assertTrue(clockwise)
