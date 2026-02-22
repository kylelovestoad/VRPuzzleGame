using System;
using System.Collections.Generic;
using System.Linq;
using Persistence;
using UnityEngine;

public class Puzzle
{
    public long LocalID { get; }
    public long? OnlineID { get; }
    public string Name { get; }
    public string Description { get; }
    public string Author { get; }
    public long Seed { get; }
    public PieceShape Shape { get; }
    public long SolvedPieces { get; private set; }
    
    public long TotalPieces => chunks.Sum(chunk => chunk.PieceCount);
    public bool IsLocalOnly => OnlineID == null;
    public double PercentComplete => (double) SolvedPieces / TotalPieces;
    
    private List<Chunk> chunks;
    
    public Puzzle(PuzzleSaveData saveData)
    {
        LocalID = saveData.localID;
        OnlineID = saveData.OnlineID;
        Name = saveData.name;
        Description = saveData.description;
        Author = saveData.author;
        Seed = saveData.seed;
        Shape = saveData.shape;
    }
    
    public PuzzleSaveData ToData()
    {
        return new PuzzleSaveData(
            localID: LocalID,
            onlineID: OnlineID,
            name: Name,
            description: Description,
            author: Author,
            seed: Seed,
            shape: Shape,
            chunks: chunks.Select(c => c.ToData()).ToList()
        );
    }
}