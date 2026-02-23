using System;
using System.Collections.Generic;
using System.Linq;
using Persistence;
using PuzzleGeneration;
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
    
    public long TotalPieces => _chunks.Sum(chunk => chunk.PieceCount);
    public bool IsLocalOnly => OnlineID == null;
    public double PercentComplete => (double) SolvedPieces / TotalPieces;
    
    private List<Chunk> _chunks;
    // private PuzzleLayout _puzzleLayout;
    public PuzzleRenderData RenderData { get; }

    public Puzzle(PuzzleRenderData r)
    {
        _chunks = new List<Chunk>();
        RenderData = r;
    }
    
    public Puzzle(PuzzleSaveData saveData, PuzzleRenderData renderData)
    {
        LocalID = saveData.localID;
        OnlineID = saveData.OnlineID;
        Name = saveData.name;
        Description = saveData.description;
        Author = saveData.author;
        Seed = saveData.seed;
        Shape = saveData.shape;
        RenderData = renderData;
        _chunks = new List<Chunk>();
    }

    public Puzzle(
        string name,
        string description,
        string author,
        PuzzleLayout puzzleLayout, 
        PuzzleRenderData renderData
    ) {
        Name = name;
        Description = description;
        Author = author;
        Shape = puzzleLayout.Shape;
        
        RenderData = renderData;
        _chunks = new List<Chunk>();
        
        foreach (var cut in puzzleLayout.PieceCuts)
        {
            Debug.Log(cut.SolutionLocation);

            // TODO: randomize placement
            _chunks.Add(ChunkFactory.Instance.CreateSinglePieceChunk(
                cut.SolutionLocation +
                new Vector3(cut.SolutionLocation.x * 1.5f, cut.SolutionLocation.y * 1.5f, 0),
                Quaternion.identity,
                cut,
                this
            ));
        }
    }

    public void RemoveChunk(Chunk chunk)
    {
        _chunks.Remove(chunk);
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
            chunks: _chunks.Select(c => c.ToData()).ToList()
        );
    }
}