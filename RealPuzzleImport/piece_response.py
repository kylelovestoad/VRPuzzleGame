import numpy as np
from pydantic import BaseModel
from pydantic.alias_generators import to_camel

from piece import Piece


class Vector2(BaseModel):
    x: float
    y: float

    model_config = {"populate_by_name": True, "alias_generator": to_camel}


class Vector3(BaseModel):
    x: float
    y: float
    z: float

    model_config = {"populate_by_name": True, "alias_generator": to_camel}


class PieceResponse(BaseModel):
    piece_index: int
    neighbor_indices: list[int]
    solution_location: Vector3
    border_points: list[Vector2]

    model_config = {"populate_by_name": True, "alias_generator": to_camel}


def _get_solution_location_response(solution_location):
    x, y = solution_location

    solution_location_response = Vector3.model_validate({
        "x": x,
        "y": y,
        "z": 0,
    })

    return solution_location_response


def _get_border_points_response(border_points):
    x, y = border_points
    border_points_response = []

    for x0, y0 in zip(x, y):
        curr_point = Vector2.model_validate({
            "x": x0,
            "y": y0,
        })

        border_points_response.append(curr_point)

    return border_points_response


def is_clockwise(contour: np.ndarray) -> bool:
    x = contour[0, :]
    y = contour[1, :]

    clockwise = np.sum(x * np.roll(y, -1) - np.roll(x, -1) * y) < 0

    return clockwise


def get_piece_response(piece: Piece) -> PieceResponse:
    solution_location = _get_solution_location_response(piece.solution_location)
    border_points = _get_border_points_response(piece.local_border_points)

    x, y = piece.local_border_points
    print(max(x), min(x), max(y), min(y))
    print(solution_location)

    import numpy as np
    import matplotlib.pyplot as plt

    data = piece.local_border_points

    plt.plot(data[0], data[1])
    plt.show()

    assert is_clockwise(data)
    print(f"Clockwise {is_clockwise(data)}")

    return PieceResponse.model_validate({
        "piece_index": 0, # TODO: fill in
        "neighbor_indices": [], # TODO: fill in
        "solution_location": solution_location,
        "border_points": border_points,
    })
