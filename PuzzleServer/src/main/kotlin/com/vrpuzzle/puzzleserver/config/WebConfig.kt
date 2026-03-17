package com.vrpuzzle.puzzleserver.config

import com.vrpuzzle.puzzleserver.components.ObjectIDConverter
import org.springframework.context.annotation.Configuration
import org.springframework.format.FormatterRegistry
import org.springframework.web.servlet.config.annotation.WebMvcConfigurer

@Configuration
class WebConfig(
    private val objectIDConverter: ObjectIDConverter
) : WebMvcConfigurer {
    override fun addFormatters(registry: FormatterRegistry) {
        registry.addConverter(objectIDConverter)
    }
}