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
        
        public void DisplayPuzzle(PuzzleMetadata puzzleMetadata)
        {
            puzzleNameLabel.text = puzzleMetadata.name;
            puzzleImage.sprite = UIUtils.PuzzleImageSprite(puzzleMetadata.PuzzleImage);

            _metadata = puzzleMetadata;
            
            gameObject.SetActive(true);
        }
        
        // TODO this will be moved to play info
        [Button("Download Puzzle")]
        public void OnClick()
        {
            LocalSave.Instance.Create(new PuzzleSaveData(
                null,
                _metadata.onlineID,
                _metadata.name,
                _metadata.author,
                _metadata.layout,
                null,
                _metadata.PuzzleImage
            ));
        }
    }
}