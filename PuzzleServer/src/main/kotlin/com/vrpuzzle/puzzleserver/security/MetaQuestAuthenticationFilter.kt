package com.vrpuzzle.puzzleserver.security

import com.vrpuzzle.puzzleserver.services.MetaQuestAuthService
import jakarta.servlet.FilterChain
import jakarta.servlet.http.HttpServletRequest
import jakarta.servlet.http.HttpServletResponse
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
        val userAccessToken  = request.getHeader("Puzzle-Meta-User-Access-Token")

        logger.info("Request: ${request.method} ${request.requestURI} | userId=$userId | userAccessToken=${userAccessToken?.take(10)}")

        if (userId != null && userAccessToken != null) {
            try {

                val metaUser = metaQuestAuthService.verify(userId, userAccessToken)

                val principal = MetaQuestAuthenticationPrincipal(userId, metaUser)
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