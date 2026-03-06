from puzzle import puzzle_from_image_path

path = "images/moon.png"
puzzle = puzzle_from_image_path(path)
puzzle.debug_segmentation()
