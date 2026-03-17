package com.vrpuzzle.puzzleserver.security

data class MetaQuestAuthenticationPrincipal(
    val userId: String,
    val orgScopedId: String
)