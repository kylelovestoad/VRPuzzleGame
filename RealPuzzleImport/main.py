from puzzle import puzzle_from_image
from puzzle_solver import solve

path = "images/IMG_0233.jpg"
puzzle = puzzle_from_image(path)
puzzle.debug_segmentation()

# print(solve(puzzle))
