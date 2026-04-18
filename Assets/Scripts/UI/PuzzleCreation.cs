using System;
using System.Collections;
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
        private void OnCreate()
        {
            StartCoroutine(OnCreateCoroutine());
        }

        private IEnumerator OnCreateCoroutine()
        {
            var valid = puzzleFormBehaviour.TryGetFormInput(out var form);
            if (!valid) yield break;

            var generator = form.Shape.Generator();

            var generationTask = generator.Generate(
                form.PuzzleImage,
                form.Rows,
                form.Columns,
                PuzzleGameHeight
            );

            yield return new WaitUntil(() => generationTask.IsCompleted);

            if (generationTask.IsFaulted)
            {
                Debug.LogError($"PuzzleCreation: Generation failed: {generationTask.Exception}");
                yield break;
            }

            OnGeneration(form, generationTask.Result);
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