using System;
using Persistence;
using PuzzleGeneration;
using UnityEngine;

public class ChunkFactory : MonoBehaviour
{
    public static ChunkFactory Instance { get; private set; }
    
    [SerializeField]
    private Chunk chunkPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public Chunk CreateSinglePieceChunk(
        Vector3 initialPosition,
        Quaternion initialRotation,
        PieceCut pieceCut,
        Puzzle puzzle
    ) {
        Chunk chunk = Instantiate(chunkPrefab, initialPosition, initialRotation, puzzle.transform);
        
        chunk.InitializeSinglePieceChunk(pieceCut, puzzle);
        
        return chunk;
    }
    
    public Chunk CreateMultiplePieceChunk(
        ChunkSaveData chunkStateData,
        Puzzle puzzle
    ) {
        Chunk chunk = Instantiate(chunkPrefab, chunkStateData.position, chunkStateData.rotation, puzzle.transform);
        
        chunk.InitializeMultiplePieceChunk(chunkStateData, puzzle);
        
        return chunk;
    }
}
