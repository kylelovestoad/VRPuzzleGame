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

        public void SetVisible(PuzzleSaveData puzzleSaveData)
        {
            puzzleNameLabel.text = puzzleSaveData.name;
            puzzleImage.sprite = UIUtils.PuzzleImageSprite(puzzleSaveData);
        
            _puzzleSaveData = puzzleSaveData;
            
            gameObject.SetActive(true);
        }
    
        [ContextMenu("Open Puzzle")]
        public void OnClick()
        {
            PuzzleManager.Instance.OpenPuzzle(_puzzleSaveData);
        }
    }
}
