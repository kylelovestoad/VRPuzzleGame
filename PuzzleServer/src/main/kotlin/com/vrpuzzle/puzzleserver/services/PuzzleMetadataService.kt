package com.vrpuzzle.puzzleserver.services

import com.vrpuzzle.puzzleserver.model.entity.PuzzleMetadata
import com.vrpuzzle.puzzleserver.model.dto.PuzzleMetadataDTO
import com.vrpuzzle.puzzleserver.request.CreatePuzzleRequest
import com.vrpuzzle.puzzleserver.request.UpdatePuzzleMetadataRequest
import com.vrpuzzle.puzzleserver.repository.PuzzleMetadataRepository
import com.vrpuzzle.puzzleserver.security.MetaQuestAuthenticationPrincipal
import org.bson.types.ObjectId
import org.springframework.stereotype.Service
import org.springframework.web.multipart.MultipartFile

@Service
class PuzzleMetadataService(
    private val puzzleMetadataRepository: PuzzleMetadataRepository,
    private val contentService: ContentService
) {
    internal fun createMetadata(metadata: PuzzleMetadata): PuzzleMetadataDTO =
        puzzleMetadataRepository.save(metadata).toDTO()

    fun createPuzzle(
        metadata: CreatePuzzleRequest,
        image: MultipartFile, principal:
        MetaQuestAuthenticationPrincipal
    ): PuzzleMetadataDTO {

        val content = contentService.uploadContent(image)

        val puzzleMetadata = PuzzleMetadata(
            name = metadata.name,
            author = principal.metaUser.displayName,
            layout = metadata.layout,
            content = content,
        )

        return createMetadata(puzzleMetadata)
    }

    internal fun getPuzzleById(id: ObjectId): PuzzleMetadata =
        puzzleMetadataRepository.findById(id)
            .orElseThrow { NoSuchElementException("Puzzle not found: $id") }

    fun getPuzzle(id: ObjectId): PuzzleMetadataDTO = getPuzzleById(id).toDTO()

    fun getAllPuzzles(): List<PuzzleMetadataDTO> =
        puzzleMetadataRepository.findAll().map { it.toDTO() }

    fun updatePuzzle(
        id: ObjectId,
        metadata: UpdatePuzzleMetadataRequest,
        image: MultipartFile?,
        principal: MetaQuestAuthenticationPrincipal
    ): PuzzleMetadataDTO {
        val existing = getPuzzleById(id)

        if (existing.author != principal.userId) {
            throw IllegalAccessException("User is not the author of the Puzzle")
        }

        val updatedContent = if (image != null) {
            contentService.uploadContent(image)
        } else {
            existing.content
        }

        val updated = existing.copy(
            name = metadata.name ?: existing.name,
            author = principal.metaUser.displayName,
            layout = metadata.layout ?: existing.layout,
            content = updatedContent,
        )


        return puzzleMetadataRepository.save(updated).toDTO()
    }

    fun deletePuzzle(id: ObjectId, principal: MetaQuestAuthenticationPrincipal) {
        val existing = getPuzzleById(id)
        if (existing.author != principal.userId) {
            throw IllegalAccessException("User is not the author of the Puzzle")
        }

        puzzleMetadataRepository.deleteById(id)
        contentService.deleteContent(existing.content.id)
    }
}