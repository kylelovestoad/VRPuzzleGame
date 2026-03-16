package com.vrpuzzle.puzzleserver.model.type

data class ChunkSaveData(
    val position: Vector3,
    val rotation: Quaternion,
    val pieces: List<PieceSaveData>
)