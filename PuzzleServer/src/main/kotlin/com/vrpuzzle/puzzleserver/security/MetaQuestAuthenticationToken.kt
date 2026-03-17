package com.vrpuzzle.puzzleserver.security

import org.springframework.security.authentication.AbstractAuthenticationToken
import org.springframework.security.core.GrantedAuthority
import org.springframework.security.core.authority.AuthorityUtils

class MetaQuestAuthenticationToken(
    private val authPrincipal: MetaQuestAuthenticationPrincipal,
    authorities: Collection<GrantedAuthority> = AuthorityUtils.NO_AUTHORITIES
) : AbstractAuthenticationToken(authorities) {
    override fun isAuthenticated() = true
    override fun getPrincipal() = authPrincipal
    override fun getCredentials() = null
}