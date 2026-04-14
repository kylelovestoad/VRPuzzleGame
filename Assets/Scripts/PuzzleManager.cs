using System;
using Networking;
using Networking.API;
using Persistence;
using UnityEngine;
using UnityEngine.Serialization;


public class PuzzleManager : MonoBehaviour
{
    private static PuzzleManager _instance;
    public static PuzzleManager Instance => _instance ?? throw new NullReferenceException(
        "There must be one instance of PuzzleManager"
    );
    
    [SerializeField] 
    private Puzzle puzzlePrefab;
    
    [SerializeField] 
    private Material hintFrontMaterial;
    
    [SerializeField] 
    private Material hintBackAndSidesMaterial;

    public Puzzle CurrentPuzzle { get; private set; }
    
    private HintManager _hintManager;
    
    public event Action OnLocalPuzzleOpened;
    public event Action OnPuzzleClosed;
    
    public event Action OnOnlinePuzzleOpened;
    

    private void Awake()
    {
        _instance = this;
        _hintManager = new HintManager(hintFrontMaterial, hintBackAndSidesMaterial);
    }

    public void OpenPuzzle(PuzzleSaveData puzzleSaveData)
    {
        if (puzzleSaveData.HasOnlineID)
        {
            OpenOnlinePuzzle(puzzleSaveData);
        }
        else
        {
            OpenLocalPuzzle(puzzleSaveData);
        }
    }

    private void OpenLocalPuzzle(PuzzleSaveData puzzleSaveData)
    {
        Debug.Log("Puzzle Manager: PuzzleOpened");
        
        CurrentPuzzle = Instantiate(puzzlePrefab);
        CurrentPuzzle.InitializePuzzle(puzzleSaveData);
        CurrentPuzzle.OnProgressUpdated += OnChunkMerge;
        
        OnLocalPuzzleOpened?.Invoke();
    }
    
    private void OpenOnlinePuzzle(PuzzleSaveData puzzleSaveData)
    {
        Debug.Log("Puzzle Manager: PuzzleOpened");
        
        CurrentPuzzle = Instantiate(puzzlePrefab);
        CurrentPuzzle.InitializePuzzle(puzzleSaveData);
        
        OnOnlinePuzzleOpened?.Invoke();
    }

    public void ClosePuzzle()
    {
        Debug.Log("PuzzleManager: ClosePuzzle");
        Debug.Assert(CurrentPuzzle != null, "Puzzle must be playing to close it");
        
        if (CurrentPuzzle.IsOnline)
        {
            CloseOnlinePuzzle();
        }
        else
        {
            CloseLocalPuzzle();
        }
    }
    
    private void CloseLocalPuzzle()
    {
        Debug.Log("Close local puzzle");
        
        CurrentPuzzle.OnProgressUpdated -= OnChunkMerge;
        
        var saveData = CurrentPuzzle.ToData();
        LocalSave.Instance.SaveSkipImage(saveData);
        
        OnPuzzleClosed?.Invoke();

        if (Application.isPlaying)
        {
            Destroy(CurrentPuzzle.gameObject);
        }
        else
        {
            DestroyImmediate(CurrentPuzzle.gameObject);
        }
        
        CurrentPuzzle = null;
    }
    
    private void CloseOnlinePuzzle()
    {
        OnPuzzleClosed?.Invoke();

        if (CurrentPuzzle.IsCompleted)
        {
            _ = PuzzleServerApi.Instance.Leaderboards
                .UpsertLeaderboardEntry(CurrentPuzzle.OnlineID, CurrentPuzzle.ElapsedTime);
        }

        if (Application.isPlaying)
        {
            Destroy(CurrentPuzzle.gameObject);
        }
        else
        {
            DestroyImmediate(CurrentPuzzle.gameObject);
        }
        
        CurrentPuzzle = null;
    }

    private void OnChunkMerge(Piece[] updatedPieces)
    {
        Debug.Assert(CurrentPuzzle != null, "Puzzle must be playing to merge");

        _hintManager.ClearHintIfConnected(updatedPieces);
    }

    public void ShowPuzzleHint()
    {
        Debug.Assert(CurrentPuzzle != null, "Puzzle must be playing to give hint");

        if (_hintManager.HintActive) return;
        
        var (piece0, piece1) = CurrentPuzzle.RandomUnconnectedPiecePair();
        
        _hintManager.ShowHint(piece0, piece1);
    }
}
