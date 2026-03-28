using System;
using Persistence;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HUD : MonoBehaviour
    {
        public Button exitButton;
        
        [SerializeField]
        private TMP_Text timerField;
        [SerializeField]
        private TMP_Text progressPercentField;
        [SerializeField]
        private TMP_Text progressConnectionsField;
        
        private Puzzle _currentPuzzle;
        
        public void Start()
        {
            exitButton.onClick.AddListener(OnExit);

            PuzzleManager.Instance.OnPuzzleOpened += OnPuzzleOpened;
        }

        public void OnDestroy()
        {
            exitButton.onClick.RemoveListener(OnExit);
            
            PuzzleManager.Instance.OnPuzzleOpened -= OnPuzzleOpened;
            PuzzleManager.Instance.OnPuzzleClosed += OnPuzzleClosed;
        }
        
        private void OnPuzzleOpened(Puzzle puzzle)
        {
            _currentPuzzle = puzzle;
            _currentPuzzle.UpdateTimer += OnTimerUpdate;
            _currentPuzzle.OnProgressUpdated += OnProgressUpdated;
            
            progressPercentField.text = "0%";
            progressConnectionsField.text = $"0/{puzzle.GoalConnections}";
        }
        
        private void OnPuzzleClosed()
        {
            _currentPuzzle.UpdateTimer -= OnTimerUpdate;
            _currentPuzzle = null;
            timerField.text = "";
        }

        private void OnTimerUpdate(float timeRemaining)
        {
            var time = TimeSpan
                .FromSeconds(timeRemaining)
                .ToString(@"m\:ss");
            
            timerField.text = time;
        }
        
        private void OnProgressUpdated()
        {
            var currConnections = _currentPuzzle.CurrentConnections;
            var goalConnections = _currentPuzzle.GoalConnections;
                
            var percentComplete = (float) currConnections / goalConnections * 100;
            
            progressPercentField.text = $"{percentComplete:F0}%";
            progressConnectionsField.text = $"{currConnections}/{goalConnections}";
        }

        [ContextMenu("Exit Puzzle")]
        public void OnExit()
        {
            PuzzleManager.Instance.CloseCurrentPuzzle();
            timerField.text = "";
        }
    }
}
