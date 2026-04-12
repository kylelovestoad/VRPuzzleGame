using System;
using EditorAttributes;
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
        private Image puzzleImage;

        [SerializeField] 
        private TMP_Text elapsedTimeField;
        
        [SerializeField] 
        private TMP_Text percentCompleteField;
        
        [SerializeField] 
        private TMP_Text pieceProgressField;
        
        [SerializeField] 
        private Button settingsButton;

        [SerializeField] 
        private Button exitButton;

        private PuzzleSaveData _puzzleSaveData;
        
        public event Action<PuzzleMetadata> OnSettingsOpened;
        public event Action OnExited;

        private void Start()
        {
            playButton.onClick.AddListener(OnPlay);
            exitButton.onClick.AddListener(OnExit);
        }

        private void OnDestroy()
        {
            playButton.onClick.RemoveListener(OnPlay);
            exitButton.onClick.RemoveListener(OnExit);
        }
        
        public void DisplayPuzzle(PuzzleSaveData puzzleSaveData)
        {
            pieceCountField.text = $"Piece Count: {puzzleSaveData.PieceCount}";
            pieceShapeField.text = $"Piece Shape: {puzzleSaveData.layout.shape.ToString()}";
            elapsedTimeField.text = $"{puzzleSaveData.elapsedTime}";
            percentCompleteField.text = $"{puzzleSaveData.PercentComplete():F0}% Complete";
            pieceProgressField.text = $"{puzzleSaveData.CurrentConnections()}/{puzzleSaveData.PieceCount}";
            
            puzzleImage.sprite = UIUtils.PuzzleImageSprite(puzzleSaveData.PuzzleImage);
            
            _puzzleSaveData = puzzleSaveData;
            
            gameObject.SetActive(true);
        }
        
        // TODO HACKY. Design needs to make more sense here with PuzzleSaveData and PuzzleMetadata being separate
        public void DisplayPuzzle(PuzzleMetadata puzzleMetadata)
        {
            pieceCountField.text = $"Piece Count: {puzzleMetadata.PieceCount}";
            pieceShapeField.text = $"Piece Shape: {puzzleMetadata.layout.shape.ToString()}";
            elapsedTimeField.text = "";
            percentCompleteField.text = "0% Complete";
            pieceProgressField.text = $"0/{puzzleMetadata.PieceCount}";
            
            puzzleImage.sprite = UIUtils.PuzzleImageSprite(puzzleMetadata.PuzzleImage);
            
            gameObject.SetActive(true);
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