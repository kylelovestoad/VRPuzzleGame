using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction.Samples;
using Persistence;
using PuzzleGeneration;
using PuzzleGeneration.Jigsaw;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.Serialization;

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
        
        public static IEnumerator GetPuzzleResponse(string url, Texture2D inputTexture, Action<PuzzleResponse> onComplete)
        {
            byte[] imageBytes = inputTexture.EncodeToPNG();
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>
            {
                new MultipartFormFileSection("file", imageBytes, "puzzle.png", "image/png")
            };
            
            var request = UnityWebRequest.Post(url, formData);
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var json = request.downloadHandler.text;
                
                Debug.Log(json.Substring(0, 100));
                
                var puzzleResponse = JsonUtility.FromJson<PuzzleResponse>(json);
                onComplete?.Invoke(puzzleResponse);
            }
            else
            {
                Debug.LogError(request.error);
            }
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

            // StartCoroutine(GetPuzzleResponse("http://localhost:6969/real-puzzle", realImage, response =>
            // {
            //     Debug.Log(response.initialPieceCuts);
            //     Debug.Log(response.solvedPuzzleImage);
            //     
            //     var renderData = response.ToPuzzleRenderData();
            //     
            //     LocalSave.Instance.Create(new PuzzleSaveData(
            //         null,
            //         null,
            //         puzzleName,
            //         "DK", // TODO author will be handled with meta quest account
            //         renderData.Layout,
            //         new List<ChunkSaveData>(),
            //         renderData.PuzzleImage // TODO this will be uploaded
            //     ));
            // }));
            
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