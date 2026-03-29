using System;
using System.Collections.Generic;
using Networking;
using Networking.Request;
using Oculus.Interaction.Samples;
using Persistence;
using PuzzleGeneration;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    public class PuzzleCreationBehaviour : MonoBehaviour
    {
        private const float PuzzleGameHeight = 0.3f;
        
        public TMP_InputField nameInputField;
        public TMP_InputField rowsInputField;
        public TMP_InputField columnsInputField;
        public DropDownGroup dropdown;
        public Button createButton;

        public Texture2D puzzleImage;
        public Texture2D realImage;

        public void Start()
        {
            createButton.onClick.AddListener(OnCreate);
        }

        public void OnDestroy()
        {
            createButton.onClick.RemoveListener(OnCreate);
        }

        [ContextMenu("Create Puzzle")]
        private void OnCreate()
        {
            var puzzleName = nameInputField.text;
            var selected = dropdown.SelectedIndex;

            if (!Enum.IsDefined(typeof(PieceShape), selected))
            {
                Debug.LogWarning("Invalid shape");
                return;
            }

            var selectedShape = (PieceShape)selected;

            if (string.IsNullOrWhiteSpace(puzzleName))
            {
                Debug.LogWarning("Name field cannot be empty");
                return;
            }

            var notParsedRows = !int.TryParse(rowsInputField.text, out var rows);
            if (notParsedRows)
            {
                Debug.LogWarning("Row field cannot be empty");
                return;
            }

            var notParsedColumns = !int.TryParse(columnsInputField.text, out var columns);
            if (notParsedColumns)
            {
                Debug.LogWarning("Col field cannot be empty");
                return;
            }

            var generator = selectedShape.Generator();
            
            generator.Generate(realImage, rows, columns, PuzzleGameHeight, renderData =>
            {
                LocalSave.Instance.Create(new PuzzleSaveData(
                    null,
                    null,
                    puzzleName,
                    "DK",
                    renderData.Layout,
                    new List<ChunkSaveData>(),
                    renderData.PuzzleImage
                ));
            });
        }

        private bool ValidateInputs(out string puzzleName, out int rows, out int columns, out PieceShape selectedShape)
        {
            puzzleName = nameInputField.text;
            rows = 0;
            columns = 0;
            selectedShape = default;

            var selected = dropdown.SelectedIndex;
            if (!Enum.IsDefined(typeof(PieceShape), selected))
            {
                Debug.LogWarning("Invalid shape");
                return false;
            }

            selectedShape = (PieceShape)selected;

            if (string.IsNullOrWhiteSpace(puzzleName))
            {
                Debug.LogWarning("Name field cannot be empty");
                return false;
            }

            if (!int.TryParse(rowsInputField.text, out rows))
            {
                Debug.LogWarning("Row field cannot be empty");
                return false;
            }

            if (!int.TryParse(columnsInputField.text, out columns))
            {
                Debug.LogWarning("Col field cannot be empty");
                return false;
            }

            return true;
        }
    }
}