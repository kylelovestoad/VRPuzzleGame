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
        private PuzzleSettings puzzleSettings;

        [SerializeField] 
        private PuzzleGallery puzzleGallery;
        
        [SerializeField] 
        private HUD gameplayHUD;
        
        [SerializeField]
        private CompletionDialog completionDialog;
        
        [SerializeField]
        private RealPuzzleDetectionReport realPuzzleDetectionReport;

        [SerializeField] 
        private PuzzleLeaderboard leaderboard;
        
        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            PuzzleManager.Instance.OnLocalPuzzleOpened += OnLocalPuzzleOpened;
            PuzzleManager.Instance.OnPuzzleClosed += OnPuzzleClosed;
            PuzzleManager.Instance.OnOnlinePuzzleOpened += OnOnlinePuzzleOpened;

            puzzleGallery.OnPuzzleSelected += ShowSelectedPuzzle;
            puzzleGallery.OnCreateOptionSelected += ShowPuzzleCreation;
            
            puzzleCreation.OnRealPuzzleGenerated += OnRealPuzzleGenerated;
            puzzleCreation.OnExited += ShowPuzzleGallery;

            puzzleInfo.OnExited += ShowPuzzleGallery;
            puzzleInfo.OnSettingsOpened += PuzzleSettingsOpen;
            puzzleInfo.OnLeaderboardOpened += PuzzleLeaderboardOpen;
            
            puzzleSettings.OnExited += PuzzleSettingsExit;
            
            realPuzzleDetectionReport.OnExit += OnRealPuzzleDetectionReportExit;
            
            ShowPuzzleGallery();
        }

        private void OnDestroy()
        {
            PuzzleManager.Instance.OnLocalPuzzleOpened -= OnLocalPuzzleOpened;
            PuzzleManager.Instance.OnPuzzleClosed -= OnPuzzleClosed;
            PuzzleManager.Instance.OnOnlinePuzzleOpened -= OnOnlinePuzzleOpened;
            
            puzzleGallery.OnPuzzleSelected -= ShowSelectedPuzzle;
            puzzleGallery.OnCreateOptionSelected -= ShowPuzzleCreation;
            
            puzzleCreation.OnRealPuzzleGenerated -= OnRealPuzzleGenerated;
            puzzleCreation.OnExited -= ShowPuzzleGallery;
            
            puzzleInfo.OnExited -= ShowPuzzleGallery;
            puzzleInfo.OnSettingsOpened -= PuzzleSettingsOpen;
            puzzleInfo.OnLeaderboardOpened -= PuzzleLeaderboardOpen;

            
            leaderboard.OnExit += PuzzleLeaderboardExit;
            
            puzzleSettings.OnExited -= PuzzleSettingsExit;
            
            realPuzzleDetectionReport.OnExit -= OnRealPuzzleDetectionReportExit;
        }

        private void OnLocalPuzzleOpened()
        {
            Debug.Log("UI Manager: OnLocalPuzzleOpened");
            
            PuzzleManager.Instance.CurrentPuzzle.OnCompleted += OnPuzzleCompleted;
            
            ShowLocalGameplayHud();
        }

        private void OnPuzzleClosed()
        {
            PuzzleManager.Instance.CurrentPuzzle.OnCompleted -= OnPuzzleCompleted;
            
            ShowPuzzleGallery();
        }
        
        private void OnOnlinePuzzleOpened()
        {
            Debug.Log("UI Manager: OnLocalPuzzleOpened");
            
            PuzzleManager.Instance.CurrentPuzzle.OnCompleted += OnPuzzleCompleted;
            
            ShowOnlineGameplayHud();
        }

        private void OnPuzzleCompleted()
        {
            completionDialog.gameObject.SetActive(true);
            completionDialog.DisplayFields();
        }

        private void ShowPuzzleCreation()
        {
            puzzleInfo.gameObject.SetActive(false);
            gameplayHUD.gameObject.SetActive(false);
            completionDialog.gameObject.SetActive(false);
            realPuzzleDetectionReport.gameObject.SetActive(false);
            puzzleGallery.gameObject.SetActive(false);
            
            puzzleCreation.gameObject.SetActive(true);
        }

        private void ShowSelectedPuzzle(PuzzleSaveData saveData)
        {
            puzzleGallery.gameObject.SetActive(false);
            puzzleCreation.gameObject.SetActive(false);
            completionDialog.gameObject.SetActive(false);
            realPuzzleDetectionReport.gameObject.SetActive(false);
            gameplayHUD.gameObject.SetActive(false);
            
            puzzleInfo.DisplayLocalPuzzle(saveData);
        }
        
        // TODO HACKY Design needs to make more sense here with PuzzleSaveData and PuzzleMetadata being separate
        public void ShowSelectedPuzzle(PuzzleMetadata saveData)
        {
            puzzleInfo.DisplayOnlinePuzzle(saveData);
        }
        
        private void PuzzleSettingsOpen(PuzzleMetadata metaData)
        {
            puzzleSettings.gameObject.SetActive(true);
            puzzleSettings.Initialize(metaData);
        }
        
        private void PuzzleLeaderboardOpen(string puzzleId)
        {
            leaderboard.gameObject.SetActive(true);
            leaderboard.FillLeaderboard(puzzleId);
        }
        
        private void PuzzleLeaderboardExit()
        {
            leaderboard.gameObject.SetActive(false);
        }

        
        private void PuzzleSettingsExit()
        {
            puzzleSettings.gameObject.SetActive(false);
        }

        private void ShowPuzzleGallery()
        {
            puzzleCreation.gameObject.SetActive(false);
            puzzleInfo.gameObject.SetActive(false);
            puzzleSettings.gameObject.SetActive(false);
            gameplayHUD.gameObject.SetActive(false);
            completionDialog.gameObject.SetActive(false);
            realPuzzleDetectionReport.gameObject.SetActive(false);
            
            puzzleGallery.gameObject.SetActive(true);

        }

        private void ShowLocalGameplayHud()
        {
            puzzleGallery.gameObject.SetActive(false);
            puzzleCreation.gameObject.SetActive(false);
            puzzleInfo.gameObject.SetActive(false);
            puzzleSettings.gameObject.SetActive(false);
            completionDialog.gameObject.SetActive(false);
            realPuzzleDetectionReport.gameObject.SetActive(false);
            
            gameplayHUD.gameObject.SetActive(true);
            gameplayHUD.ShowLocalHud();
        }
        
        private void ShowOnlineGameplayHud()
        {
            puzzleGallery.gameObject.SetActive(false);
            puzzleCreation.gameObject.SetActive(false);
            puzzleInfo.gameObject.SetActive(false);
            puzzleSettings.gameObject.SetActive(false);
            completionDialog.gameObject.SetActive(false);
            realPuzzleDetectionReport.gameObject.SetActive(false);
            
            gameplayHUD.gameObject.SetActive(true);
            gameplayHUD.ShowOnlineHud();
        }
        
        private void OnRealPuzzleGenerated(
            string puzzleName, 
            PuzzleGenerationData generationData
        )
        {
            Debug.Log("UI Manager: OnRealPuzzleGenerated");
            
            realPuzzleDetectionReport.gameObject.SetActive(true);
            realPuzzleDetectionReport.Display(puzzleName, generationData);
        }

        private void OnRealPuzzleDetectionReportExit()
        {
            realPuzzleDetectionReport.gameObject.SetActive(false);
        }
    }
}
