using System.Collections.Generic;
using UnityEngine;

public class Puzzle: MonoBehaviour
{
    [SerializeField]
    private ChunkFactory chunkFactory;
    private List<Chunk> chunks;

    public void InitializeChunks(List<Chunk> pieces)
    {
        chunks = new List<Chunk>();

        foreach (Chunk chunk in pieces)
        {
            chunks.Add(chunk);
        }
    }
}
