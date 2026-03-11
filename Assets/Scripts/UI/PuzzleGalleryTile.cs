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

        public void SetFields(PuzzleSaveData puzzleSaveData)
        {
            puzzleNameLabel.text = puzzleSaveData.name;
        
            var puzzleImageTexture = puzzleSaveData.PuzzleImage;
            var puzzleImageSprite = Sprite.Create(
                puzzleImageTexture,
                new Rect(0, 0, puzzleImageTexture.width, puzzleImageTexture.height), 
                Vector2.zero
            );
            puzzleImage.sprite = puzzleImageSprite;
        
            _puzzleSaveData = puzzleSaveData;
        }
    
        [ContextMenu("Open Puzzle")]
        public void OnClick()
        {
            PuzzleManager.Instance.OpenPuzzle(_puzzleSaveData);
        }
    }
}
