package com.vrpuzzle.puzzleserver.services

import com.vrpuzzle.puzzleserver.request.MetaUser
import com.vrpuzzle.puzzleserver.security.InvalidMetaQuestTokenException
import org.slf4j.Logger
import org.slf4j.LoggerFactory
import org.springframework.stereotype.Service
import org.springframework.web.client.RestClient
import org.springframework.web.client.body

@Service
class MetaQuestAuthService(
    private val restClient: RestClient,
) {
    val logger: Logger = LoggerFactory.getLogger(MetaQuestAuthService::class.java)

    fun verify(userId: String, userAccessToken: String): MetaUser {
        logger.info("Verifying user: $userId")
        return fetchMetaUser(userId, userAccessToken)
    }

    private fun fetchMetaUser(userId: String, userAccessToken: String): MetaUser {
        logger.info("Fetching org scoped id for user: $userId")
        val user = restClient.get()
            .uri { uriBuilder ->
                uriBuilder
                    .scheme("https")
                    .host("graph.oculus.com")
                    .path("/$userId")
                    .queryParam("fields", "org_scoped_id,display_name")
                    .queryParam("access_token", userAccessToken)
                    .build()
            }
            .retrieve()
            .body<MetaUser>()

        if (user != null)
            logger.info("Got org scoped id: $user")
        else {
            throw InvalidMetaQuestTokenException("Failed to get org scoped id for user: $userId")
        }

        return user
    }
}