using System;
using EditorAttributes;
using Persistence;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PuzzleGalleryTile : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI puzzleNameLabel;
    
        [SerializeField]
        private Image puzzleImage;

        private PuzzleSaveData _puzzleSaveData;
        
        public event Action<PuzzleSaveData> OnTileClicked;

        public void DisplayPuzzle(PuzzleSaveData puzzleSaveData)
        {
            puzzleNameLabel.text = puzzleSaveData.name;
            puzzleImage.sprite = UIUtils.PuzzleImageSprite(puzzleSaveData.PuzzleImage);
        
            _puzzleSaveData = puzzleSaveData;
            
            gameObject.SetActive(true);
        }
    
        [Button("Open Puzzle")]
        public void OnClick()
        {
            OnTileClicked?.Invoke(_puzzleSaveData);
        }
    }
}
