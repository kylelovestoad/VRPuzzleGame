package com.vrpuzzle.puzzleserver.request

import com.vrpuzzle.puzzleserver.model.entity.PuzzleMetadata
import com.vrpuzzle.puzzleserver.model.type.ChunkSaveData

data class UpdatePuzzleSaveDataRequest(
    val chunks: List<ChunkSaveData>?,
    val elapsedTime: Float
)