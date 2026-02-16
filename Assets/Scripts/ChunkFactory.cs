using UnityEngine;

public class ChunkFactory : MonoBehaviour
{
    [SerializeField]
    private Chunk chunkPrefab;
    
    public Chunk CreateSinglePieceChunk(
        Vector3 initialPosition,
        Quaternion initialRotation,
        PieceRenderData pieceRenderData
    ) {
        Chunk chunk = Instantiate(chunkPrefab, initialPosition, initialRotation);
        chunk.InitializeSinglePieceChunk(pieceRenderData);
        
        return chunk;
    }
}
