package com.vrpuzzle.puzzleserver.security

import com.vrpuzzle.puzzleserver.request.MetaUser

data class MetaQuestAuthenticationPrincipal(
    val userId: String,
    val metaUser: MetaUser,
)