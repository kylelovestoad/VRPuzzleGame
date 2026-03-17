package com.vrpuzzle.puzzleserver.components

import org.bson.types.ObjectId
import org.springframework.core.convert.converter.Converter
import org.springframework.stereotype.Component

@Component
class ObjectIDConverter : Converter<String, ObjectId> {
    override fun convert(source: String): ObjectId = ObjectId(source)
}