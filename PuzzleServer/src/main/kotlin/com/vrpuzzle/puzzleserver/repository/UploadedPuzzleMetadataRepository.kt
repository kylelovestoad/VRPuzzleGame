package com.vrpuzzle.puzzleserver.repository

import UploadedPuzzleMetadata
import org.bson.types.ObjectId
import org.springframework.data.mongodb.repository.MongoRepository

interface UploadedPuzzleMetadataRepository : MongoRepository<UploadedPuzzleMetadata, ObjectId>