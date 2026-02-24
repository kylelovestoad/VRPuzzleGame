using System;
using System.Collections.Generic;
using System.Linq;
using Persistence;
using PuzzleGeneration;
using UnityEngine;

public class Puzzle
{
    public long LocalID { get; }
    public long OnlineID { get; }
    public string Name { get; }
    public string Description { get; }
    public string Author { get; }
    
    public PuzzleLayout Layout { get; }
    public long SolvedPieces { get; private set; }
    
    public long TotalPieces => _chunks.Sum(chunk => chunk.PieceCount);
    public bool IsLocalOnly => OnlineID == -1;
    public double PercentComplete => (double) SolvedPieces / TotalPieces;
    
    private List<Chunk> _chunks;
    public PuzzleRenderData RenderData { get; }

    public Puzzle(PuzzleRenderData r)
    {
        _chunks = new List<Chunk>();
        RenderData = r;
    }
    
    public Puzzle(PuzzleSaveData saveData, PuzzleRenderData renderData)
    {
        LocalID = saveData.localID;
        OnlineID = saveData.onlineID;
        Name = saveData.name;
        Description = saveData.description;
        Author = saveData.author;
        Layout = saveData.layout;
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
        Layout = puzzleLayout;
        RenderData = renderData;
        _chunks = new List<Chunk>();
    }

    public void InitializeChunks()
    {
        foreach (var cut in Layout.initialPieceCuts)
        {
            Debug.Log(cut.solutionLocation);

            // TODO: randomize placement
            _chunks.Add(ChunkFactory.Instance.CreateSinglePieceChunk(
                cut.solutionLocation +
                new Vector3(cut.solutionLocation.x * 1.5f, cut.solutionLocation.y * 1.5f, 0),
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
            onlineID: OnlineID,
            name: Name,
            description: Description,
            author: Author,
            layout: Layout,
            chunks: _chunks.Select(c => c.ToData()).ToList()
        );
    }
}