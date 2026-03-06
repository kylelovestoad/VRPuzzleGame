import numpy as np
from scipy.interpolate import make_splprep


NUM_PIECE_POINTS = 1024


class Piece:
    def __init__(self, spline):
        # need more fields later
        self.spline = spline

    def as_contour(self):
        param = np.linspace(0, 1, NUM_PIECE_POINTS)
        x, y = self.spline(param)

        res_array = [[[int(xi), int(yi)]] for xi, yi in zip(x, y)]
        return np.asarray(res_array, dtype=np.int32)


class PieceSpline:
    def __init__(self, spline, smooth_param, noisy_param):
        self.spline = spline
        self.smoothed_params = smooth_param
        self.noisy_param = noisy_param

    def __call__(self, *args):
        params = np.interp(*args, self.smoothed_params, self.noisy_param)

        return self.spline(params)


def smooth_parameter_mapping(spline):
    noisy_params = np.linspace(0, 1, NUM_PIECE_POINTS)
    x, y = spline(noisy_params)

    diffs_x = np.diff(x)
    diffs_y = np.diff(y)
    smooth_distances = np.hypot(diffs_x, diffs_y)

    cumulative_smooth_distances = np.concatenate([[0], np.cumsum(smooth_distances)])
    smooth_arc_length = cumulative_smooth_distances[-1]

    smoothed_params = cumulative_smooth_distances / smooth_arc_length

    return smoothed_params, noisy_params


# https://agniva.me/scipy/2016/10/25/contour-smoothing.html
def piece_from_contour(contour):
    x = contour[:, 0, 0].tolist()
    x.append(x[0])
    y = contour[:, 0, 1].tolist()
    y.append(y[0])
    contour_points = len(x)

    spline, u = make_splprep([x, y], s=contour_points)

    smoothed_params, fixed_params = smooth_parameter_mapping(spline)
    piece_spline = PieceSpline(spline, smoothed_params, fixed_params)

    return Piece(piece_spline)
