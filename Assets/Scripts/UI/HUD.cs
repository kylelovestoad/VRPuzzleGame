using System;
using EditorAttributes;
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

        private void Start()
        {
            exitButton.onClick.AddListener(OnExit);
        }

        private void OnDisable()
        {
            var puzzle = PuzzleManager.Instance.CurrentPuzzle;
            
            if (puzzle == null) return;
            
            puzzle.UpdateTimer -= OnTimerUpdate;
            puzzle.OnProgressUpdated -= OnProgressUpdated;
            
            timerField.text = "";
        }

        private void OnDestroy()
        {
            exitButton.onClick.RemoveListener(OnExit);
        }
        
        public void DisplayFields()
        {
            var puzzle = PuzzleManager.Instance.CurrentPuzzle;
            
            puzzle.UpdateTimer += OnTimerUpdate;
            puzzle.OnProgressUpdated += OnProgressUpdated;
            
            progressPercentField.text = "0%";
            progressConnectionsField.text = $"0/{puzzle.GoalConnections}";
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
            var currentPuzzle =  PuzzleManager.Instance.CurrentPuzzle;
            
            var percentComplete = currentPuzzle.PercentComplete;
            progressPercentField.text = $"{percentComplete:F0}%";
            
            var currConnections = currentPuzzle.CurrentConnections;
            var goalConnections = currentPuzzle.GoalConnections;
            progressConnectionsField.text = $"{currConnections}/{goalConnections}";
        }

        [Button("Exit Puzzle")]
        public void OnExit()
        {
            PuzzleManager.Instance.CloseCurrentPuzzle();
        }
    }
}
