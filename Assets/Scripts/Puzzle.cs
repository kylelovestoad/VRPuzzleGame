using System;
using System.Collections.Generic;
using System.Linq;
using Persistence;
using PuzzleGeneration;
using UnityEngine;

public class Puzzle: MonoBehaviour
{
    public string LocalID { get; set; }
    public string OnlineID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Author { get; set; }
    
    public PuzzleLayout Layout { get; set; }
    public long SolvedPieces { get; private set; }
    
    private Chunk[] Chunks => GetComponentsInChildren<Chunk>();
    public long TotalPieces => Chunks.Sum(chunk => chunk.PieceCount);
    public bool IsOnline => OnlineID != null;
    public double PercentComplete => (double) SolvedPieces / TotalPieces;

    public PuzzleRenderData RenderData { get; set; }
    
    public void InitializePuzzle(PuzzleSaveData saveData, PuzzleRenderData renderData)
    {
        LocalID = saveData.localID;
        OnlineID = saveData.onlineID;
        Name = saveData.name;
        Description = saveData.description;
        Author = saveData.author;
        Layout = saveData.layout;
        RenderData = renderData;
        
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
            
            ChunkFactory.Instance.CreateSinglePieceChunk(
                cut.solutionLocation + offset,
                Quaternion.identity,
                cut,
                this
            );
        }
    }
    
    private void InitializeSavedChunkStates(List<ChunkSaveData> chunks)
    {
        foreach (var chunkSaveData in chunks)
        {
            ChunkFactory.Instance.CreateMultiplePieceChunk(
                chunkSaveData,
                this
            );
        }
    }
    
    public PuzzleSaveData ToData()
    {
        return new PuzzleSaveData(
            localID: LocalID,
            onlineID: OnlineID,
            name: Name,
            description: Description,
            author: Author,
            layout: Layout,
            chunks: Chunks.Select(c => c.ToData()).ToList()
        );
    }
}