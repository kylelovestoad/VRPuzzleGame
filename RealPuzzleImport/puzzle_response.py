from pydantic import BaseModel
from pydantic.alias_generators import to_camel

from piece_response import PieceResponse, get_piece_response
from puzzle import Puzzle


class PuzzleResponse(BaseModel):
    solved_puzzle_image: str
    initial_piece_cuts: list[PieceResponse]
    rows: int
    cols: int
    puzzle_width: float
    puzzle_height: float

    model_config = {"populate_by_name": True, "alias_generator": to_camel}


def calculate_rows_and_cols(puzzle: Puzzle):
    piece_count = len(puzzle.pieces)
    game_width, game_height = puzzle.game_width, puzzle.game_height

    width_height_ratio = game_width / game_height

    optimal_diff = float('inf')
    optimal_rows = 1
    optimal_cols = piece_count

    i = 1

    while i * i <= piece_count:
        if piece_count % i == 0:
            other_count = piece_count // i
            rows, cols = (i, other_count) if game_height < game_width else (other_count, i)
            diff = abs(cols / rows - width_height_ratio)

            if diff < optimal_diff:
                optimal_diff = diff
                optimal_rows = rows
                optimal_cols = cols

        i += 1

    return optimal_rows, optimal_cols


def get_puzzle_response(puzzle: Puzzle) -> PuzzleResponse:
    initial_piece_cuts = []

    for piece in puzzle.pieces:
        res = get_piece_response(piece)
        initial_piece_cuts.append(res)
        
    game_width, game_height = puzzle.game_width, puzzle.game_height
    rows, cols = calculate_rows_and_cols(puzzle)

    print(game_width, game_height)

    return PuzzleResponse.model_validate({
        "solved_puzzle_image": puzzle.generate_solved_image(),
        "initial_piece_cuts": initial_piece_cuts,
        "rows": rows,
        "cols": cols,
        "puzzle_width": game_width,
        "puzzle_height": game_height
    })
