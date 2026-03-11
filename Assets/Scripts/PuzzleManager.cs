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

    private Puzzle _currentPuzzle;

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
            
        _currentPuzzle = Instantiate(puzzlePrefab);
        
        _currentPuzzle.InitializePuzzle(
            puzzleSaveData,
            puzzleRenderData
        );
    }
    
    public void CloseCurrentPuzzle()
    {
        var saveData = _currentPuzzle.ToData();
        LocalSave.Instance.SaveSkipImage(saveData);
        
        Destroy(_currentPuzzle.gameObject);
        _currentPuzzle = null;
    }
}
