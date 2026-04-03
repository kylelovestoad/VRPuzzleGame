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
    private Material hintOutlineMaterial;

    public Puzzle CurrentPuzzle { get; private set; }
    
    private HintManager _hintManager;
    
    public event Action OnPuzzleOpened;
    public event Action OnPuzzleClosed;
    

    private void Awake()
    {
        _instance = this;
        _hintManager = new HintManager(hintOutlineMaterial);
    }

    public void OpenPuzzle(PuzzleSaveData puzzleSaveData)
    {
        Debug.Log("Puzzle Manager: PuzzleOpened");
        
        CurrentPuzzle = Instantiate(puzzlePrefab);
        
        CurrentPuzzle.InitializePuzzle(puzzleSaveData);
        
        OnPuzzleOpened?.Invoke();
    }
    
    public void CloseCurrentPuzzle()
    {
        Debug.Assert(CurrentPuzzle != null, "Puzzle must be playing to close it");
        
        var saveData = CurrentPuzzle.ToData();
        LocalSave.Instance.SaveSkipImage(saveData);
        
        OnPuzzleClosed?.Invoke();
        
    #if UNITY_INCLUDE_TESTS
        DestroyImmediate(CurrentPuzzle.gameObject);
    #else
        Destroy(CurrentPuzzle.gameObject);
    #endif
        
        CurrentPuzzle = null;
    }

    public void ShowPuzzleHint()
    {
        Debug.Assert(CurrentPuzzle != null, "Puzzle must be playing to give hint");
        
        var (piece0, piece1) = CurrentPuzzle.RandomUnconnectedPiecePair();
        
        _hintManager.ShowHint(piece0, piece1);
    }
}
