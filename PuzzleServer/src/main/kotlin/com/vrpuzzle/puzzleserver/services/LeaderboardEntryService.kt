package com.vrpuzzle.puzzleserver.services

import com.vrpuzzle.puzzleserver.model.dto.LeaderboardEntryDTO
import com.vrpuzzle.puzzleserver.model.entity.LeaderboardEntry
import com.vrpuzzle.puzzleserver.repository.LeaderboardEntryRepository
import com.vrpuzzle.puzzleserver.security.MetaQuestAuthenticationPrincipal
import org.bson.types.ObjectId
import org.slf4j.Logger
import org.slf4j.LoggerFactory
import org.springframework.stereotype.Service

@Service
class LeaderboardEntryService(
    private val leaderboardEntryRepository: LeaderboardEntryRepository,
) {

    val logger: Logger = LoggerFactory.getLogger(LeaderboardEntryService::class.java)

    fun upsertLeaderboardEntry(
        time: Float,
        puzzleId: ObjectId,
        principal: MetaQuestAuthenticationPrincipal
    ): LeaderboardEntryDTO {
        val existing = leaderboardEntryRepository.findByUserIdAndPuzzleId(principal.userId, puzzleId)

        return if (existing == null) {
            leaderboardEntryRepository.save(
                LeaderboardEntry(
                    puzzleId = puzzleId,
                    userId = principal.userId,
                    username = principal.metaUser.displayName,
                    time = time
                )
            ).toDTO()
        } else if (time < existing.time) {
            // Only ever updated when the new time is better. This check should be both on serverside and client
            leaderboardEntryRepository.save(existing.copy(time = time)).toDTO()
        } else {
            existing.toDTO()
        }
    }

    fun deleteAllLeaderboardEntriesForPuzzle(puzzleId: ObjectId) {
        leaderboardEntryRepository.deleteAllByPuzzleId(puzzleId)
    }

    fun getAllLeaderboardEntriesForPuzzle(puzzleId: ObjectId): List<LeaderboardEntryDTO> {
        return leaderboardEntryRepository.findAllByPuzzleIdOrderByTimeAsc((puzzleId))
            .map { it.toDTO() }
    }
}