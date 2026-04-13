using System;
using System.Collections.Generic;
using EditorAttributes;
using Networking;
using Networking.API;
using Networking.Request;
using NUnit.Framework;
using Persistence;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    // TODO add logic from gallery tile to this since tile click brings you to this UI
    public class PuzzleInfo : MonoBehaviour
    {
        [SerializeField] 
        private Button playButton;
        
        [SerializeField]
        private TMP_Text pieceCountField;
        
        [SerializeField]
        private TMP_Text pieceShapeField;
        
        [SerializeField]
        private TMP_Text createdByField;
        
        [SerializeField]
        private Image puzzleImage;

        [SerializeField] 
        private TMP_Text elapsedTimeField;
        
        [SerializeField] 
        private TMP_Text percentCompleteField;
        
        [SerializeField] 
        private TMP_Text pieceProgressField;
        
        [SerializeField]
        private Button leaderboardButton;
        
        [SerializeField]
        private Button uploadButton;
        
        [SerializeField]
        private Button saveCopyButton;
        
        [SerializeField] 
        private Button settingsButton;

        [SerializeField] 
        private Button exitButton;

        private PuzzleSaveData _puzzleSaveData;
        
        public event Action<string> OnLeaderboardOpened;
        public event Action<PuzzleMetadata> OnSettingsOpened;
        public event Action OnExited;

        private void Start()
        {
            LocalSave.Instance.OnSaved += OnPuzzleSaved;
            
            playButton.onClick.AddListener(OnPlay);
            uploadButton.onClick.AddListener(OnUpload);
            saveCopyButton.onClick.AddListener(OnSaveCopy);
            leaderboardButton.onClick.AddListener(OnOpenLeaderboard);
            settingsButton.onClick.AddListener(OnOpenSettings);
            exitButton.onClick.AddListener(OnExit);
        }

        private void OnDestroy()
        {
            LocalSave.Instance.OnSaved += OnPuzzleSaved;
            
            playButton.onClick.RemoveListener(OnPlay);
            uploadButton.onClick.RemoveListener(OnUpload);
            saveCopyButton.onClick.RemoveListener(OnSaveCopy);
            leaderboardButton.onClick.RemoveListener(OnOpenLeaderboard);
            settingsButton.onClick.RemoveListener(OnOpenSettings);
            exitButton.onClick.RemoveListener(OnExit);
        }

        public void DisplayPuzzle(PuzzleSaveData saveData)
        {
            if (saveData.HasOnlineID)
            {
                DisplayOnlinePuzzle(saveData.GetMetaData());
            }
            else
            {
                DisplayLocalPuzzle(saveData);
            }
        }
        
        private void DisplayLocalPuzzle(PuzzleSaveData puzzleSaveData)
        {
            createdByField.text = $"Created By: {puzzleSaveData.author}";
            pieceCountField.text = $"Piece Count: {puzzleSaveData.PieceCount}";
            pieceShapeField.text = $"Piece Shape: {puzzleSaveData.layout.shape.ToString()}";
            elapsedTimeField.text = $"{UIUtils.AsTimeString(puzzleSaveData.elapsedTime)}";
            percentCompleteField.text = $"{puzzleSaveData.PercentComplete():F0}% Complete";
            pieceProgressField.text = $"{puzzleSaveData.CurrentConnections()}/{puzzleSaveData.PieceCount}";
            
            puzzleImage.sprite = UIUtils.PuzzleImageSprite(puzzleSaveData.PuzzleImage);
            
            _puzzleSaveData = puzzleSaveData;
            
            settingsButton.gameObject.SetActive(true);
            leaderboardButton.gameObject.SetActive(false);
            gameObject.SetActive(true);
        }
        
        // TODO HACKY. Design needs to make more sense here with PuzzleSaveData and PuzzleMetadata being separate
        private async void DisplayOnlinePuzzle(PuzzleMetadata puzzleMetadata)
        {
            var user = await PuzzleServerApi.Instance.Manager.GetUser();
            
            settingsButton.gameObject.SetActive(user.ID.ToString() == puzzleMetadata.authorId);
            
            createdByField.text = $"Created By: {puzzleMetadata.author}";
            pieceCountField.text = $"Piece Count: {puzzleMetadata.PieceCount}";
            pieceShapeField.text = $"Piece Shape: {puzzleMetadata.layout.shape.ToString()}";
            elapsedTimeField.text = "";
            percentCompleteField.text = "0% Complete";
            pieceProgressField.text = $"0/{puzzleMetadata.PieceCount}";
            
            puzzleImage.sprite = UIUtils.PuzzleImageSprite(puzzleMetadata.PuzzleImage);
            
            _puzzleSaveData = PuzzleSaveData.FromMetaData(puzzleMetadata);
            
            leaderboardButton.gameObject.SetActive(true);
            gameObject.SetActive(true);
        }

        private void OnPuzzleSaved(List<PuzzleSaveData> saveDataList)
        {
            var saveData = saveDataList[0];

            DisplayPuzzle(saveData);
        }
        
        [Button("Leaderboard")]
        private void OnOpenLeaderboard()
        {
            OnLeaderboardOpened?.Invoke(_puzzleSaveData.onlineID);
        }
        
        [Button("Upload Puzzle")]
        private async void OnUpload()
        {
            var createRequest = new CreatePuzzleRequest(
                _puzzleSaveData.name,
                _puzzleSaveData.layout
            );
            
            await PuzzleServerApi.Instance.Puzzles.CreatePuzzle(
                createRequest, 
                _puzzleSaveData.PuzzleImage
            );
        }
        
        [Button("Save Puzzle Copy")]
        private void OnSaveCopy()
        {
            // might need to remove online id
            LocalSave.Instance.Create(_puzzleSaveData);
        }
        
        [Button("Settings")]
        private void OnOpenSettings()
        {
            OnSettingsOpened?.Invoke(_puzzleSaveData.GetMetaData());
        }

        [Button("Play Puzzle")]
        private void OnPlay()
        {
            PuzzleManager.Instance.OpenPuzzle(_puzzleSaveData);
        } 
        
        [Button("Exit")]
        private void OnExit()
        {
            OnExited?.Invoke();
        }
    }
}