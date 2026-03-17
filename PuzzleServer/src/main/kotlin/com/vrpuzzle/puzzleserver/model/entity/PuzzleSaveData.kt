package com.vrpuzzle.puzzleserver.model.entity

import com.vrpuzzle.puzzleserver.model.dto.PuzzleSaveDataDTO
import com.vrpuzzle.puzzleserver.model.type.ChunkSaveData
import org.bson.types.ObjectId
import org.springframework.data.annotation.Id
import org.springframework.data.mongodb.core.mapping.Document
import java.time.LocalDateTime

@Document(collection = "puzzle_save_data")
data class PuzzleSaveData(
    @Id
    val id: ObjectId = ObjectId.get(),
    val metaUserId: String,
    val puzzleMetadata: PuzzleMetadata,
    val chunks: List<ChunkSaveData>?,
    val clockBase: Int,
    val clockTimestamp: LocalDateTime?
) {
    fun toDTO() = PuzzleSaveDataDTO(
        id = id.toHexString(),
        metaUserId = metaUserId,
        puzzleMetadata = puzzleMetadata.toDTO(),
        chunks = chunks,
        clockBase = clockBase,
        clockTimestamp = clockTimestamp
    )
}
