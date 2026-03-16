package com.vrpuzzle.puzzleserver.model.dto

import com.vrpuzzle.puzzleserver.model.type.ChunkSaveData
import com.vrpuzzle.puzzleserver.model.type.PuzzleLayout
import java.time.LocalDateTime

data class UploadedPuzzleMetadataDTO(
    val onlineID: String,
    val name: String,
    val author: String,
    val layout: PuzzleLayout,
    val imageUrl: String?
)