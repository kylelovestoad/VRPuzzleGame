package com.vrpuzzle.puzzleserver.model.dto

import com.vrpuzzle.puzzleserver.model.type.PuzzleLayout
import org.bson.types.ObjectId

data class PuzzleMetadataDTO(
    val onlineID: String,
    val name: String,
    val author: String,
    val layout: PuzzleLayout,
    val content: ContentDTO
)