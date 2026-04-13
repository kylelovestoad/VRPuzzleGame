package com.vrpuzzle.puzzleserver.model.type

data class PuzzleLayout(
    val rows: Int,
    val cols: Int,
    val width: Float,
    val height: Float,
    val shape: Int,
    val initialPieceCuts: List<PieceCut>,
)