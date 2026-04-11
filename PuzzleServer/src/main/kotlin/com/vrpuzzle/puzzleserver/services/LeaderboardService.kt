package com.vrpuzzle.puzzleserver.services

import com.vrpuzzle.puzzleserver.config.AppConfig
import org.bson.types.ObjectId
import org.slf4j.Logger
import org.slf4j.LoggerFactory
import org.springframework.beans.factory.annotation.Value
import org.springframework.http.MediaType
import org.springframework.util.LinkedMultiValueMap
import org.springframework.web.client.RestClient
import org.springframework.web.client.body

class LeaderboardService(
    private val restClient: RestClient,
    @Value("\${meta.app-id}") private val appId: String,
    @Value("\${meta.app-access-token}") private val appAccessToken: String
    private val puzzleMetadataService: PuzzleMetadataService
) {

    val logger: Logger = LoggerFactory.getLogger(LeaderboardService::class.java)

}