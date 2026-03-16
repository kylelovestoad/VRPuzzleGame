import unittest
import numpy as np
from transformation import Transformation, IDENTITY_TRANSFORMATION, get_transformation


PLACES_TOLERANCE = 6


class TestTransformation(unittest.TestCase):

    def test_identity_applies_no_change(self):
        points = np.array([[0], [0]])
        result = IDENTITY_TRANSFORMATION(points)

        np.testing.assert_array_almost_equal(result, points)

    def test_composing_identity(self):
        transformation = Transformation(2, 3)
        composed = transformation(IDENTITY_TRANSFORMATION)

        self.assertAlmostEqual(transformation.rotation, composed.rotation, places=PLACES_TOLERANCE)
        self.assertAlmostEqual(transformation.translation, composed.translation, places=PLACES_TOLERANCE)

    def test_inversion(self):
        transformation = Transformation(2, 3)
        inverted = transformation.invert()
        expected_ident = transformation(inverted)

        self.assertAlmostEqual(IDENTITY_TRANSFORMATION.rotation, expected_ident.rotation, places=PLACES_TOLERANCE)
        self.assertAlmostEqual(IDENTITY_TRANSFORMATION.translation, expected_ident.translation, places=PLACES_TOLERANCE)

    def test_transformation_applies(self):
        transformation = Transformation(2, 3)
        inverted = transformation.invert()

        points = np.array([[2], [1]])
        transformed = transformation(points)

        expected_original = inverted(transformed)

        np.testing.assert_array_almost_equal(expected_original, points)


    def test_transformation_keeps_original_scale(self):
        from_start = np.array([0.0, 0.0])
        from_end = np.array([1.0, 0.0])
        to_start = np.array([2.0, 2.0])
        to_end = np.array([7.0, 5.0])

        transformation = get_transformation(from_start, from_end, to_start, to_end)

        original_point0 = np.array([[0.0], [0.0]])
        original_point1 = np.array([[3.0], [4.0]])

        transformed_point0 = transformation(original_point0)
        transformed_point1 = transformation(original_point1)

        original_dist = np.linalg.norm(original_point1 - original_point0)
        transformed_dist = np.linalg.norm(transformed_point1 - transformed_point0)

        self.assertAlmostEqual(original_dist, transformed_dist, places=PLACES_TOLERANCE)
