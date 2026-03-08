from puzzle import puzzle_from_image_path

path = "images/moon.png"
puzzle = puzzle_from_image_path(path)
puzzle.debug_segmentation()

# for piece in puzzle.pieces:
#     for p1 in puzzle.pieces:
#         piece.best_fit(p1)

pieces = puzzle.pieces
pieces[0].best_fit_inside(pieces[1])

print(pieces[0].piece_caves)
print(pieces[0].piece_hull_sections)

