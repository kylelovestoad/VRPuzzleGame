package com.vrpuzzle.puzzleserver.model.dto

import com.vrpuzzle.puzzleserver.model.type.PuzzleLayout

data class PuzzleMetadataDTO(
    val onlineID: String,
    val name: String,
    val authorId: String,
    val author: String,
    val layout: PuzzleLayout,
    val content: ContentDTO
)