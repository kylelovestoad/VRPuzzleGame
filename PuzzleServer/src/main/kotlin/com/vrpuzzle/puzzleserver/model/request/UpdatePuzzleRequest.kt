package com.vrpuzzle.puzzleserver.model.request

import com.vrpuzzle.puzzleserver.model.type.PuzzleLayout

data class UpdatePuzzleRequest(
    val name: String?,
    val author: String?,
    val layout: PuzzleLayout?,
)