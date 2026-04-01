using System;
using Persistence;
using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager _instance;
        public static UIManager Instance => _instance ?? throw new NullReferenceException(
            "There must be one instance of UIManager"
        );
        
        [SerializeField] 
        private PuzzleCreationBehaviour puzzleCreation;
        
        [SerializeField]
        private PuzzleInfo puzzleInfo;

        [SerializeField] 
        private PuzzleGallery puzzleGallery;
        
        [SerializeField] 
        private HUD gameplayHUD;
        
        [SerializeField]
        private CompletionDialog completionDialog;
        
        private void Awake()
        {
            _instance = this;
        }

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
            Debug.Log("UI Manager: OnPuzzleOpened");
            
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

        public void ShowSelectedPuzzle(PuzzleSaveData saveData)
        {
            puzzleInfo.DisplayPuzzle(saveData);
        }
        
        // TODO HACKY Design needs to make more sense here with PuzzleSaveData and PuzzleMetadata being separate
        public void ShowSelectedPuzzle(PuzzleMetadata saveData)
        {
            puzzleInfo.DisplayPuzzle(saveData);
        }

        private void ShowBrowsingScreens()
        {
            puzzleCreation.gameObject.SetActive(true);
            puzzleGallery.gameObject.SetActive(true);
            
            puzzleInfo.gameObject.SetActive(false);
            gameplayHUD.gameObject.SetActive(false);
            completionDialog.gameObject.SetActive(false);
        }

        private void ShowGameplayScreens()
        {
            puzzleCreation.gameObject.SetActive(false);
            puzzleInfo.gameObject.SetActive(false);
            puzzleGallery.gameObject.SetActive(false);
            
            gameplayHUD.gameObject.SetActive(true);
            gameplayHUD.DisplayFields();
        }
    }
}
