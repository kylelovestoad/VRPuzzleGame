using System.Collections.Generic;
using UnityEngine;

public class Puzzle: MonoBehaviour
{
    [SerializeField]
    private ChunkFactory chunkFactory;
    private List<Chunk> chunks;

    public void InitializeChunks(List<Piece> pieces)
    {
        chunks = new List<Chunk>();

        foreach (Piece piece in pieces)
        {
            chunks.Add(chunkFactory.CreateChunk(piece));
        }
    }
}
