package com.vrpuzzle.puzzleserver.model.dto

import com.vrpuzzle.puzzleserver.model.type.ChunkSaveData
import java.time.LocalDateTime

data class PuzzleSaveDataDTO(
    val id: String,
    val metaUserId: String,
    val puzzleMetadata: PuzzleMetadataDTO,
    val chunks: List<ChunkSaveData>?,
    val clockBase: Int,
    val clockTimestamp: LocalDateTime?
)