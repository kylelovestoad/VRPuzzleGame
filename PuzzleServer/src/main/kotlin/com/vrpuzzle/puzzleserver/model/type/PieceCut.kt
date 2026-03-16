package com.vrpuzzle.puzzleserver.model.type

data class PieceCut(
    val pieceIndex: Int,
    val neighborIndices: List<Int>,
    val solutionLocation: Vector3,
    val borderPoints: List<Vector2>
)