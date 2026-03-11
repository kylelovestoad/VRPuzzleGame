using Persistence;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HUD : MonoBehaviour
    {
        public Button exitButton;

        private Puzzle _puzzle;
        
        public void Start()
        {
            exitButton.onClick.AddListener(OnExit);
        }

        public void OnDestroy()
        {
            exitButton.onClick.RemoveListener(OnExit);
        }

        [ContextMenu("Exit Puzzle")]
        public void OnExit()
        {
            PuzzleManager.Instance.CloseCurrentPuzzle();
        }
    }
}