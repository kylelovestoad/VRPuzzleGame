using System;
using System.Collections.Generic;
using EditorAttributes;
using Persistence;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class RealPuzzleDetectionReport : MonoBehaviour
    {
        [SerializeField]
        private Image realPuzzleImage;

        [SerializeField] 
        private Button acceptButton;
        
        [SerializeField] 
        private Button trashButton;

        private string _puzzleName;
        private PuzzleGenerationData _generationData;
        
        public event Action OnExit;
        
        private void Start()
        {
            acceptButton.onClick.AddListener(Accept);
            trashButton.onClick.AddListener(Trash);
        }
        
        private void OnDestroy()
        {
            acceptButton.onClick.RemoveListener(Accept);
            acceptButton.onClick.RemoveListener(Trash);
        }
        
        public void Display(
            string puzzleName, 
            PuzzleGenerationData generationData
        )
        {
            _puzzleName = puzzleName;
            _generationData = generationData;
            realPuzzleImage.sprite = UIUtils.PuzzleImageSprite(_generationData.PuzzleImage);
            
            gameObject.SetActive(true);
        }

        [Button("Accept")]
        private void Accept()
        {
            UIUtils.CreatePuzzleForCurrentUser(
                _puzzleName,
                _generationData.Layout,
                _generationData.PuzzleImage
            );
            
            OnExit?.Invoke();
        }
        
        [Button("Trash")]
        private void Trash()
        {
            OnExit?.Invoke();
        }
    }
}
