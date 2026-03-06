import cv2
import numpy as np
from scipy.interpolate import make_splprep


# have to just tune these params for now
# seems good for now with black background and light puzzle
border_sample_margin = 1
background_distance_threshold = 32
noisy_background_marker = 1
cleaned_background_marker = 2
connected_neighbors = 8
piece_min_area = 128
piece_label_start = 1
num_piece_points = 1024


def get_background_color(image):
    border_pixels = np.concatenate([
        image[:border_sample_margin, :].reshape(-1, 3),
        image[-border_sample_margin:, :].reshape(-1, 3),
        image[:, :border_sample_margin].reshape(-1, 3),
        image[:, -border_sample_margin:].reshape(-1, 3),
    ])

    return np.median(border_pixels, axis=0)


def pixel_background_difference(image):
    background_color = get_background_color(image)

    diff = np.abs(image - background_color)
    squared_diff = np.sum(diff ** 2, axis=2)
    difference_distance = np.sqrt(squared_diff)

    return difference_distance


def flood_fill_piece_interiors(initial_background, rows, cols):
    flood_fill_mask = np.zeros((rows + 2, cols + 2), np.uint8)
    background_mask = np.zeros((rows, cols), np.uint8)

    initial_background[0, 0] = noisy_background_marker
    cv2.floodFill(initial_background, flood_fill_mask, (0, 0), cleaned_background_marker, flags=connected_neighbors)

    initial_piece_mask = np.zeros_like(background_mask)
    initial_piece_mask[initial_background != cleaned_background_marker] = 1


def get_background_mask(image):
    rows, cols = image.shape[:2]

    background_distance = pixel_background_difference(image)

    background_mask = np.zeros((rows, cols), np.uint8)
    background_mask[background_distance <= background_distance_threshold] = noisy_background_marker
    flood_fill_piece_interiors(background_mask, rows, cols)

    return background_mask


def clean_piece_mask(noisy_piece_mask):
    num_labels, labels, stats, _ = cv2.connectedComponentsWithStats(noisy_piece_mask, connectivity=connected_neighbors)

    cleaned_piece_mask = np.zeros_like(noisy_piece_mask)

    for i in range(piece_label_start, num_labels):
        if stats[i, cv2.CC_STAT_AREA] > piece_min_area:
            cleaned_piece_mask[labels == i] = 1

    return cleaned_piece_mask


def get_piece_mask(image):
    background_mask = get_background_mask(image)

    noisy_piece_mask = np.zeros_like(background_mask)
    noisy_piece_mask[background_mask != cleaned_background_marker] = 1
    cleaned_mask = clean_piece_mask(noisy_piece_mask)

    return cleaned_mask

def get_piece_contours(piece_mask):
    noisy_contours, _ = cv2.findContours(
        piece_mask,
        cv2.RETR_EXTERNAL,
        cv2.CHAIN_APPROX_NONE
    )

    # https://agniva.me/scipy/2016/10/25/contour-smoothing.html
    smoothened_contours = []

    for contour in noisy_contours:
        x = contour[:, 0, 0].tolist()
        y = contour[:, 0, 1].tolist()
        m = len(x)

        spline, u = make_splprep([x, y], s=m)

        u_new = np.linspace(np.min(u), np.max(u), num_piece_points)
        x_new, y_new = spline(u_new)

        res_array = [[[int(xi), int(yi)]] for xi, yi in zip(x_new, y_new)]
        smoothened_contours.append(np.asarray(res_array, dtype=np.int32))

    return smoothened_contours


def get_puzzle_piece_outlines(image):
    piece_mask = get_piece_mask(image)
    piece_contours = get_piece_contours(piece_mask)

    return piece_mask, piece_contours


def debug_pieces(image, piece_mask, contours):
    red = (0, 0, 255)
    green = (0, 255, 0)
    blue = (255, 0, 0)
    all_contours = -1

    output = np.zeros_like(image)
    output[piece_mask == 1] = image[piece_mask == 1]

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

        # print(len(defects), defects)

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


image_path = "images/moon.png"
image = cv2.imread(image_path)
piece_mask, contours = get_puzzle_piece_outlines(image)
debug_pieces(image, piece_mask, contours)
