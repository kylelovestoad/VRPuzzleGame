from pydantic import BaseModel
from pydantic.alias_generators import to_camel

from piece_response import PieceResponse, get_piece_response
from puzzle import Puzzle


class PuzzleResponse(BaseModel):
    solved_puzzle_image: str
    initial_piece_cuts: list[PieceResponse]
    puzzle_width: float
    puzzle_height: float

    model_config = {"populate_by_name": True, "alias_generator": to_camel}


def get_puzzle_response(puzzle: Puzzle) -> PuzzleResponse:
    initial_piece_cuts = []

    for piece in puzzle.pieces:
        res = get_piece_response(piece)
        initial_piece_cuts.append(res)
        
    game_width, game_height = puzzle.game_width_and_height()

    print(game_width, game_height)

    return PuzzleResponse.model_validate({
        "solved_puzzle_image": puzzle.generate_solved_image(),
        "initial_piece_cuts": initial_piece_cuts,
        "puzzle_width": game_width,
        "puzzle_height": game_height
    })
