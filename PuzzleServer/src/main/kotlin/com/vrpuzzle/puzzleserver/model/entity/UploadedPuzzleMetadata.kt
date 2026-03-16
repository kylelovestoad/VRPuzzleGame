// PuzzleSaveData.kt
import com.vrpuzzle.puzzleserver.model.dto.UploadedPuzzleMetadataDTO
import com.vrpuzzle.puzzleserver.model.type.PuzzleLayout
import org.bson.types.ObjectId
import org.springframework.data.annotation.Id
import org.springframework.data.mongodb.core.index.CompoundIndex
import org.springframework.data.mongodb.core.mapping.Document

@Document(collection = "uploaded_puzzle_metadata")
@CompoundIndex(def = "{'author': 1, 'name': 1}")
data class UploadedPuzzleMetadata(
    @Id
    val onlineID: ObjectId = ObjectId.get(),
    val name: String,
    val author: String,
    val layout: PuzzleLayout,
    val imageUrl: String?
) {
    fun toDTO() = UploadedPuzzleMetadataDTO(
        onlineID = onlineID.toHexString(),
        name = name,
        author = author,
        layout = layout,
        imageUrl = imageUrl
    )
}