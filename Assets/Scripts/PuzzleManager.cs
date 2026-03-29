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

    public Puzzle CurrentPuzzle { get; private set; }
    
    public event Action OnPuzzleOpened;
    public event Action OnPuzzleClosed;

    private void Awake()
    {
        _instance = this;
    }

    public void OpenPuzzle(PuzzleSaveData puzzleSaveData)
    {
        var puzzleRenderData = new PuzzleRenderData(
            puzzleSaveData.PuzzleImage, 
            puzzleSaveData.layout
        );
            
        CurrentPuzzle = Instantiate(puzzlePrefab);
        
        CurrentPuzzle.InitializePuzzle(
            puzzleSaveData,
            puzzleRenderData
        );
        
        OnPuzzleOpened?.Invoke();
    }
    
    public void CloseCurrentPuzzle()
    {
        var saveData = CurrentPuzzle.ToData();
        LocalSave.Instance.SaveSkipImage(saveData);
        
        OnPuzzleClosed?.Invoke();
        
        Destroy(CurrentPuzzle.gameObject);
        CurrentPuzzle = null;
    }
}
