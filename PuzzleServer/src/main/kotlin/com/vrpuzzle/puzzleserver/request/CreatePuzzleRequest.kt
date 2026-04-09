package com.vrpuzzle.puzzleserver.request

import com.vrpuzzle.puzzleserver.model.type.PuzzleLayout

data class CreatePuzzleRequest(
    val name: String,
    val layout: PuzzleLayout,
)