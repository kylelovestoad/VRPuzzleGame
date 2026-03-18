package com.vrpuzzle.puzzleserver.controller

import com.vrpuzzle.puzzleserver.model.dto.PuzzleMetadataDTO
import com.vrpuzzle.puzzleserver.request.CreatePuzzleRequest
import com.vrpuzzle.puzzleserver.request.UpdatePuzzleRequest
import com.vrpuzzle.puzzleserver.security.MetaQuestAuthenticationPrincipal
import com.vrpuzzle.puzzleserver.services.ContentService
import com.vrpuzzle.puzzleserver.services.PuzzleMetadataService
import org.bson.types.ObjectId
import org.springframework.http.HttpStatus
import org.springframework.http.MediaType
import org.springframework.http.ResponseEntity
import org.springframework.security.core.annotation.AuthenticationPrincipal
import org.springframework.web.bind.annotation.DeleteMapping
import org.springframework.web.bind.annotation.GetMapping
import org.springframework.web.bind.annotation.PathVariable
import org.springframework.web.bind.annotation.PostMapping
import org.springframework.web.bind.annotation.PutMapping
import org.springframework.web.bind.annotation.RequestMapping
import org.springframework.web.bind.annotation.RequestPart
import org.springframework.web.bind.annotation.RestController
import org.springframework.web.multipart.MultipartFile

@RestController
@RequestMapping("/api/puzzles")
class PuzzleController(
    private val puzzleMetadataService: PuzzleMetadataService,
) {


    @PostMapping(consumes = [MediaType.MULTIPART_FORM_DATA_VALUE])
    fun createPuzzle(
        @RequestPart("metadata") metadata: CreatePuzzleRequest,
        @RequestPart("image") image: MultipartFile,
    ): ResponseEntity<PuzzleMetadataDTO> {
        val result = puzzleMetadataService.createPuzzle(metadata, image)
        return ResponseEntity.status(HttpStatus.CREATED).body(result)
    }

    @GetMapping("/{id}")
    fun readOnlinePuzzle(
        @PathVariable id: ObjectId
    ): ResponseEntity<PuzzleMetadataDTO> {
        val result = puzzleMetadataService.getPuzzle(id)
        return ResponseEntity.ok(result)
    }

    @GetMapping
    fun readAllOnlinePuzzles(): ResponseEntity<List<PuzzleMetadataDTO>> {
        val result = puzzleMetadataService.getAllPuzzles()
        return ResponseEntity.ok(result)
    }

    @PutMapping("/{id}", consumes = [MediaType.MULTIPART_FORM_DATA_VALUE])
    fun updateOnlinePuzzle(
        @PathVariable id: ObjectId,
        @RequestPart("metadata") metadata: UpdatePuzzleRequest,
        @RequestPart("image", required = false) image: MultipartFile?,
        @AuthenticationPrincipal principal: MetaQuestAuthenticationPrincipal,
    ): ResponseEntity<PuzzleMetadataDTO> {
        val result = puzzleMetadataService.updatePuzzle(id, metadata, image, principal)
        return ResponseEntity.ok(result)
    }

    @DeleteMapping("/{id}")
    fun deleteOnlinePuzzle(
        @PathVariable id: ObjectId,
        @AuthenticationPrincipal principal: MetaQuestAuthenticationPrincipal,
    ): ResponseEntity<Void> {
        puzzleMetadataService.deletePuzzle(id, principal)
        return ResponseEntity.noContent().build()
    }
}