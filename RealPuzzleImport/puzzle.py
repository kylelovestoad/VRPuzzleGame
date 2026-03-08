import cv2
import numpy as np

from piece import piece_from_contour

# have to just tune these params for now
# seems good for now with black background and light puzzle
BORDER_SAMPLE_MARGIN = 1
BACKGROUND_DISTANCE_THRESHOLD = 32
NOISY_BACKGROUND_MARKER = 1
CLEANED_BACKGROUND_MARKER = 2
CONNECTED_NEIGHBORS = 8
PIECE_MIN_AREA = 128
PIECE_LABEL_START = 1


class Puzzle:
    def __init__(self, image, piece_mask, piece_contours):
        self.image = image
        self.piece_mask = piece_mask
        self.pieces = piece_contours

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

        points_purple = [np.array([1373.881970724489, 2369.2405644428554]), np.array([599.7894544311248, 1920.563699312606])]

        for p in points_purple:
            cv2.circle(output, tuple(p.astype(int)), radius=16, color=(255, 0, 255), thickness=-1)

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


def _flood_fill_piece_interiors(initial_background, rows, cols):
    flood_fill_mask = np.zeros((rows + 2, cols + 2), np.uint8)
    background_mask = np.zeros((rows, cols), np.uint8)

    initial_background[0, 0] = NOISY_BACKGROUND_MARKER
    cv2.floodFill(
        initial_background,
        flood_fill_mask,
        (0, 0),
        CLEANED_BACKGROUND_MARKER,
        flags=CONNECTED_NEIGHBORS
    )

    initial_piece_mask = np.zeros_like(background_mask)
    initial_piece_mask[initial_background != CLEANED_BACKGROUND_MARKER] = 1


def _get_background_mask(image):
    rows, cols = image.shape[:2]

    background_distance = _pixel_background_difference(image)

    background_mask = np.zeros((rows, cols), np.uint8)
    background_mask[background_distance <= BACKGROUND_DISTANCE_THRESHOLD] = NOISY_BACKGROUND_MARKER
    _flood_fill_piece_interiors(background_mask, rows, cols)

    return background_mask


def _clean_piece_mask(noisy_piece_mask):
    num_labels, labels, stats, _ = cv2.connectedComponentsWithStats(noisy_piece_mask, connectivity=CONNECTED_NEIGHBORS)

    cleaned_piece_mask = np.zeros_like(noisy_piece_mask)

    for i in range(PIECE_LABEL_START, num_labels):
        if stats[i, cv2.CC_STAT_AREA] > PIECE_MIN_AREA:
            cleaned_piece_mask[labels == i] = 1

    return cleaned_piece_mask


def _get_piece_mask(image):
    background_mask = _get_background_mask(image)

    noisy_piece_mask = np.zeros_like(background_mask)
    noisy_piece_mask[background_mask != CLEANED_BACKGROUND_MARKER] = 1
    cleaned_mask = _clean_piece_mask(noisy_piece_mask)

    return cleaned_mask


def _get_pieces(piece_mask):
    noisy_contours, _ = cv2.findContours(
        piece_mask,
        cv2.RETR_EXTERNAL,
        cv2.CHAIN_APPROX_NONE
    )

    pieces = [*map(piece_from_contour, noisy_contours)]

    return pieces


def puzzle_from_image_path(image_path):
    image = cv2.imread(image_path)

    piece_mask = _get_piece_mask(image)
    pieces = _get_pieces(piece_mask)

    return Puzzle(image, piece_mask, pieces)
