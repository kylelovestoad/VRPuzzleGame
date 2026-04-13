package com.vrpuzzle.puzzleserver.services

import com.vrpuzzle.puzzleserver.model.dto.ContentDTO
import com.vrpuzzle.puzzleserver.model.entity.Content
import com.vrpuzzle.puzzleserver.repository.ContentRepository
import org.bson.types.ObjectId
import org.springframework.beans.factory.annotation.Value
import org.springframework.stereotype.Service
import org.springframework.web.multipart.MultipartFile
import java.nio.file.Files
import java.nio.file.Paths
import java.nio.file.StandardCopyOption
import kotlin.io.path.div

@Service
class ContentService(

    @param:Value($$"${vrpuzzle.storage.path}")
    private val storagePath: String,
    private val contentRepository: ContentRepository,
) {
    fun upload(file: MultipartFile): ContentDTO =
        uploadContent(file).toDTO()

    fun getContent(id: ObjectId): ContentDTO =
        getContentById(id).toDTO()

    fun download(content: ContentDTO): ByteArray {
        val downloadPath = Paths.get(storagePath).resolve(content.id)
        val fileBytes = Files.readAllBytes(downloadPath)

        return fileBytes
    }


    fun deleteContent(id: ObjectId) {
        val existing = getContentById(id)
        contentRepository.deleteById(id)

        val contentPath = Paths.get(storagePath) / existing.id.toHexString()

        Files.deleteIfExists(contentPath)
    }

    private fun createContent(content: Content): Content =
        contentRepository.save(content)

    internal fun updateContent(content: Content): ContentDTO =
        contentRepository.save(content).toDTO()

    internal fun getContentById(id: ObjectId): Content =
        contentRepository.findById(id).orElseThrow()

    internal fun uploadContent(file: MultipartFile): Content {
        if (file.isEmpty) {
            throw IllegalArgumentException("file can not be empty")
        }

        val uploadPath = Paths.get(storagePath)
        if (!Files.exists(uploadPath)) {
            Files.createDirectories(uploadPath)
        }

        val originalFilename = file.originalFilename?.takeIf { it.isNotBlank() }
            ?: throw IllegalArgumentException("Filename can not be empty")

        val contentType = file.contentType?.takeIf { it.isNotBlank() }
            ?: throw IllegalArgumentException("Content type must be provided")

        val allowedMediaTypes = setOf(
            "image/png",
            "image/jpeg",
        )

        if (contentType !in allowedMediaTypes) {
            throw IllegalArgumentException("Unsupported media type '$contentType'. Allowed types: ${allowedMediaTypes.joinToString()}")
        }

        val content = createContent(
            Content(
                filename = originalFilename,
                contentType = contentType,
                fileSize = file.size,
            )
        )

        val targetPath = uploadPath.resolve(content.id.toHexString())
        Files.copy(file.inputStream, targetPath, StandardCopyOption.REPLACE_EXISTING)

        return content
    }
}