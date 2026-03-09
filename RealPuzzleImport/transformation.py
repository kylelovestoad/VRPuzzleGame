import numpy as np


class Transformation:
    rotation: complex
    translation: complex

    def __init__(self, rotation: complex, translation: complex):
        self.rotation = rotation
        self.translation = translation

    def invert(self):
        rotation = 1 / self.rotation
        translation = -self.translation / self.rotation

        return Transformation(rotation, translation)

    def compose(self, other):
        rotation = self.rotation * other.rotation
        translation = self.rotation * other.translation + self.translation

        return Transformation(rotation, translation)

    def apply(self, points):
        x, y = points

        points_complex = x + 1j * y
        transformed_complex = self.rotation * points_complex + self.translation
        transformed = np.array([transformed_complex.real, transformed_complex.imag])

        return transformed

    def __call__(self, *args):
        arg = args[0]
        return self.compose(arg) if type(arg) == Transformation else self.apply(arg)

    def __repr__(self):
        return f"{self.translation}, rotation: {self.rotation}"


def _identity_transformation():
    rotation = complex(1, 0)
    translation = complex(0, 0)

    return Transformation(rotation, translation)


IDENTITY_TRANSFORMATION = _identity_transformation()


def get_transformation(from_start, from_end, to_start, to_end):
    to_vec = to_end - to_start

    from_len = np.linalg.norm(from_end - from_start)
    to_len = np.linalg.norm(to_vec)

    to_end = to_start + (from_len / to_len) * to_vec

    from_start_complex = complex(*from_start)
    from_end_complex = complex(*from_end)
    to_start_complex = complex(*to_start)
    to_end_complex = complex(*to_end)

    rotation = (to_end_complex - to_start_complex) / (from_end_complex - from_start_complex)
    translation = to_start_complex - rotation * from_start_complex

    transformation = Transformation(rotation, translation)
    transformation.from_point = from_start
    transformation.to_point = to_start

    return transformation
