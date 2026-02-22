using UnityEngine;
using UnityEngine.UI;

namespace Events
{
    public class PlayButtonBehaviour : MonoBehaviour
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