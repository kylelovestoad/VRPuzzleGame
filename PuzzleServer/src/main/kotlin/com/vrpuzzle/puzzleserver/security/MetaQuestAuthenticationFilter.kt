package com.vrpuzzle.puzzleserver.security

import com.vrpuzzle.puzzleserver.services.MetaQuestAuthService
import jakarta.servlet.FilterChain
import jakarta.servlet.http.HttpServletRequest
import jakarta.servlet.http.HttpServletResponse
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken
import org.springframework.security.core.context.SecurityContextHolder
import org.springframework.stereotype.Component
import org.springframework.web.filter.OncePerRequestFilter

@Component
class MetaQuestAuthenticationFilter(
    private val metaQuestAuthService: MetaQuestAuthService
) : OncePerRequestFilter() {

    override fun doFilterInternal(
        request: HttpServletRequest,
        response: HttpServletResponse,
        filterChain: FilterChain
    ) {
        val userId = request.getHeader("Puzzle-Meta-User-Id")
        val nonce  = request.getHeader("Puzzle-Meta-Nonce")

        logger.info("Request: ${request.method} ${request.requestURI} | userId=$userId | nonce=${nonce?.take(10)}")

        if (userId != null && nonce != null) {
            try {

                val orgScopedId = metaQuestAuthService.verify(userId, nonce)

                val principal = MetaQuestAuthenticationPrincipal(userId, orgScopedId)
                val auth = MetaQuestAuthenticationToken(principal, emptyList())
                SecurityContextHolder.getContext().authentication = auth
            } catch (ex: InvalidMetaQuestTokenException) {
                response.sendError(HttpServletResponse.SC_UNAUTHORIZED, ex.message)
                return
            }
        }

        filterChain.doFilter(request, response)
    }
}