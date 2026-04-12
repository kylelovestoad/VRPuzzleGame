package com.vrpuzzle.puzzleserver.model.dto

import org.bson.types.ObjectId

data class LeaderboardEntryDTO (
    var id: String,
    val puzzleId: String,
    val userId: String,
    val time: Float,
)