package com.vrpuzzle.puzzleserver.services

import com.vrpuzzle.puzzleserver.model.dto.PuzzleSaveDataDTO
import com.vrpuzzle.puzzleserver.model.entity.PuzzleSaveData
import com.vrpuzzle.puzzleserver.repository.PuzzleSaveDataRepository
import com.vrpuzzle.puzzleserver.request.UpdatePuzzleSaveDataRequest
import org.bson.types.ObjectId
import org.springframework.stereotype.Service

@Service
class PuzzleSaveDataService(private val puzzleSaveDataRepository: PuzzleSaveDataRepository) {
    fun upsertPuzzleSaveDataFromRequest(
        puzzleId: ObjectId,
        userId: String,
        req: UpdatePuzzleSaveDataRequest
    ): PuzzleSaveDataDTO {
        val existingId = puzzleSaveDataRepository
            .findPuzzleSaveDataByPuzzleIdAndMetaUserId(puzzleId, userId)?.id

        val saveData = PuzzleSaveData(
            id = existingId ?: ObjectId.get(),
            puzzleId = puzzleId,
            metaUserId = userId,
            chunks = req.chunks,
            elapsedTime = req.elapsedTime,
        )

        return puzzleSaveDataRepository.save(saveData).toDTO()
    }
}