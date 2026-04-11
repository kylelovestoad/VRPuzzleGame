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
        private const float PuzzleGameHeight = 0.3f;

        [FoldoutGroup("UI",
            nameof(nameInputField),
            nameof(rowsInputField),
            nameof(columnsInputField),
            nameof(dropdown),
            nameof(createButton),
            nameof(uploadButton)
        )]
        [SerializeField] private Void groupHolder;

        [SerializeField, HideProperty] public TMP_InputField nameInputField;
        [SerializeField, HideProperty] public TMP_InputField rowsInputField;
        [SerializeField, HideProperty] public TMP_InputField columnsInputField;
        [SerializeField, HideProperty] public DropDownGroup dropdown;
        [SerializeField, HideProperty] public Button createButton;
        [SerializeField, HideProperty] public Button uploadButton;

        [SerializeField] 
        private Button exitButton;

        public Texture2D puzzleImage;
        public Texture2D realImage;
        
        public event Action<string, PuzzleGenerationData> OnRealPuzzleGenerated;
        public event Action OnExited;

        private class PuzzleCreationForm
        {
            public readonly string Name;
            public readonly PieceShape Shape;
            public readonly int Rows;
            public readonly int Columns;

            public PuzzleCreationForm(string name, PieceShape shape, int rows, int columns)
            {
                Name = name;
                Shape = shape;
                Rows = rows;
                Columns = columns;
            }
        }

        public void Start()
        {
            createButton.onClick.AddListener(OnCreate);
            uploadButton.onClick.AddListener(OnUpload);
            exitButton.onClick.AddListener(OnExit);
        }

        public void OnDestroy()
        {
            createButton.onClick.RemoveListener(OnCreate);
            uploadButton.onClick.RemoveListener(OnUpload);
            exitButton.onClick.RemoveListener(OnExit);
        }


        private bool TryGetFormInput(out PuzzleCreationForm input)
        {
            input = null;

            if (string.IsNullOrWhiteSpace(nameInputField.text)) return false;
            if (!Enum.IsDefined(typeof(PieceShape), dropdown.SelectedIndex)) return false;
            if (!int.TryParse(rowsInputField.text, out var rows)) return false;
            if (!int.TryParse(columnsInputField.text, out var columns)) return false;

            input = new PuzzleCreationForm(nameInputField.text, (PieceShape)dropdown.SelectedIndex, rows, columns);
            return true;
        }

        [Button("Create Puzzle")]
        private async void OnCreate()
        {

            var valid = TryGetFormInput(out var form);
            if (!valid) return;

            var generator = form.Shape.Generator();

            var generationData = await generator.Generate(
                puzzleImage, 
                form.Rows, 
                form.Columns, 
                PuzzleGameHeight
            );
            
            OnGeneration(form, generationData);
        }

        private void OnGeneration(
            PuzzleCreationForm form,
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

        [Button("Upload Puzzle")]
        public async void OnUpload()
        {
            var valid = TryGetFormInput(out var form);
            if (!valid) return;
            
            var generator = form.Shape.Generator();

            var renderData = await generator.Generate(
                puzzleImage, 
                form.Rows, 
                form.Columns, 
                PuzzleGameHeight
            );
            
            await PuzzleServerApi.Instance.CreatePuzzle(new CreatePuzzleRequest(
                form.Name,
                renderData.Layout
            ), puzzleImage);
        }

        [Button("On Exit")]
        private void OnExit()
        {
            OnExited?.Invoke();
        }
    }
}