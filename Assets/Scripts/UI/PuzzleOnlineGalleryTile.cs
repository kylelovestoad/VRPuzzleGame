    using System;
    using EditorAttributes;
    using Networking.DTO;
    using Persistence;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    namespace UI
    {
        [Obsolete("Use ImageGalleryTile instead")]
        public class PuzzleOnlineGalleryTile : MonoBehaviour
        {
            [SerializeField]
            private TextMeshProUGUI puzzleNameLabel;
        
            [SerializeField]
            private Image puzzleImage;
            
            [SerializeField] 
            private Button selectButton;

            private PuzzleMetadata _metadata; 
            
            public event Action<PuzzleSaveData> OnTileClicked;

            private void Start()
            {
                selectButton.onClick.AddListener(OnClick);
            }
            
            private void OnDestroy()
            {
                selectButton.onClick.RemoveListener(OnClick);
            }

            public void DisplayPuzzle(PuzzleMetadata puzzleMetadata)
            {
                puzzleNameLabel.text = puzzleMetadata.name;
                puzzleImage.sprite = UIUtils.PuzzleImageSprite(puzzleMetadata.PuzzleImage);

                _metadata = puzzleMetadata;
                
                gameObject.SetActive(true);
            }
            
            [Button("Open Puzzle")]
            public void OnClick()
            {
                OnTileClicked?.Invoke(PuzzleSaveData.FromMetaData(_metadata));
            }
        }
    }