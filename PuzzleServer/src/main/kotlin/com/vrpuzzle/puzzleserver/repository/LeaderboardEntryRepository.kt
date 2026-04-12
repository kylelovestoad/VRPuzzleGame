package com.vrpuzzle.puzzleserver.repository

import com.vrpuzzle.puzzleserver.model.entity.LeaderboardEntry
import org.bson.types.ObjectId
import org.springframework.data.mongodb.repository.MongoRepository

interface LeaderboardEntryRepository : MongoRepository<LeaderboardEntry, ObjectId> {
    fun findByUserIdAndPuzzleId(userId: String, puzzleId: ObjectId): LeaderboardEntry?
    fun deleteAllByPuzzleId(puzzleId: ObjectId)
    fun findAllByPuzzleIdOrderByTimeAsc(puzzleId: ObjectId): MutableList<LeaderboardEntry>
}