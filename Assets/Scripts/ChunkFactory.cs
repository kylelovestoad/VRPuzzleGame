using PuzzleGeneration;
using UnityEngine;

public class ChunkFactory : MonoBehaviour
{
    [SerializeField]
    private Chunk chunkPrefab;
    
    public Chunk CreateSinglePieceChunk(
        Vector3 initialPosition,
        Quaternion initialRotation,
        PieceCut pieceCut,
        PuzzleRenderData puzzleRenderData
    ) {
        Chunk chunk = Instantiate(chunkPrefab, initialPosition, initialRotation);
        chunk.InitializeSinglePieceChunk(pieceCut, puzzleRenderData);
        
        return chunk;
    }
}
