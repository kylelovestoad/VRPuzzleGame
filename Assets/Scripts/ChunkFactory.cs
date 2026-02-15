using UnityEngine;

public class ChunkFactory : MonoBehaviour
{
    [SerializeField]
    private Chunk chunkPrefab;
    
    public Chunk CreateSinglePieceChunk(
        Vector3 initialPosition,
        Quaternion initialRotation,
        PieceInfo pieceInfo
    ) {
        Chunk chunk = Instantiate(chunkPrefab, initialPosition, initialRotation);
        chunk.InitializeSinglePieceChunk(pieceInfo);
        
        return chunk;
    }
}
