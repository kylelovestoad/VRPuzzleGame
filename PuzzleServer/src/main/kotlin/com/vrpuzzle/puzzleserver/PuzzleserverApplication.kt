package com.vrpuzzle.puzzleserver

import org.slf4j.Logger
import org.slf4j.LoggerFactory
import org.springframework.boot.autoconfigure.SpringBootApplication
import org.springframework.boot.runApplication

@SpringBootApplication
class PuzzleserverApplication

fun main(args: Array<String>) {
	runApplication<PuzzleserverApplication>(*args)
}
