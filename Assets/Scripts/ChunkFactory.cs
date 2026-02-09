using UnityEngine;

public class ChunkFactory : MonoBehaviour
{
    [SerializeField]
    private Chunk chunkPrefab;
    
    public Chunk CreateChunk(
        Vector3 initialPosition,
        Quaternion initialRotation,
        Vector3 solutionLocation,
        Mesh mesh,
        Material material
    ) {
        Chunk chunk = Instantiate(chunkPrefab, initialPosition, initialRotation);
        chunk.InitializeVariant(solutionLocation,  mesh, material);
        
        return chunk;
    }
}
