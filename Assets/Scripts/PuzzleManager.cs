using System;
using Persistence;
using UnityEngine;


public class PuzzleManager : MonoBehaviour
{
    private static PuzzleManager _instance;
    public static PuzzleManager Instance => _instance ?? throw new NullReferenceException(
        "There must be one instance of PuzzleManager"
    );
    
    [SerializeField] 
    private Puzzle puzzlePrefab;

    public Puzzle CurrentPuzzle { get; private set; }
    
    public event Action OnPuzzleOpened;
    public event Action OnPuzzleClosed;

    private void Awake()
    {
        _instance = this;
    }

    public void OpenPuzzle(PuzzleSaveData puzzleSaveData)
    {
        CurrentPuzzle = Instantiate(puzzlePrefab);
        
        CurrentPuzzle.InitializePuzzle(puzzleSaveData);
        
        OnPuzzleOpened?.Invoke();
    }
    
    public void CloseCurrentPuzzle()
    {
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
}
