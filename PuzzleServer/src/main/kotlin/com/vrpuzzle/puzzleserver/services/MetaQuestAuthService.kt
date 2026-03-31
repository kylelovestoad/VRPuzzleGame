package com.vrpuzzle.puzzleserver.services

import com.fasterxml.jackson.annotation.JsonProperty
import com.vrpuzzle.puzzleserver.PuzzleserverApplication
import com.vrpuzzle.puzzleserver.request.NonceValidationResponse
import com.vrpuzzle.puzzleserver.request.OrgScopedIdResponse
import com.vrpuzzle.puzzleserver.security.InvalidMetaQuestTokenException
import org.slf4j.Logger
import org.slf4j.LoggerFactory
import org.springframework.beans.factory.annotation.Value
import org.springframework.http.MediaType
import org.springframework.stereotype.Service
import org.springframework.util.LinkedMultiValueMap
import org.springframework.web.client.RestClient
import org.springframework.web.client.body
import kotlin.math.log

@Service
class MetaQuestAuthService(
    @Value("\${meta.app-id}") private val appId: String,
    @Value("\${meta.app-secret}") private val appSecret: String,
    private val restClient: RestClient,
) {

    val logger: Logger = LoggerFactory.getLogger(MetaQuestAuthService::class.java)


    private val accessToken get() = "OC|$appId|$appSecret"

    fun verify(userId: String, nonce: String): String {
        logger.info("Verifying for user: $userId")
        val isValid = validateNonce(userId, nonce)
        if (!isValid) {
            logger.warn("Nonce invalid")
            throw InvalidMetaQuestTokenException("Nonce validation failed for user: $userId")
        }

        // TODO replace with fetch scoped Id
        return userId
    }

    fun validateNonce(userId: String, nonce: String): Boolean {
        logger.info("Validating nonce $nonce")
        val res = restClient.post()
            .uri("https://graph.oculus.com/user_nonce_validate")
            .contentType(MediaType.APPLICATION_FORM_URLENCODED)
            .body(LinkedMultiValueMap<String, String>().apply {
                add("access_token", accessToken)
                add("nonce", nonce)
                add("user_id", userId)
            })
            .retrieve()
            .body<NonceValidationResponse>()

        return res?.isValid ?: false
    }

    fun fetchOrgScopedId(userId: String): String? {
        logger.info("Fetching org scoped id for user")
        val response = restClient.get()
            .uri { uriBuilder ->
                uriBuilder
                    .scheme("https")
                    .host("graph.oculus.com")
                    .path("/$userId")
                    .queryParam("fields", "org_scoped_id")
                    .queryParam("access_token", accessToken)
                    .build()
            }
            .retrieve()
            .body<OrgScopedIdResponse>()

        val orgScopedId = response?.orgScopedId

        if (orgScopedId != null) {
            logger.info("Got org scoped id: $orgScopedId")
        } else {
            logger.warn("Failed to get org scoped id")
        }
        return orgScopedId
    }
}



