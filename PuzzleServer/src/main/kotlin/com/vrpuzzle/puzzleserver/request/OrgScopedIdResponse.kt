package com.vrpuzzle.puzzleserver.request

import com.fasterxml.jackson.annotation.JsonProperty

data class OrgScopedIdResponse(
    @JsonProperty("org_scoped_id") val orgScopedId: String,
)