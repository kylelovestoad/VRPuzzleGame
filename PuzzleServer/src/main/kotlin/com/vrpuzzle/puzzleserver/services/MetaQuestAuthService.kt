package com.vrpuzzle.puzzleserver.services

import com.fasterxml.jackson.annotation.JsonProperty
import com.vrpuzzle.puzzleserver.request.NonceValidationResponse
import com.vrpuzzle.puzzleserver.request.OrgScopedIdResponse
import com.vrpuzzle.puzzleserver.security.InvalidMetaQuestTokenException
import org.springframework.beans.factory.annotation.Value
import org.springframework.http.MediaType
import org.springframework.stereotype.Service
import org.springframework.util.LinkedMultiValueMap
import org.springframework.web.client.RestClient
import org.springframework.web.client.body

@Service
class MetaQuestAuthService(
    @Value("\${meta.app-id}") private val appId: String,
    @Value("\${meta.app-secret}") private val appSecret: String,
    private val restClient: RestClient,
) {
    private val accessToken get() = "OC|$appId|$appSecret"

    fun verify(userId: String, nonce: String): String {
        val isValid = validateNonce(userId, nonce)
        if (!isValid) {
            throw InvalidMetaQuestTokenException("Nonce validation failed for user: $userId")
        }

        return fetchOrgScopedId(userId) ?: throw InvalidMetaQuestTokenException("Nonce validation failed for user: $userId")
    }

    fun validateNonce(userId: String, nonce: String): Boolean {
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
        val response = restClient.get()
            .uri("https://graph.oculus.com/$userId?fields=org_scoped_id&access_token=$accessToken")
            .retrieve()
            .body<OrgScopedIdResponse>()

        return response?.orgScopedId
    }
}



