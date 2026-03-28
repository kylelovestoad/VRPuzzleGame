import logging
from contextlib import asynccontextmanager

import uvicorn
from fastapi import FastAPI, File, UploadFile, HTTPException
import cv2
import numpy as np

from puzzle import puzzle_from_image
from puzzle_response import get_puzzle_response, PuzzleResponse
from puzzle_solver import solve

# TODO make config later
PORT = 6969

log = logging.getLogger("uvicorn.error")


@asynccontextmanager
async def lifespan(_: FastAPI):
    log.info(f"Listening on http://localhost:{PORT}")
    yield
    log.info("Shutting down")

app = FastAPI(lifespan=lifespan)


@app.post("/real-puzzle", response_model=PuzzleResponse)
def real_puzzle(file: UploadFile = File(...)):
    log.info("Generating a real puzzle")

    contents = file.file.read()

    image_arr = np.frombuffer(contents, np.uint8)
    image = cv2.imdecode(image_arr, cv2.IMREAD_COLOR)

    print("Shape", image.shape)

    if image is None:
        raise HTTPException(400, "Could not decode image")

    puzzle = puzzle_from_image(image)
    solve(puzzle)
    response = get_puzzle_response(puzzle)

    return response


if __name__ == "__main__":
    uvicorn.run("main:app", host="0.0.0.0", port=PORT, reload=True)
