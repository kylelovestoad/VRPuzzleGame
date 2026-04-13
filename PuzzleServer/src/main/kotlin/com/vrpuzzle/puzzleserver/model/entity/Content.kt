package com.vrpuzzle.puzzleserver.model.entity

import com.vrpuzzle.puzzleserver.model.dto.ContentDTO
import org.bson.types.ObjectId
import org.springframework.data.annotation.Id
import org.springframework.data.mongodb.core.mapping.Document

@Document("content")
class Content (
    @Id
    val id: ObjectId = ObjectId.get(),
    val filename: String,
    val fileSize: Long,
    val contentType: String
) {
    fun toDTO(): ContentDTO {
        return ContentDTO (
            id = id.toHexString(),
            filename = filename,
            contentType = contentType,
            fileSize = fileSize,
            downloadUrl = "http://localhost:8080/api/content/${id}"
        )
    }
}