using System;
using EditorAttributes;
using Networking.DTO;
using Persistence;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PuzzleOnlineGalleryTile : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI puzzleNameLabel;
    
        [SerializeField]
        private Image puzzleImage;

        private PuzzleMetadata _metadata; 
        
        public event Action<PuzzleSaveData> OnTileClicked;
        
        public void DisplayPuzzle(PuzzleMetadata puzzleMetadata)
        {
            puzzleNameLabel.text = puzzleMetadata.name;
            puzzleImage.sprite = UIUtils.PuzzleImageSprite(puzzleMetadata.PuzzleImage);

            _metadata = puzzleMetadata;
            
            gameObject.SetActive(true);
        }
        
        [Button("Open Puzzle")]
        public void OnClick()
        {
            OnTileClicked?.Invoke(PuzzleSaveData.FromMetaData(_metadata));
        }
    }
}