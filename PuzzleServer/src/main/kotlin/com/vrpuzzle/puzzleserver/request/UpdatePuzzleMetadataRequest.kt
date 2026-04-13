package com.vrpuzzle.puzzleserver.request

import com.vrpuzzle.puzzleserver.model.type.PuzzleLayout

data class UpdatePuzzleMetadataRequest(
    val name: String?,
    val layout: PuzzleLayout?,
)