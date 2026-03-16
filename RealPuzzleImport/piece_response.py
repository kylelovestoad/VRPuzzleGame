from pydantic import BaseModel
from pydantic.alias_generators import to_camel

from piece import Piece


class Vector2(BaseModel):
    x: float
    y: float


class Vector3(BaseModel):
    x: float
    y: float
    z: float


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


def get_piece_response(piece: Piece) -> PieceResponse:
    solution_location = _get_solution_location_response(piece.solution_location)
    border_points = _get_border_points_response(piece.local_border_points)

    return PieceResponse.model_validate({
        "piece_index": 0, # TODO: fill in
        "neighbor_indices": [], # TODO: fill in
        "solution_location": solution_location,
        "border_points": border_points,
    })
