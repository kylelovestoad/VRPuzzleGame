package com.vrpuzzle.puzzleserver.model.dto

import com.vrpuzzle.puzzleserver.model.type.ChunkSaveData
import org.bson.types.ObjectId
import java.time.LocalDateTime

data class PuzzleSaveDataDTO(
    val id: String,
    val metaUserId: String,
    val puzzleId: ObjectId,
    val chunks: List<ChunkSaveData>?,
    val elapsedTime: Float,
)