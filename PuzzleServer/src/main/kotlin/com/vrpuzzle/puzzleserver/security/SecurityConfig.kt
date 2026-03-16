package com.jknv.lum.security

import org.springframework.context.annotation.Bean
import org.springframework.context.annotation.Configuration
import org.springframework.http.HttpMethod
import org.springframework.security.authentication.AuthenticationManager
import org.springframework.security.authentication.AuthenticationProvider
import org.springframework.security.authentication.dao.DaoAuthenticationProvider
import org.springframework.security.config.Customizer
import org.springframework.security.config.annotation.method.configuration.EnableMethodSecurity
import org.springframework.security.config.annotation.authentication.configuration.AuthenticationConfiguration
import org.springframework.security.config.annotation.web.builders.HttpSecurity
import org.springframework.security.config.annotation.web.configuration.EnableWebSecurity
import org.springframework.security.config.http.SessionCreationPolicy
import org.springframework.security.core.userdetails.UserDetailsService
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder
import org.springframework.security.web.SecurityFilterChain
import org.springframework.security.web.authentication.UsernamePasswordAuthenticationFilter
import org.springframework.web.cors.CorsConfiguration
import org.springframework.web.cors.CorsConfigurationSource
import org.springframework.web.cors.UrlBasedCorsConfigurationSource
import org.springframework.security.access.hierarchicalroles.RoleHierarchyImpl
import org.springframework.security.access.hierarchicalroles.RoleHierarchy


@Configuration
@EnableWebSecurity
@EnableMethodSecurity(prePostEnabled = true)
class SecurityConfig(
//    val userDetailsService: UserDetailsService,
) {

//    @Bean
//    fun bCryptPasswordEncoder(): BCryptPasswordEncoder {
//        return BCryptPasswordEncoder()
//    }
//
//    /**
//     * Specifies security filtering, allowing or denying certain requests
//     */
//    @Bean
//    fun securityFilterChain(httpSecurity: HttpSecurity, jwtTokenFilter: JwtTokenFilter): SecurityFilterChain {
//        return httpSecurity
//            .csrf{
//                    csrf -> csrf.disable() // React handles this for us
//            }
//            .cors(Customizer.withDefaults())
//            .authorizeHttpRequests { auth ->
//                auth
//                    .requestMatchers("/ws/**").permitAll()
//                    .requestMatchers( HttpMethod.POST,"/api/accounts").permitAll()
//                    .requestMatchers(HttpMethod.POST,"/api/accounts/login").permitAll() // Sign up page should be available
//                    .requestMatchers(HttpMethod.POST,"/api/accounts/logout").permitAll()
//                    .anyRequest().authenticated() // Otherwise everything else needs authentication
//            }
//            .sessionManagement { session -> session.sessionCreationPolicy(SessionCreationPolicy.STATELESS) }
//            .addFilterBefore(jwtTokenFilter, UsernamePasswordAuthenticationFilter::class.java)
//            .build()
//    }
//
//    @Bean
//    fun authenticationManager(config: AuthenticationConfiguration): AuthenticationManager {
//        return config.authenticationManager
//    }
//
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

//    @Bean
//    fun corsConfigurationSource(): CorsConfigurationSource {
//        val configuration = CorsConfiguration()
//        configuration.allowedOrigins = listOf("http://localhost:5173")
//        configuration.allowedMethods = listOf("GET", "POST", "PUT", "DELETE", "PATCH")
//        configuration.allowCredentials = true
//        configuration.allowedHeaders = listOf("*")
//        val source = UrlBasedCorsConfigurationSource()
//        source.registerCorsConfiguration("/**", configuration)
//        return source
//    }
}