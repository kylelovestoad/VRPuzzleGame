using System;
using System.Collections.Generic;
using Persistence;
using PuzzleGeneration;
using PuzzleGeneration.Jigsaw;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

namespace UI
{
    public class PuzzleCreationBehaviour : MonoBehaviour
    {
        public TMP_InputField nameInputField;
        public TMP_InputField rowsInputField;
        public TMP_InputField columnsInputField;
        public TMP_Dropdown dropdown;
        public Button createButton;
        public Texture2D puzzleImage;


        public void Start()
        {
            createButton.onClick.AddListener(OnCreate);
        }

        public void OnDestroy()
        {
            createButton.onClick.RemoveListener(OnCreate);
        }

        private void OnCreate()
        {
            var puzzleName = nameInputField.text;
            
            if (!Enum.IsDefined(typeof(PieceShape), 1))
            {
                Debug.LogWarning("Invalid shape");
                return;
            }
            
            var selectedShape = (PieceShape) 1;
            
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
                generator.Generate(puzzleImage, rows, columns, 0.1f),
                new List<ChunkSaveData>()
            ));
        }
    }
}