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
        
        [SerializeField]
        private Button hintButton;

        private void Start()
        {
            exitButton.onClick.AddListener(OnExit);
            hintButton.onClick.AddListener(OnHint);
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
            hintButton.onClick.RemoveListener(OnHint);
        }
        
        public void ShowLocalHud()
        {
            var puzzle = PuzzleManager.Instance.CurrentPuzzle;
            
            puzzle.UpdateTimer += OnTimerUpdate;
            puzzle.OnProgressUpdated += OnProgressUpdated;

            hintButton.gameObject.SetActive(true);
            OnProgressUpdated(null);
        }
        
        public void ShowOnlineHud()
        {
            var puzzle = PuzzleManager.Instance.CurrentPuzzle;
            
            puzzle.UpdateTimer += OnTimerUpdate;
            puzzle.OnProgressUpdated += OnProgressUpdated;

            hintButton.gameObject.SetActive(false);
        }

        private void OnTimerUpdate(float timeRemaining)
        {
            var time = UIUtils.AsTimeString(timeRemaining);
            
            timerField.text = time;
        }
        
        private void OnProgressUpdated(Piece[] _)
        {
            var currentPuzzle =  PuzzleManager.Instance.CurrentPuzzle;
            
            var percentComplete = currentPuzzle.PercentComplete;
            progressPercentField.text = $"{percentComplete:F0}%";
            
            var currConnections = currentPuzzle.CurrentConnections;
            var goalConnections = currentPuzzle.GoalConnections;
            progressConnectionsField.text = $"{currConnections}/{goalConnections}";
        }

        [Button("Exit Puzzle")]
        private void OnExit()
        {
            var puzzle = PuzzleManager.Instance.CurrentPuzzle;
            
            puzzle.UpdateTimer -= OnTimerUpdate;
            puzzle.OnProgressUpdated -= OnProgressUpdated;
            
            PuzzleManager.Instance.ClosePuzzle();
        }
        
        [Button("Get Hint")]
        private void OnHint()
        {
            PuzzleManager.Instance.ShowPuzzleHint();
        }
    }
}
