package com.vrpuzzle.puzzleserver.model.type

data class PuzzleLayout(
    val width: Float,
    val height: Float,
    val shape: PieceShape,
    val initialPieceCuts: List<PieceCut>
)