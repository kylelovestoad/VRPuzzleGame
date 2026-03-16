package com.vrpuzzle.puzzleserver.model.type

data class PieceSaveData(
    val pieceIndex: Int,
    val position: Vector3,
    val rotation: Quaternion
)