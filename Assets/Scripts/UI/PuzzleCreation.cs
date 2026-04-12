using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Networking;
using Networking.Request;
using Oculus.Interaction.Samples;
using Persistence;
using PuzzleGeneration;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EditorAttributes;
using PuzzleGeneration.Hexagon;
using PuzzleGeneration.Jigsaw;
using PuzzleGeneration.Real;
using PuzzleGeneration.Rectangle;
using Void = EditorAttributes.Void;

namespace UI
{
    public class PuzzleCreationBehaviour : MonoBehaviour
    {
        public const float PuzzleGameHeight = 0.3f;

        [SerializeField, HideProperty] public Button createButton;
        
        [SerializeField]
        private PuzzleFormBehaviour puzzleFormBehaviour;
        
        [SerializeField] 
        private Button exitButton;

        public Texture2D puzzleImage;
        public Texture2D realImage;
        
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
            // var generator = new RectanglePuzzleGenerator();

            var generationData = await generator.Generate(
                puzzleImage, 
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
                LocalSave.Instance.Create(new PuzzleSaveData(
                    null,
                    null,
                    form.Name,
                    "DK",
                    generationData.Layout,
                    new List<ChunkSaveData>(),
                    generationData.PuzzleImage
                ));
            }
        }

        [Button("On Exit")]
        private void OnExit()
        {
            OnExited?.Invoke();
        }
    }
}