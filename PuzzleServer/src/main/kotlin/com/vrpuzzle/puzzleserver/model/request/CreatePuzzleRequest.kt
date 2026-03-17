package com.vrpuzzle.puzzleserver.model.request

import com.vrpuzzle.puzzleserver.model.type.PuzzleLayout

data class CreatePuzzleRequest(
    val name: String,
    val author: String,
    val layout: PuzzleLayout,
)