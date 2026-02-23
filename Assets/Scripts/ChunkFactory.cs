using System;
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
        Chunk chunk = Instantiate(chunkPrefab, initialPosition, initialRotation);
        chunk.InitializeSinglePieceChunk(pieceCut, puzzle);
        
        return chunk;
    }
}
