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
        
        public void Start()
        {
            exitButton.onClick.AddListener(OnExit);

            PuzzleManager.Instance.OnPuzzleOpened += OnPuzzleOpened;
        }

        public void OnDestroy()
        {
            exitButton.onClick.RemoveListener(OnExit);
            
            PuzzleManager.Instance.OnPuzzleOpened -= OnPuzzleOpened;
        }
        
        public void OnPuzzleOpened(Puzzle puzzle)
        {
            puzzle.UpdateTimer += OnTimerUpdate;
        }

        public void OnTimerUpdate(float timeRemaining)
        {
            var time = TimeSpan
                .FromSeconds(timeRemaining)
                .ToString(@"m\:ss");
            
            timerField.text = time;
        }

        [ContextMenu("Exit Puzzle")]
        public void OnExit()
        {
            PuzzleManager.Instance.CloseCurrentPuzzle();
        }
    }
}