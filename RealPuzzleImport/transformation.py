import numpy as np


class Transformation:
    def __init__(self, from_point: complex, to_point: complex, rotation: complex):
        self.from_point = from_point
        self.to_point = to_point
        self.rotation = rotation

    def invert(self):
        return Transformation(self.to_point, self.from_point, 1 / self.rotation)

    def __call__(self, *args):
        from_offset = args[0] - self.from_point
        transformed_complex = self.to_point + self.rotation * from_offset

        return transformed_complex

    def __repr__(self):
        return f"{self.from_point} -> {self.to_point}, rotation: {self.rotation}"


def _identity_transformation():
    origin = complex(0, 0)
    ident_rot = complex(1, 0)

    return Transformation(origin, origin, ident_rot)


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

    return Transformation(from_start_complex, to_start_complex, rotation)
