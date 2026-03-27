using System;
using System.Collections.Generic;
using System.Linq;
using Persistence;
using PuzzleGeneration;
using UnityEngine;

public class Puzzle: MonoBehaviour
{
    [SerializeField]
    public Chunk chunkPrefab;
    
    public string LocalID { get; set; }
    public string OnlineID { get; set; }
    public string Name { get; set; }
    public string Author { get; set; }
    
    public PuzzleLayout Layout { get; set; }
    public long SolvedPieces { get; private set; }
    
    public Texture2D PuzzleImage { get; set; }
    
    private Chunk[] Chunks => GetComponentsInChildren<Chunk>();
    
    public event Action<float> UpdateTimer;
    private float _elapsedTime;
    private bool _timeRunning;
    
    
    public long TotalPieces => Chunks.Sum(chunk => chunk.PieceCount);
    public bool IsOnline => OnlineID != null;
    public double PercentComplete => (double) SolvedPieces / TotalPieces;

    public PuzzleRenderData RenderData { get; set; }
    
    public void InitializePuzzle(PuzzleSaveData saveData, PuzzleRenderData renderData)
    {
        LocalID = saveData.localID;
        OnlineID = saveData.onlineID;
        Name = saveData.name;
        Author = saveData.author;
        Layout = saveData.layout;
        RenderData = renderData;
        PuzzleImage = saveData.PuzzleImage;
        
        InitializeChunks(saveData);

        _elapsedTime = saveData.elapsedTime;
        _timeRunning = true;
    }
    
    void Update()
    {
        if (!_timeRunning) return;
        
        _elapsedTime += Time.deltaTime;

        UpdateTimer?.Invoke(_elapsedTime);
    }
    
    private void InitializeChunks(PuzzleSaveData saveData)
    {
        if (saveData.chunks == null || saveData.chunks.Count == 0)
        {
            InitializeSinglePieceChunks();
        }
        else
        {
            InitializeSavedChunkStates(saveData.chunks);
        }
    }

    private void InitializeSinglePieceChunks()
    {
        foreach (var cut in Layout.initialPieceCuts)
        {
            Debug.Log(cut.solutionLocation);

            // TODO: randomize placement
            var offset = new Vector3(cut.solutionLocation.x * 1.5f, cut.solutionLocation.y * 1.5f, 0);

            Chunk chunk = Instantiate(
                chunkPrefab, 
                cut.solutionLocation + offset, 
                Quaternion.identity, 
                transform
            );
        
            chunk.InitializeSinglePieceChunk(cut, this);
        }
    }
    
    private void InitializeSavedChunkStates(List<ChunkSaveData> chunks)
    {
        foreach (var chunkSaveData in chunks)
        {
            Chunk chunk = Instantiate(
                chunkPrefab, 
                chunkSaveData.position, 
                chunkSaveData.rotation, 
                transform
            );
        
            chunk.InitializeMultiplePieceChunk(chunkSaveData, this);
        }
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
            elapsedTime: _elapsedTime
        );
    }
}