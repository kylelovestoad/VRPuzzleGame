using System;
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
    
    public event Action OnPuzzleOpened;
    public event Action OnPuzzleClosed;
    

    private void Awake()
    {
        _instance = this;
        _hintManager = new HintManager(hintFrontMaterial, hintBackAndSidesMaterial);
    }

    public void OpenPuzzle(PuzzleSaveData puzzleSaveData)
    {
        Debug.Log("Puzzle Manager: PuzzleOpened");
        
        CurrentPuzzle = Instantiate(puzzlePrefab);
        CurrentPuzzle.InitializePuzzle(puzzleSaveData);
        CurrentPuzzle.OnProgressUpdated += OnChunkMerge;
        
        OnPuzzleOpened?.Invoke();
    }
    
    public void CloseCurrentPuzzle()
    {
        Debug.Assert(CurrentPuzzle != null, "Puzzle must be playing to close it");
        
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

    private void OnChunkMerge(Piece[] updatedPieces)
    {
        Debug.Assert(CurrentPuzzle != null, "Puzzle must be playing to merge");

        _hintManager.ClearHintIfConnected(updatedPieces);
    }

    public void ShowPuzzleHint()
    {
        Debug.Assert(CurrentPuzzle != null, "Puzzle must be playing to give hint");
        
        var (piece0, piece1) = CurrentPuzzle.RandomUnconnectedPiecePair();
        
        _hintManager.ShowHint(piece0, piece1);
    }
}
