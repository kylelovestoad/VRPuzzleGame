import base64

import cv2
import numpy as np

from numpy import ndarray
from rembg import remove

from piece import piece_from_contour, Piece, NUM_PIECE_POINTS

# have to just tune these params for now
# seems good for now with black background and light puzzle
BORDER_SAMPLE_MARGIN = 1
BACKGROUND_DISTANCE_THRESHOLD = 32
PIECE_MIN_AREA = 128
PIECE_LABEL_START = 1
CLEANED_BACKGROUND_MARKER = 1
CONNECTED_NEIGHBORS = 8
GAUSSIAN_BLUR_KERNEL_SIZE = (7, 7)
GAUSSIAN_BLUR_SIGMA_X = 0
SEGMENTATION_GRAYSCALE_THRESHOLD = 127
PUZZLE_GAME_HEIGHT = 0.3


class Puzzle:
    image: ndarray
    piece_mask: ndarray
    pieces: list[Piece]
    game_height: float

    def __init__(self, image, piece_mask, pieces):
        self.image = image
        self.piece_mask = piece_mask
        self.pieces = pieces
        self.game_height = PUZZLE_GAME_HEIGHT

    def __iter__(self):
        return iter(self.pieces)

    def piece_count(self):
        return len(self.pieces)

    def width_height_ratio(self):
        height, width = self.image.shape[:2]

        return width / height

    def game_width_and_height(self):
        ratio = self.width_height_ratio()

        return self.game_height * ratio, self.game_height

    def generate_solved_image(self) -> str:
        rows, cols = self.image.shape[:2]

        output = np.zeros((rows, cols, 3), dtype=self.image.dtype)

        min_row = rows
        min_col = cols
        piece_points = []

        for piece in self:
            xy_original, xy_final = piece.transformed_shape(self.image.shape[:2])
            piece_points.append((xy_original, xy_final))
            x, y = xy_final

            min_row = min(min_row, np.min(y))
            min_col = min(min_col, np.min(x))

        for xy_original, xy_final in piece_points:
            x0, y0 = xy_original
            x, y = xy_final

            for x01, y01, x1, y1 in zip(x0, y0, x, y):
                output[int(y1), int(x1)] = self.image[int(y01), int(x01)]

        output_path = "debug_images/solved.png"
        cv2.imwrite(output_path, output)

        _, buffer = cv2.imencode(".png", output)
        encoded_image = base64.b64encode(buffer).decode("utf-8")

        return encoded_image


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
        output[piece_mask == 1] = image[piece_mask == 1]

        cv2.circle(output, (417, 698), radius=16, color=green, thickness=-1)
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


def _get_background_color(image):
    border_pixels = np.concatenate([
        image[:BORDER_SAMPLE_MARGIN, :].reshape(-1, 3),
        image[-BORDER_SAMPLE_MARGIN:, :].reshape(-1, 3),
        image[:, :BORDER_SAMPLE_MARGIN].reshape(-1, 3),
        image[:, -BORDER_SAMPLE_MARGIN:].reshape(-1, 3),
    ])

    return np.median(border_pixels, axis=0)


def _pixel_background_difference(image):
    background_color = _get_background_color(image)

    diff = np.abs(image - background_color)
    squared_diff = np.sum(diff ** 2, axis=2)
    difference_distance = np.sqrt(squared_diff)

    return difference_distance


def _flood_fill_piece_interiors(piece_mask):
    rows, cols = piece_mask.shape

    flood_fill_mask = np.zeros((rows + 2, cols + 2), np.uint8)

    cv2.floodFill(
        piece_mask,
        flood_fill_mask,
        (0, 0),
        CLEANED_BACKGROUND_MARKER,
        flags=CONNECTED_NEIGHBORS
    )

    piece_mask[:] = np.where(piece_mask == CLEANED_BACKGROUND_MARKER, 0, 255)


def _clean_piece_mask(noisy_piece_mask):
    num_labels, labels, stats, _ = cv2.connectedComponentsWithStats(noisy_piece_mask, connectivity=CONNECTED_NEIGHBORS)

    cleaned_piece_mask = np.zeros_like(noisy_piece_mask)

    for i in range(PIECE_LABEL_START, num_labels):
        if stats[i, cv2.CC_STAT_AREA] > PIECE_MIN_AREA:
            cleaned_piece_mask[labels == i] = 1

    return cleaned_piece_mask


def _get_piece_mask(image):
    rows, cols = image.shape[:2]

    background_distance = _pixel_background_difference(image)

    piece_mask = np.zeros((rows, cols), np.uint8)
    piece_mask[background_distance > BACKGROUND_DISTANCE_THRESHOLD] = 255
    _flood_fill_piece_interiors(piece_mask)
    piece_mask = _clean_piece_mask(piece_mask)

    return piece_mask


def _get_piece_mask_ml(image):
    noisy_background_removed = remove(image, only_mask=True)

    noisy_piece_mask = np.array(noisy_background_removed)

    blurred_piece_mask = cv2.GaussianBlur(noisy_piece_mask, GAUSSIAN_BLUR_KERNEL_SIZE, GAUSSIAN_BLUR_SIGMA_X)

    _, clean_piece_mask = cv2.threshold(
        blurred_piece_mask,
        SEGMENTATION_GRAYSCALE_THRESHOLD,
        255,
        cv2.THRESH_BINARY
    )

    _flood_fill_piece_interiors(clean_piece_mask)

    return clean_piece_mask


def _get_pieces(image, piece_mask):
    noisy_contours, _ = cv2.findContours(
        piece_mask,
        cv2.RETR_EXTERNAL,
        cv2.CHAIN_APPROX_NONE
    )

    print("Contours", len(noisy_contours))

    pieces = []

    for i, contour in enumerate(noisy_contours):
        piece = piece_from_contour(image, piece_mask, i, contour)
        pieces.append(piece)

    return pieces


def puzzle_from_image(image_matrix):
    piece_mask = _get_piece_mask(image_matrix)
    pieces = _get_pieces(image_matrix, piece_mask)

    return Puzzle(image_matrix, piece_mask, pieces)
