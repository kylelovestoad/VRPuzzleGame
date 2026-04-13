package com.vrpuzzle.puzzleserver.model.entity

import com.vrpuzzle.puzzleserver.model.dto.LeaderboardEntryDTO
import com.vrpuzzle.puzzleserver.model.dto.PuzzleMetadataDTO
import org.bson.types.ObjectId
import org.springframework.data.annotation.Id
import org.springframework.data.mongodb.core.index.CompoundIndex
import org.springframework.data.mongodb.core.index.CompoundIndexes
import org.springframework.data.mongodb.core.mapping.Document
import kotlin.time.Instant

@Document(collection = "leaderboard_entries")
@CompoundIndexes(
    CompoundIndex(name = "userId_puzzleId_unique", def = "{'userId': 1, 'puzzleId': 1}", unique = true),
    CompoundIndex(name = "puzzleId_time", def = "{'puzzleId': 1, 'time': 1}")
)
data class LeaderboardEntry(
    @Id
    val id: ObjectId = ObjectId.get(),
    val puzzleId: ObjectId,
    val userId: String,
    val username: String,
    val time: Float,
) {
    fun toDTO() = LeaderboardEntryDTO(
        id = id.toHexString(),
        puzzleId = puzzleId.toHexString(),
        userId = userId,
        username = username,
        time = time,
    )
}