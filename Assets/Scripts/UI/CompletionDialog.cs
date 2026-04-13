using System;
using EditorAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CompletionDialog : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text finishTime;
        
        [SerializeField]
        private Button exitButton;

        private void Start()
        {
            exitButton.onClick.AddListener(OnExit);
        }
        
        private void OnDestroy()
        {
            exitButton.onClick.RemoveListener(OnExit);
        }

        public void DisplayFields()
        {
            var puzzle = PuzzleManager.Instance.CurrentPuzzle;
            
            var time = puzzle.ElapsedTime;

            var timeStr = UIUtils.AsTimeString(time);
            
            finishTime.text = $"Time: {timeStr}";
        }

        [Button("Exit Puzzle")]
        private void OnExit()
        {
            PuzzleManager.Instance.ClosePuzzle();
        }
    }
}
