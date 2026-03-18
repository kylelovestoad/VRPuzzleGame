package com.vrpuzzle.puzzleserver.request

import com.fasterxml.jackson.annotation.JsonProperty

data class NonceValidationResponse(
    @JsonProperty("is_valid") val isValid: Boolean,
)