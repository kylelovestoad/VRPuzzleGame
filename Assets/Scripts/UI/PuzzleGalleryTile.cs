using System;
using EditorAttributes;
using Persistence;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [Obsolete("Use ImageGalleryTile instead")]
    public class PuzzleGalleryTile : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI puzzleNameLabel;
    
        [SerializeField]
        private Image puzzleImage;

        [SerializeField] 
        private Button selectButton;

        private PuzzleSaveData _puzzleSaveData;
        
        public event Action<PuzzleSaveData> OnTileClicked;
        
        private void Start()
        {
            selectButton.onClick.AddListener(OnClick);
        }
        
        private void OnDestroy()
        {
            selectButton.onClick.RemoveListener(OnClick);
        }

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
