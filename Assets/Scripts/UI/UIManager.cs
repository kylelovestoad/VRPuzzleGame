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

        private MonoBehaviour[] _allPanels;
        
        private void Awake()
        {
            _instance = this;
            _allPanels = new MonoBehaviour[]
            {
                puzzleCreation,
                puzzleInfo,
                puzzleSettings,
                puzzleGallery,
                gameplayHUD,
                realPuzzleDetectionReport,
                leaderboard,
                completionDialog
            };
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
            
            leaderboard.OnExit += PuzzleLeaderboardExit;
            
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

            leaderboard.OnExit -= PuzzleLeaderboardExit;
            
            puzzleSettings.OnExited -= PuzzleSettingsExit;
            
            realPuzzleDetectionReport.OnExit -= OnRealPuzzleDetectionReportExit;
        }

        private void OnLocalPuzzleOpened()
        {
            Debug.Log("UI Manager: OnLocalPuzzleOpened");
            
            PuzzleManager.Instance.CurrentPuzzle.OnCompleted += OnPuzzleCompleted;
            
            ShowOnly(gameplayHUD);
            gameplayHUD.ShowLocalHud();
        }

        private void OnPuzzleClosed()
        {
            PuzzleManager.Instance.CurrentPuzzle.OnCompleted -= OnPuzzleCompleted;
            
            ShowPuzzleGallery();
        }
        
        private void OnOnlinePuzzleOpened()
        {
            Debug.Log("UI Manager: OnOnlinePuzzleOpened");
            
            PuzzleManager.Instance.CurrentPuzzle.OnCompleted += OnPuzzleCompleted;
            
            ShowOnly(gameplayHUD);
            gameplayHUD.ShowOnlineHud();
        }

        private void OnPuzzleCompleted()
        {
            completionDialog.gameObject.SetActive(true);
            completionDialog.DisplayFields();
        }

        private void ShowPuzzleCreation()
        {
            ShowOnly(puzzleCreation);
        }

        private void ShowSelectedPuzzle(PuzzleSaveData saveData)
        {
            ShowOnly(puzzleInfo);
            puzzleInfo.DisplayPuzzle(saveData);
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
            ShowOnly(puzzleGallery);
            completionDialog.gameObject.SetActive(false);
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

        private void ShowOnly(MonoBehaviour panel)
        {
            foreach (var p in _allPanels)
                p.gameObject.SetActive(false);
            panel.gameObject.SetActive(true);
        }
    }
}