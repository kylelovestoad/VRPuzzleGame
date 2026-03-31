package com.vrpuzzle.puzzleserver.repository

import com.vrpuzzle.puzzleserver.model.entity.PuzzleSaveData
import org.bson.types.ObjectId
import org.springframework.data.mongodb.repository.MongoRepository

interface PuzzleSaveDataRepository : MongoRepository<PuzzleSaveData, ObjectId> {
    fun findPuzzleSaveDataByPuzzleIdAndMetaUserId(puzzleId: ObjectId, metaUserId: String): PuzzleSaveData?
}