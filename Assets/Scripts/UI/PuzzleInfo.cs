using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    // TODO add logic from gallery tile to this since tile click brings you to this UI
    public class PuzzleInfo : MonoBehaviour
    {
        public Button button;

        public void Start()
        {
            button.onClick.AddListener(OnPlay);
        }

        public void OnDestroy()
        {
            button.onClick.RemoveListener(OnPlay);
        }

        private static void OnPlay()
        {
            
        } 
    }
}