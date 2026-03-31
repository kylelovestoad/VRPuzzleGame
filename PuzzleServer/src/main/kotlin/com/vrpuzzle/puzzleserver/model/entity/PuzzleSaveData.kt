package com.vrpuzzle.puzzleserver.model.entity

import com.vrpuzzle.puzzleserver.model.dto.PuzzleSaveDataDTO
import com.vrpuzzle.puzzleserver.model.type.ChunkSaveData
import org.bson.types.ObjectId
import org.springframework.data.annotation.Id
import org.springframework.data.mongodb.core.mapping.Document

@Document(collection = "puzzle_save_data")
data class PuzzleSaveData(
    @Id
    val id: ObjectId = ObjectId.get(),
    val metaUserId: String,
    val puzzleId: ObjectId,
    val chunks: List<ChunkSaveData>?,
    val elapsedTime: Float
) {
    fun toDTO() = PuzzleSaveDataDTO(
        id = id,
        metaUserId = metaUserId,
        puzzleId = puzzleId,
        chunks = chunks,
        elapsedTime = elapsedTime
    )
}
