using System.Collections.Generic;
using UnityEngine;

public class Puzzle
{
    List<Chunk> chunks;

    public Puzzle(List<Piece> pieces)
    {
        chunks = new List<Chunk>();

        foreach (Piece piece in pieces)
        {
            chunks.Add(Chunk.CreateSinglePieceChunk(piece));
        }
    }
    
    
}
