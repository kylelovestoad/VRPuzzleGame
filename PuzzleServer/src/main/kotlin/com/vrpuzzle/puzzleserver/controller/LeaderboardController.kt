package com.vrpuzzle.puzzleserver.controller

import com.vrpuzzle.puzzleserver.model.dto.LeaderboardEntryDTO
import com.vrpuzzle.puzzleserver.request.UpsertLeaderboardEntryRequest
import com.vrpuzzle.puzzleserver.security.MetaQuestAuthenticationPrincipal
import com.vrpuzzle.puzzleserver.services.LeaderboardEntryService
import org.bson.types.ObjectId
import org.springframework.http.ResponseEntity
import org.springframework.security.core.annotation.AuthenticationPrincipal
import org.springframework.web.bind.annotation.GetMapping
import org.springframework.web.bind.annotation.PathVariable
import org.springframework.web.bind.annotation.PostMapping
import org.springframework.web.bind.annotation.PutMapping
import org.springframework.web.bind.annotation.RequestBody
import org.springframework.web.bind.annotation.RequestMapping
import org.springframework.web.bind.annotation.RestController

@RestController
@RequestMapping("/api/puzzles/{puzzleId}/leaderboards")
class LeaderboardController(
    private val leaderboardEntryService: LeaderboardEntryService
) {


    @PostMapping
    fun upsertLeaderboardEntry(
        @RequestBody req: UpsertLeaderboardEntryRequest,
        @PathVariable puzzleId: ObjectId,
        @AuthenticationPrincipal principal: MetaQuestAuthenticationPrincipal
    ): ResponseEntity<LeaderboardEntryDTO> {
        val entry = leaderboardEntryService.upsertLeaderboardEntry(req.time, puzzleId, principal)
        return ResponseEntity.ok(entry)
    }

    @GetMapping
    fun getLeaderboardEntries(
        @PathVariable puzzleId: ObjectId,
    ): ResponseEntity<List<LeaderboardEntryDTO>> {
        val entries = leaderboardEntryService.getAllLeaderboardEntriesForPuzzle(puzzleId)
        return ResponseEntity.ok(entries)
    }
}