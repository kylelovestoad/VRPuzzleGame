import cv2
import numpy as np
from rembg import remove

from numpy import ndarray

from piece import Piece, piece_from_contour

# have to just tune these params for now
# seems good for now with black background and light puzzle
BORDER_SAMPLE_MARGIN = 25
BACKGROUND_DISTANCE_THRESHOLD = 39
NOISY_BACKGROUND_MARKER = 1
CLEANED_BACKGROUND_MARKER = 2
CONNECTED_NEIGHBORS = 8
PIECE_MIN_AREA = 128
PIECE_LABEL_START = 1
GAUSSIAN_BLUR_KERNEL_SIZE = (7, 7)
GAUSSIAN_BLUR_SIGMA_X = 0
SEGMENTATION_GRAYSCALE_THRESHOLD = 127


class Puzzle:
    image: ndarray
    piece_mask: ndarray
    pieces: list[Piece]

    def __init__(
        self,
        image: ndarray,
        piece_mask: ndarray,
        pieces: list[Piece]
    ):
        self.image = image
        self.piece_mask = piece_mask
        self.pieces = pieces

    def __iter__(self):
        return iter(self.pieces)

    def piece_count(self):
        return len(self.pieces)

    def debug_segmentation(self):
        image = self.image
        piece_mask = self.piece_mask
        contours = [*map(lambda piece: piece.to_contour(), self.pieces)]

        red = (0, 0, 255)
        green = (0, 255, 0)
        blue = (255, 0, 0)
        purple = (255, 0, 255)
        all_contours = -1

        output = np.zeros_like(image)
        output[piece_mask == 255] = image[piece_mask == 255]

        points_from = [
            [551, 1478],
            [986, 1032],
            [1369, 2366]
        ]
        points_to = [
            [1597, 941],
            [1599, 1117],
            [598, 1918]
        ]

        colors = [red, green, purple]

        for p_from, p_to, color in zip(points_from, points_to, colors):
            cv2.circle(output, tuple(np.array(p_from).astype(int)), radius=16, color=color, thickness=-1)
            cv2.circle(output, tuple(np.array(p_to).astype(int)), radius=16, color=color, thickness=-1)

        convex_hulls = []

        for con in contours:
            convex_hulls.append(cv2.convexHull(con))

        cv2.drawContours(
            output,
            contours,
            all_contours,
            red,
            thickness=2
        )

        cv2.drawContours(
            output,
            convex_hulls,
            all_contours,
            green,
            thickness=2
        )

        for con in contours:
            print(f"Contour Area: {cv2.contourArea(con)}")

            curr_hull = cv2.convexHull(con, returnPoints=False)
            defects = cv2.convexityDefects(con, curr_hull)

            if defects is not None:
                for i in range(defects.shape[0]):
                    start_idx, end_idx, deepest_idx, scaled_depth = defects[i, 0]

                    start = con[start_idx][0]
                    end = con[end_idx][0]
                    deepest = con[deepest_idx][0]
                    depth = scaled_depth / 256.0

                    if depth > 10:
                        cv2.line(output, start, deepest, red, 2)
                        cv2.line(output, deepest, end, blue, 2)

        output_path = "debug_images/pieces.png"
        cv2.imwrite(output_path, output)

        print(f"Num Pieces: {len(contours)}")

        for con in contours:
            print(f"Contour pieces num: {len(con)}")

        print(f"Debug image path: {output_path}")


def _get_piece_mask(image):
    noisy_background_removed = remove(image, only_mask=True)

    noisy_piece_mask = np.array(noisy_background_removed)

    blurred_piece_mask = cv2.GaussianBlur(noisy_piece_mask, GAUSSIAN_BLUR_KERNEL_SIZE, GAUSSIAN_BLUR_SIGMA_X)

    _, clean_piece_mask = cv2.threshold(
        blurred_piece_mask,
        SEGMENTATION_GRAYSCALE_THRESHOLD,
        255,
        cv2.THRESH_BINARY
    )

    return clean_piece_mask


def _get_pieces(piece_mask):
    initial_contours, _ = cv2.findContours(
        piece_mask,
        cv2.RETR_EXTERNAL,
        cv2.CHAIN_APPROX_SIMPLE
    )

    pieces = [*map(piece_from_contour, initial_contours)]

    return pieces


def puzzle_from_image(image_path):
    image = cv2.imread(image_path)

    piece_mask = _get_piece_mask(image)
    pieces = _get_pieces(piece_mask)

    return Puzzle(image, piece_mask, pieces)
