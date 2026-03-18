package com.vrpuzzle.puzzleserver.repository

import com.vrpuzzle.puzzleserver.model.entity.PuzzleMetadata
import org.bson.types.ObjectId
import org.springframework.data.mongodb.repository.MongoRepository

interface PuzzleMetadataRepository : MongoRepository<PuzzleMetadata, ObjectId>