using Persistence;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HUD : MonoBehaviour
    {
        
        public Button exitButton;
        
        public void Start()
        {
            exitButton.onClick.AddListener(OnExit);
        }

        public void OnDestroy()
        {
            exitButton.onClick.RemoveListener(OnExit);
        }

        public void OnExit()
        {
            // TODO make better later!!! Actually hold a reference to current puzzle
            foreach (var puzzle in FindObjectsByType<Puzzle>(FindObjectsSortMode.None))
            {
                LocalSave.Instance.Save(puzzle.ToData());
                Destroy(puzzle.gameObject);
            }
        }
    }
}