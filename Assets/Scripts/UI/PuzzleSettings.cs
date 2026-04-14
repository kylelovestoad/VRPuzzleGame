using System;
using System.Collections.Generic;
using EditorAttributes;
using Networking;
using Networking.Request;
using Persistence;
using PuzzleGeneration;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PuzzleSettings : MonoBehaviour
    {
        [SerializeField] 
        public Button saveChangesButton;
        
        [SerializeField]
        private PuzzleFormBehaviour puzzleFormBehaviour;
        
        [SerializeField]
        private Image puzzleImage; 
        
        [SerializeField] 
        private Button exitButton;
        
        [SerializeField] 
        private Button deleteButton;
        
        private PuzzleMetadata _puzzleMetaData;
        
        public event Action OnExited;
        
        public event Action OnDeleted;

        public void Start()
        {
            saveChangesButton.onClick.AddListener(OnSave);
            exitButton.onClick.AddListener(OnExit);
            deleteButton.onClick.AddListener(OnDelete);
        }

        public void OnDestroy()
        {
            saveChangesButton.onClick.RemoveListener(OnSave);
            exitButton.onClick.RemoveListener(OnExit);
            deleteButton.onClick.RemoveListener(OnDelete);
        }

        public void Initialize(PuzzleMetadata metaData)
        {
            _puzzleMetaData = metaData;
            puzzleFormBehaviour.FillForm(_puzzleMetaData);
            
            puzzleImage.sprite = UIUtils.PuzzleImageSprite(metaData.PuzzleImage);
        }

        [Button("Save Puzzle")]
        private async void OnSave()
        {
            var valid = puzzleFormBehaviour.TryGetFormInput(out var form);
            if (!valid) return;

            var generator = form.Shape.Generator();

            var generationData = await generator.Generate(
                form.PuzzleImage,
                form.Rows, 
                form.Columns, 
                PuzzleCreationBehaviour.PuzzleGameHeight
            );
            
            OnGeneration(form, generationData);
        }

        private void OnGeneration(
            PuzzleForm form,
            PuzzleGenerationData generationData
        )
        {
            LocalSave.Instance.Save(new PuzzleSaveData(
                _puzzleMetaData.localID,
                _puzzleMetaData.onlineID,
                form.Name,
                _puzzleMetaData.authorId,
                _puzzleMetaData.author,
                generationData.Layout,
                new List<ChunkSaveData>(),
                generationData.PuzzleImage
            ));
        }

        [Button("On Delete")]
        private void OnDelete()
        {
            LocalSave.Instance.Delete(_puzzleMetaData.localID);
            
            OnDeleted?.Invoke();
        }

        [Button("On Exit")]
        private void OnExit()
        {
            OnExited?.Invoke();
        }
    }
}
