using System;
using System.Collections.Generic;
using System.Linq;
using Persistence;
using PuzzleGeneration;
using UnityEngine;

public class Puzzle
{
    public string LocalID { get; }
    public string OnlineID { get; }
    public string Name { get; }
    public string Description { get; }
    public string Author { get; }
    
    public PuzzleLayout Layout { get; }
    public long SolvedPieces { get; private set; }
    
    public long TotalPieces => _chunks.Sum(chunk => chunk.PieceCount);
    public bool IsOnline => OnlineID != null;
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
        
        InitializeChunks(saveData);
    }
    
    private void InitializeChunks(PuzzleSaveData saveData)
    {
        if (saveData.chunks == null || saveData.chunks.Count == 0)
        {
            InitializeChunks();
        }
        else
        {
            InitializeSavedChunkStates(saveData.chunks);
        }
    }

    private void InitializeChunks()
    {
        foreach (var cut in Layout.initialPieceCuts)
        {
            Debug.Log(cut.solutionLocation);

            // TODO: randomize placement
            var offset = new Vector3(cut.solutionLocation.x * 1.5f, cut.solutionLocation.y * 1.5f, 0);
            
            _chunks.Add(ChunkFactory.Instance.CreateSinglePieceChunk(
                cut.solutionLocation + offset,
                Quaternion.identity,
                cut,
                this
            ));
        }
    }
    
    private void InitializeSavedChunkStates(List<ChunkSaveData> chunks)
    {
        foreach (var chunkSaveData in chunks)
        {
            var chunk = ChunkFactory.Instance.CreateMultiplePieceChunk(
                chunkSaveData,
                this
            );
            
            _chunks.Add(chunk);
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