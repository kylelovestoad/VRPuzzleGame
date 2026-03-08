from puzzle import puzzle_from_image_path
from puzzle_solver import _best_fit_inside, solve

path = "images/moon.png"
puzzle = puzzle_from_image_path(path)
puzzle.debug_segmentation()

# for piece in puzzle.pieces:
#     for p1 in puzzle.pieces:
#         piece.best_fit(p1)

pieces = puzzle.pieces
# _best_fit_inside(pieces[0], pieces[1])

print(solve(puzzle))

