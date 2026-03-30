using System;
using System.Collections.Generic;
using System.Linq;
using Persistence;
using PuzzleGeneration;
using UnityEngine;
using UnityEngine.Serialization;

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
    public PuzzleRenderData RenderData { get; set; }
    
    private Chunk[] Chunks => GetComponentsInChildren<Chunk>();
    
    public float ElapsedTime { get; private set; }
    private bool _timeRunning;

    public long CurrentConnections => GoalConnections == Chunks.Length ? 0 : GoalConnections - Chunks.Length + 1;
    public long GoalConnections => Layout.initialPieceCuts.Count;
    public float PercentComplete => (float) CurrentConnections / GoalConnections * 100;
    public bool IsCompleted => CurrentConnections == GoalConnections;
    
    public bool IsOnline => OnlineID != null;
    
    public event Action<float> UpdateTimer;
    public event Action OnProgressUpdated;
    
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

        ElapsedTime = saveData.elapsedTime;
        _timeRunning = true;
    }
    
    void Update()
    {
        if (!_timeRunning) return;
        
        ElapsedTime += Time.deltaTime;

        UpdateTimer?.Invoke(ElapsedTime);
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
    
    private void OnTransformChildrenChanged()
    {
        _timeRunning = _timeRunning && !IsCompleted;
        OnProgressUpdated?.Invoke();
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