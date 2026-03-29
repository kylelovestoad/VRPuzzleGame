using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] 
        private PuzzleCreationBehaviour puzzleCreation;

        [SerializeField] 
        private PuzzleGallery puzzleGallery;
        
        [SerializeField] 
        private HUD gameplayHUD;
        
        [SerializeField]
        private CompletionDialog completionDialog;

        private void Start()
        {
            PuzzleManager.Instance.OnPuzzleOpened += OnPuzzleOpened;
            PuzzleManager.Instance.OnPuzzleClosed += OnPuzzleClosed;
            
            ShowBrowsingScreens();
        }

        private void OnDestroy()
        {
            PuzzleManager.Instance.OnPuzzleOpened -= OnPuzzleOpened;
            PuzzleManager.Instance.OnPuzzleClosed -= OnPuzzleClosed;
        }

        private void OnPuzzleOpened()
        {
            PuzzleManager.Instance.CurrentPuzzle.OnProgressUpdated += OnProgressUpdated;
            
            ShowGameplayScreens();
        }

        private void OnPuzzleClosed()
        {
            PuzzleManager.Instance.CurrentPuzzle.OnProgressUpdated -= OnProgressUpdated;
            
            ShowBrowsingScreens();
        }

        private void OnProgressUpdated()
        {
            if (!PuzzleManager.Instance.CurrentPuzzle.IsCompleted) return;
            
            completionDialog.gameObject.SetActive(true);
            completionDialog.DisplayFields();
        }

        private void ShowBrowsingScreens()
        {
            puzzleCreation.gameObject.SetActive(true);
            puzzleGallery.gameObject.SetActive(true);
            
            gameplayHUD.gameObject.SetActive(false);
            completionDialog.gameObject.SetActive(false);
        }

        private void ShowGameplayScreens()
        {
            puzzleCreation.gameObject.SetActive(false);
            puzzleGallery.gameObject.SetActive(false);
            
            gameplayHUD.gameObject.SetActive(true);
            gameplayHUD.DisplayFields();
        }
    }
}
