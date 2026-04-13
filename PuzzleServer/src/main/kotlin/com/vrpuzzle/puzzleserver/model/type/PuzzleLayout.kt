package com.vrpuzzle.puzzleserver.model.type

data class PuzzleLayout(
    val rows: Int,
    val cols: Int,
    val width: Float,
    val height: Float,
    val shape: PieceShape,
    val initialPieceCuts: List<PieceCut>,
)