//package com.vrpuzzle.puzzleserver.security
//
//import jakarta.servlet.FilterChain
//import jakarta.servlet.http.HttpServletRequest
//import jakarta.servlet.http.HttpServletResponse
//import org.springframework.security.authentication.UsernamePasswordAuthenticationToken
//import org.springframework.security.core.context.SecurityContextHolder
//import org.springframework.stereotype.Component
//import org.springframework.web.filter.OncePerRequestFilter
//
//@Component
//class MetaQuestAuthFilter : OncePerRequestFilter() {
//
//    override fun doFilterInternal(
//        request: HttpServletRequest,
//        response: HttpServletResponse,
//        filterChain: FilterChain
//    ) {
//
//        if (metaQuestId != null) {
//            val auth = UsernamePasswordAuthenticationToken(
//                metaQuestId,
//                null,
//                emptyList()
//            )
//            SecurityContextHolder.getContext().authentication = auth
//        }
//
//        filterChain.doFilter(request, response)
//    }
//}