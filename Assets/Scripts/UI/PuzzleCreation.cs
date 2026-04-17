using System;
using System.Collections.Generic;
using Persistence;
using PuzzleGeneration;
using UnityEngine;
using UnityEngine.UI;
using EditorAttributes;

namespace UI
{
    public class PuzzleCreationBehaviour : MonoBehaviour
    {
        public const float PuzzleGameHeight = 0.3f;

        [SerializeField] 
        public Button createButton;
        
        [SerializeField]
        public PuzzleFormBehaviour puzzleFormBehaviour;
        
        [SerializeField] 
        private Button exitButton;
        
        private Texture2D _puzzleImage;

        public event Action OnPuzzleGenerated;
        public event Action<string, PuzzleGenerationData> OnRealPuzzleGenerated;
        public event Action OnExited;

        public void Start()
        {
            createButton.onClick.AddListener(OnCreate);
            exitButton.onClick.AddListener(OnExit);
        }

        public void OnDestroy()
        {
            createButton.onClick.RemoveListener(OnCreate);
            exitButton.onClick.RemoveListener(OnExit);
            
        }

        [Button("Create Puzzle")]
        private async void OnCreate()
        {
            var valid = puzzleFormBehaviour.TryGetFormInput(out var form);
            if (!valid) return;
            
            var generator = form.Shape.Generator();

            var generationData = await generator.Generate(
                form.PuzzleImage, 
                form.Rows, 
                form.Columns, 
                PuzzleGameHeight
            );
            
            OnGeneration(form, generationData);
        }

        private void OnGeneration(
            PuzzleForm form,
            PuzzleGenerationData generationData
        )
        {
            if (form.Shape == PieceShape.Real)
            {
                Debug.Log("PuzzleCreation: OnRealPuzzleGenerated");
                OnRealPuzzleGenerated?.Invoke(form.Name, generationData);
            }
            else
            {
                UIUtils.CreatePuzzleForCurrentUser(
                    form.Name,
                    generationData.Layout,
                    generationData.PuzzleImage
                );
                Debug.Log("Creating Local Puzzle");
                OnPuzzleGenerated?.Invoke();
            }
        }

        [Button("On Exit")]
        private void OnExit()
        {
            OnExited?.Invoke();
        }
    }
}