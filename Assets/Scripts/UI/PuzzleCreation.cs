using System;
using System.Collections.Generic;
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
        public TMP_InputField nameInputField;
        public TMP_InputField rowsInputField;
        public TMP_InputField columnsInputField;
        public DropDownGroup dropdown;
        public Button createButton;
        
        // TODO: upload
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
            
            var selectedShape = (PieceShape) selected;
            
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
            
            LocalSave.Instance.Create(new PuzzleSaveData(
                null,
                null,
                puzzleName,
                "DK", // TODO author will be handled with meta quest account
                generator.Generate(puzzleImage, rows, columns, 0.3f),
                new List<ChunkSaveData>(),
                puzzleImage // TODO this will be uploaded
            ));
        }
    }
}