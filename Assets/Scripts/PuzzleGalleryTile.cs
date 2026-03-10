using Persistence;
using TMPro;
using UnityEngine;

public class PuzzleGalleryTile : MonoBehaviour
{
    [SerializeField]
    private GameObject puzzleNameLabel;

    private PuzzleSaveData _puzzleSaveData;

    public void SetName(PuzzleSaveData puzzleSaveData)
    {
        var text = puzzleNameLabel.GetComponent<TextMeshProUGUI>();
        text.text = puzzleSaveData.name;
        
        _puzzleSaveData = puzzleSaveData;
    }
    
    [ContextMenu("Test Click")]
    public void OnClick()
    {
        Debug.Log("Clicked Puzzle Tile ");
    }
}
