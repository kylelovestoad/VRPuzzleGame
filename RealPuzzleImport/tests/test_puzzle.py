import unittest
import numpy as np
from puzzle import _clean_piece_mask, puzzle_from_image, _get_background_color


class TestPuzzle(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.shape = (1024, 1024)

    def draw_piece(self, x, y, w, h):
        self.mask[y:y + h, x:x + w] = 255
        self.image[y:y + h, x:x + w] = np.array([255, 255, 255])

    def setUp(self):
        self.mask = np.zeros(self.shape, dtype=np.uint8)
        self.image = np.zeros((*self.shape, 3), dtype=np.uint8)

    def test_removes_tiny_spec(self):
        self.draw_piece(x=0, y=0, w=1, h=1)

        cloeaned_mask = _clean_piece_mask(self.mask)

        self.assertTrue(np.all(cloeaned_mask == 0))

    def test_keeps_large_enough_piece(self):
        self.draw_piece(x=16, y=16, w=32, h=32)

        cleaned_mask = _clean_piece_mask(self.mask)

        self.assertEqual(cleaned_mask[16, 16], 1)

    def test_multiple_large_enough_pieces(self):
        self.draw_piece(x=16, y=16, w=32, h=32)
        self.draw_piece(x=33, y=32, w=32, h=32)

        cleaned_mask = _clean_piece_mask(self.mask)

        self.assertEqual(cleaned_mask[16, 16], 1)
        self.assertEqual(cleaned_mask[32, 33], 1)

    def test_filters_small_and_keeps_large(self):
        self.draw_piece(x=16, y=16, w=1, h=1)
        self.draw_piece(x=32, y=32, w=32, h=32)

        result = _clean_piece_mask(self.mask)

        self.assertEqual(result[16, 16], 0)
        self.assertEqual(result[32, 32], 1)

    def test_single_piece_detected_no_noise(self):
        self.draw_piece(x=32, y=32, w=32, h=32)

        puzzle = puzzle_from_image(self.image)

        self.assertEqual(puzzle.piece_count(), 1)

    def test_single_piece_detected_with_noise(self):
        self.draw_piece(x=16, y=16, w=1, h=1)
        self.draw_piece(x=32, y=32, w=32, h=32)

        puzzle = puzzle_from_image(self.image)

        self.assertEqual(puzzle.piece_count(), 1)

    def test_multiple_piece_detected_no_noise(self):
        self.draw_piece(x=32, y=32, w=32, h=32)
        self.draw_piece(x=65, y=64, w=32, h=32)

        puzzle = puzzle_from_image(self.image)

        self.assertEqual(puzzle.piece_count(), 2)

    def test_multiple_piece_detected_with_noise(self):
        self.draw_piece(x=16, y=16, w=1, h=1)
        self.draw_piece(x=32, y=32, w=32, h=32)
        self.draw_piece(x=100, y=100, w=32, h=32)

        puzzle = puzzle_from_image(self.image)

        self.assertEqual(puzzle.piece_count(), 2)

    def test_background_median_calculated(self):
        self.image[0, :] = 4
        self.image[-1, :] = 4
        self.image[:, 0] = 4
        self.image[:, -1] = 4

        bg_color = _get_background_color(self.image)

        np.testing.assert_array_almost_equal(bg_color, np.array([4, 4, 4]))
