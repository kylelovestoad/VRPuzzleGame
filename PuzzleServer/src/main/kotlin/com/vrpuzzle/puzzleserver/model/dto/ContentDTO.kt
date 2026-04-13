package com.vrpuzzle.puzzleserver.model.dto

import org.bson.types.ObjectId

data class ContentDTO (
    var id: String,
    var filename: String,
    var contentType: String,
    var fileSize: Long,
    var downloadUrl: String
)