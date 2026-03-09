from puzzle import puzzle_from_image
from puzzle_solver import solve

path = "images/moon.png"
puzzle = puzzle_from_image(path)
puzzle.debug_segmentation()

solve(puzzle)

puzzle.generate_solved_image()
