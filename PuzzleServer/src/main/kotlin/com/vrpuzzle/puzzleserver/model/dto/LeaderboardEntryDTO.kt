package com.vrpuzzle.puzzleserver.model.dto

data class LeaderboardEntryDTO (
    var id: String,
    val puzzleId: String,
    val userId: String,
    val username: String,
    val time: Float,
)