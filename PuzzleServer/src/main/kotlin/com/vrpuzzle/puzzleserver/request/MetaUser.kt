package com.vrpuzzle.puzzleserver.request

import com.fasterxml.jackson.annotation.JsonProperty

data class MetaUser(
    @JsonProperty("org_scoped_id") val orgScopedId: String,
    @JsonProperty("display_name") val displayName: String,
)