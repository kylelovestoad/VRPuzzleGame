using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Persistence;
using PuzzleGeneration;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Puzzle: MonoBehaviour
{
    [SerializeField]
    public Chunk chunkPrefab;
    
    public string LocalID { get; set; }
    public string OnlineID { get; set; }
    public string Name { get; set; }
    public string Author { get; set; }
    
    public PuzzleLayout Layout { get; set; }
    
    public Texture2D PuzzleImage { get; set; }
    
    public Chunk[] Chunks => GetComponentsInChildren<Chunk>();
    
    public float ElapsedTime { get; private set; }
    private bool _timeRunning;

    public long CurrentConnections { get; private set; }
    public long GoalConnections => Layout.initialPieceCuts.Count;
    public float PercentComplete => (float) CurrentConnections / GoalConnections * 100;
    public bool IsCompleted => CurrentConnections == GoalConnections;
    
    public bool IsOnline => OnlineID != null;
    
    public event Action<float> UpdateTimer;
    public event Action<Piece[]> OnProgressUpdated;
    
    public void InitializePuzzle(PuzzleSaveData saveData)
    {
        LocalID = saveData.localID;
        OnlineID = saveData.onlineID;
        Name = saveData.name;
        Author = saveData.author;
        Layout = saveData.layout;
        PuzzleImage = saveData.PuzzleImage;
        
        InitializeChunks(saveData);

        ElapsedTime = saveData.elapsedTime;
        _timeRunning = true;
    }
    
    private void Update()
    {
        if (!_timeRunning) return;
        
        ElapsedTime += Time.deltaTime;

        UpdateTimer?.Invoke(ElapsedTime);
    }
    
    private void InitializeChunks(PuzzleSaveData saveData)
    {
        if (saveData.chunks == null || saveData.chunks.Count == 0)
        {
            InitializeSinglePieceChunks(saveData);
        }
        else
        {
            InitializeSavedChunkStates(saveData);
        }

        foreach (var chunk in Chunks)
        {
            chunk.OnMerge += OnChunkMerged;
        }
    }

    private void InitializeSinglePieceChunks(PuzzleSaveData saveData)
    {
        var initialCuts = Layout.initialPieceCuts;
        
        var grid = PuzzlePlacement.GetBoundingGrid(Layout);
        PuzzlePlacement.ShuffleCells(grid.Cells);

        Debug.Log("Grid Count " + grid.Cells.Count);
        
        for (var i = 0; i < initialCuts.Count; i++)
        {
            PlacePieceInCell(initialCuts[i], grid.Cells[i], grid, saveData);
        }
    }

    private void PlacePieceInCell(
        PieceCut cut, 
        PuzzlePlacement.Cell cell, 
        PuzzlePlacement.BoundingGrid boundingGrid, 
        PuzzleSaveData saveData
    )
    {
        var randomRotation = PuzzlePlacement.RandomRotationZ();
        var chunk = Instantiate(chunkPrefab, Vector3.zero, randomRotation, transform);
        chunk.InitializeSinglePieceChunk(cut, saveData);

        var piece = chunk.FirstPiece();
        var position = PuzzlePlacement.RandomPositionInCell(cell, boundingGrid, piece);
        chunk.transform.position = position;
    }
    
    private void InitializeSavedChunkStates(PuzzleSaveData saveData)
    {
        Debug.Assert(saveData.chunks != null, 
            "Puzzle In Progress must have chunk states");
        
        foreach (var chunkSaveData in saveData.chunks)
        {
            var chunk = Instantiate(
                chunkPrefab, 
                chunkSaveData.position, 
                chunkSaveData.rotation, 
                transform
            );

            CurrentConnections += chunkSaveData.pieces.Count - 1;
        
            chunk.InitializeMultiplePieceChunk(chunkSaveData, saveData);
        }

        if (CurrentConnections == 1)
        {
            CurrentConnections = 2;
        }
    }
    
    private void OnChunkMerged(Chunk combinedChunk, Chunk destroyedChunk)
    {
        CurrentConnections = CurrentConnections == 0 ? 2 : CurrentConnections + 1;
        _timeRunning = _timeRunning && combinedChunk.PieceCount != GoalConnections;

        destroyedChunk.OnMerge -= OnChunkMerged;
        
        OnProgressUpdated?.Invoke(combinedChunk.Pieces);
    }

    private Piece LookupPiece(int pieceIndex)
    {
        foreach (var chunk in Chunks)
        {
            if (chunk.TryLookupPiece(pieceIndex, out var piece))
                return piece;
        }
        
        return null;
    }

    public (Piece, Piece) RandomUnconnectedPiecePair()
    {
        var randChunkIndex = Random.Range(0, Chunks.Length);
        var pieces = Chunks[randChunkIndex].MissingConnections();
        
        Debug.Log($"Piece Index {pieces.Count}");
        
        var piece0MissingConnections = pieces[Random.Range(0, pieces.Count)];
        var piece0 = piece0MissingConnections.CurrPiece;
        var unconnectedNeighborIndices = piece0MissingConnections.UnconnectedNeighborIndices;

        var randUnconnectedNeighborIndex = Random.Range(0, unconnectedNeighborIndices.Count);
        var piece1 = LookupPiece(unconnectedNeighborIndices[randUnconnectedNeighborIndex]);

        return (piece0, piece1);
    }
    
    public PuzzleSaveData ToData()
    {
        return new PuzzleSaveData(
            localID: LocalID,
            onlineID: OnlineID,
            name: Name,
            author: Author,
            layout: Layout,
            puzzleImage: PuzzleImage,
            chunks: Chunks.Select(c => c.ToData()).ToList(),
            elapsedTime: ElapsedTime
        );
    }
}