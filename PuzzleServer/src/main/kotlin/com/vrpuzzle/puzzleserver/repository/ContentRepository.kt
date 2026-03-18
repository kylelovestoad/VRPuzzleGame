package com.vrpuzzle.puzzleserver.repository

import com.vrpuzzle.puzzleserver.model.entity.Content
import org.bson.types.ObjectId
import org.springframework.data.mongodb.repository.MongoRepository

interface ContentRepository : MongoRepository<Content, ObjectId>