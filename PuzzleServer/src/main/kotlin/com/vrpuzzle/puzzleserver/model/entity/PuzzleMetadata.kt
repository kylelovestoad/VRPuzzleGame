package com.vrpuzzle.puzzleserver.model.entity

import com.vrpuzzle.puzzleserver.model.dto.PuzzleMetadataDTO
import com.vrpuzzle.puzzleserver.model.type.PuzzleLayout
import org.bson.types.ObjectId
import org.springframework.data.annotation.Id
import org.springframework.data.mongodb.core.index.CompoundIndex
import org.springframework.data.mongodb.core.mapping.Document

@Document(collection = "puzzle_metadata")
@CompoundIndex(def = "{'author': 1, 'name': 1}")
data class PuzzleMetadata(
    @Id
    val onlineID: ObjectId = ObjectId.get(),
    val name: String,
    val author: String,
    val layout: PuzzleLayout,
    val content: Content,
) {
    fun toDTO() = PuzzleMetadataDTO(
        onlineID = onlineID,
        name = name,
        author = author,
        layout = layout,
        content = content.toDTO(),
    )
}