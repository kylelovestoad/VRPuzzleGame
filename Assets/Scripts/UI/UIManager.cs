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
        
        [SerializeField]
        private RealPuzzleDetectionReport realPuzzleDetectionReport;
        
        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            PuzzleManager.Instance.OnPuzzleOpened += OnPuzzleOpened;
            PuzzleManager.Instance.OnPuzzleClosed += OnPuzzleClosed;

            puzzleGallery.OnPuzzleSelected += ShowSelectedPuzzle;
            puzzleCreation.OnRealPuzzleGenerated += OnRealPuzzleGenerated;
            realPuzzleDetectionReport.OnExit += OnRealPuzzleDetectionReportExit;
            
            ShowBrowsingScreens();
        }

        private void OnDestroy()
        {
            PuzzleManager.Instance.OnPuzzleOpened -= OnPuzzleOpened;
            PuzzleManager.Instance.OnPuzzleClosed -= OnPuzzleClosed;
            
            puzzleGallery.OnPuzzleSelected -= ShowSelectedPuzzle;
            puzzleCreation.OnRealPuzzleGenerated -= OnRealPuzzleGenerated;
            realPuzzleDetectionReport.OnExit -= OnRealPuzzleDetectionReportExit;
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

        private void OnProgressUpdated(Piece[] _)
        {
            if (!PuzzleManager.Instance.CurrentPuzzle.IsCompleted) return;
            
            completionDialog.gameObject.SetActive(true);
            completionDialog.DisplayFields();
        }

        private void ShowSelectedPuzzle(PuzzleSaveData saveData)
        {
            puzzleInfo.DisplayPuzzle(saveData);
        }
        
        // TODO HACKY Design needs to make more sense here with PuzzleSaveData and PuzzleMetadata being separate
        // public void ShowSelectedPuzzle(PuzzleMetadata saveData)
        // {
        //     puzzleInfo.DisplayPuzzle(saveData);
        // }

        private void ShowBrowsingScreens()
        {
            puzzleCreation.gameObject.SetActive(true);
            puzzleGallery.gameObject.SetActive(true);
            
            puzzleInfo.gameObject.SetActive(false);
            gameplayHUD.gameObject.SetActive(false);
            completionDialog.gameObject.SetActive(false);
            realPuzzleDetectionReport.gameObject.SetActive(false);
        }

        private void ShowGameplayScreens()
        {
            puzzleCreation.gameObject.SetActive(false);
            puzzleInfo.gameObject.SetActive(false);
            puzzleGallery.gameObject.SetActive(false);
            realPuzzleDetectionReport.gameObject.SetActive(false);
            
            gameplayHUD.gameObject.SetActive(true);
            gameplayHUD.DisplayFields();
        }

        private void OnRealPuzzleGenerated(
            string puzzleName, 
            PuzzleGenerationData generationData
        )
        {
            realPuzzleDetectionReport.gameObject.SetActive(true);
            realPuzzleDetectionReport.Display(puzzleName, generationData);
        }

        private void OnRealPuzzleDetectionReportExit()
        {
            realPuzzleDetectionReport.gameObject.SetActive(false);
        }
    }
}
