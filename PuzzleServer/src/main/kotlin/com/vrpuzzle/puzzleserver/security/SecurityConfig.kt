package com.vrpuzzle.puzzleserver.security

import org.springframework.context.annotation.Bean
import org.springframework.context.annotation.Configuration
import org.springframework.security.config.Customizer
import org.springframework.security.config.annotation.method.configuration.EnableMethodSecurity
import org.springframework.security.config.annotation.web.builders.HttpSecurity
import org.springframework.security.config.annotation.web.configuration.EnableWebSecurity
import org.springframework.security.web.SecurityFilterChain
import org.springframework.security.web.authentication.UsernamePasswordAuthenticationFilter
import org.springframework.web.cors.CorsConfiguration
import org.springframework.web.cors.CorsConfigurationSource
import org.springframework.web.cors.UrlBasedCorsConfigurationSource


@Configuration
@EnableWebSecurity
@EnableMethodSecurity(prePostEnabled = true)
class SecurityConfig(
    private val metaQuestAuthFilter: MetaQuestAuthenticationFilter
) {

//    @Bean
//    fun bCryptPasswordEncoder(): BCryptPasswordEncoder {
//        return BCryptPasswordEncoder()
//    }
//

    /**
     * Specifies security filtering, allowing or denying certain requests
     */
    @Bean
    fun securityFilterChain(httpSecurity: HttpSecurity): SecurityFilterChain {
        return httpSecurity
            .csrf{ it.disable() }
            .cors(Customizer.withDefaults())
            .authorizeHttpRequests { auth ->
                auth.anyRequest().authenticated()
            }
            .addFilterBefore(metaQuestAuthFilter, UsernamePasswordAuthenticationFilter::class.java)
            .build()
    }

//
//    @Bean
//    fun authenticationManager(config: AuthenticationConfiguration): AuthenticationManager {
//        return config.authenticationManager
//    }

//    /**
//     * Handles authentication. Checks password against hashes on login
//     */
//    @Bean
//    fun authenticationProvider(): AuthenticationProvider {
//        val provider = DaoAuthenticationProvider(userDetailsService)
//        provider.setPasswordEncoder(bCryptPasswordEncoder())
//        return provider
//    }
//
//    @Bean
//    fun roleHierarchy(): RoleHierarchy {
//        return RoleHierarchyImpl.withDefaultRolePrefix()
//            .role("ADMIN").implies("COACH")
//            .role("COACH").implies("GUARDIAN")
//            .role("GUARDIAN").implies("PLAYER")
//            .build()
//    }

    @Bean
    fun corsConfigurationSource(): CorsConfigurationSource {
        val configuration = CorsConfiguration()
        configuration.allowedOrigins = listOf("http://localhost:5173")
        configuration.allowedMethods = listOf("GET", "POST", "PUT", "DELETE", "PATCH")
        configuration.allowCredentials = true
        configuration.allowedHeaders = listOf("*")
        val source = UrlBasedCorsConfigurationSource()
        source.registerCorsConfiguration("/**", configuration)
        return source
    }
}