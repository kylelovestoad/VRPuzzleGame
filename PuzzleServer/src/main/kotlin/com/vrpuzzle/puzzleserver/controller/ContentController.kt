package com.vrpuzzle.puzzleserver.controller

import com.vrpuzzle.puzzleserver.services.ContentService
import org.bson.types.ObjectId
import org.springframework.http.MediaType
import org.springframework.http.ResponseEntity
import org.springframework.web.bind.annotation.GetMapping
import org.springframework.web.bind.annotation.PathVariable
import org.springframework.web.bind.annotation.RequestMapping
import org.springframework.web.bind.annotation.RestController

@RestController
@RequestMapping("/api/content")
class ContentController(
    private val contentService: ContentService,
) {
    @GetMapping("/{fileId}")
    fun download(@PathVariable fileId: ObjectId): ResponseEntity<ByteArray> {
        val contentDTO = contentService.getContent(fileId)
        val contentBytes = contentService.download(contentDTO)
        val contentType = MediaType.parseMediaType(contentDTO.contentType)
        return ResponseEntity.ok()
            .contentType(contentType)
            .body(contentBytes)
    }
}