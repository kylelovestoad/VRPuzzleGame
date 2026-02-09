using UnityEngine;

public class ChunkFactory : MonoBehaviour
{
    [SerializeField] 
    private Chunk chunkPrefab;
    
    public Chunk CreateChunk(Piece piece) 
    {
        Chunk chunk = Instantiate(chunkPrefab, piece.transform.position, piece.transform.rotation);
        chunk.InitializeVariant(piece);
        
        return chunk;
    }
}
