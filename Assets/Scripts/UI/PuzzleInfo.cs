using Persistence;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    // TODO add logic from gallery tile to this since tile click brings you to this UI
    public class PuzzleInfo : MonoBehaviour
    {
        [SerializeField] 
        private Button button;
        
        [SerializeField]
        private Image puzzleImage;

        private PuzzleSaveData _puzzleSaveData;

        private void Start()
        {
            button.onClick.AddListener(OnPlay);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(OnPlay);
        }
        
        public void SetVisible(PuzzleSaveData puzzleSaveData)
        {
            _puzzleSaveData = puzzleSaveData;
            puzzleImage.sprite = UIUtils.PuzzleImageSprite(puzzleSaveData);
        
            _puzzleSaveData = puzzleSaveData;
            
            gameObject.SetActive(true);
        }

        private void OnPlay()
        {
            PuzzleManager.Instance.OpenPuzzle(_puzzleSaveData);
        } 
    }
}