using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EditorAttributes;
using Networking;
using Networking.API;
using Persistence;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class PuzzleGallery : MonoBehaviour
    {
        [SerializeField]
        private PuzzleGalleryTile puzzleGalleryItemPrefab;
        
        [SerializeField]
        private PuzzleOnlineGalleryTile puzzleOnlineGalleryItemPrefab;
        
        [FormerlySerializedAs("puzzleGalleryItemContainer")] [SerializeField]
        private GameObject puzzleLocalGalleryItemContainer;
        
        [SerializeField]
        private GameObject puzzleOnlineGalleryItemContainer;

        [SerializeField]
        private Toggle localTab;
        
        [SerializeField]
        private Toggle onlineTab; 
        
        [SerializeField]
        private Button onCreateOption;
        
        public event Action<PuzzleSaveData> OnPuzzleSelected;
        public event Action OnCreateOptionSelected;

        private enum Tab
        {
            Local,
            Online
        }

        private Tab _currentTab;
        private Tab CurrentTab
        {
            get => _currentTab;
            set
            {
                _currentTab = value;
                OnTabChanged(value);
            }
        }


        private void OnTabChanged(Tab value)
        {
            switch (value)
            {
                case Tab.Local:
                    ClearLocalPuzzles();
                    puzzleLocalGalleryItemContainer.SetActive(true);
                    puzzleOnlineGalleryItemContainer.SetActive(false);
                    FillLocalPuzzles();
                    break;
                case Tab.Online:
                    puzzleLocalGalleryItemContainer.SetActive(false);
                    puzzleOnlineGalleryItemContainer.SetActive(true);
                    FillOnlinePuzzles();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }
        
        [Button("Go To Local Tab")]
        private void SetLocalTab()
        {
            localTab.isOn = true;
        }

        [Button("Go To Online Tab")]
        private void SetOnlineTab()
        {
            onlineTab.isOn = true;
        }
        
        private void Start()
        {
            CurrentTab = Tab.Local;
            LocalSave.Instance.OnSaved += OnPuzzleSaved;
            localTab.onValueChanged.AddListener(isOn => { if (isOn) CurrentTab = Tab.Local; });
            onlineTab.onValueChanged.AddListener(isOn => { if (isOn) CurrentTab = Tab.Online; });

            onCreateOption.onClick.AddListener(OnCreateButtonClicked);
        }

        private void OnDestroy()
        {
            LocalSave.Instance.OnSaved -= OnPuzzleSaved;
            localTab.onValueChanged.RemoveAllListeners();
            onlineTab.onValueChanged.RemoveAllListeners();
            
            onCreateOption.onClick.RemoveListener(OnCreateButtonClicked);
        }
        
        private void FillLocalPuzzles()
        {
            var localSave = LocalSave.Instance;

            var puzzles = localSave.LoadAll();

            var p = puzzles.ToList();
            Debug.Log("Filling Puzzles " + p.Count);

            foreach (var puzzleSaveData in p)
            {
                Debug.LogError("Has Local Id " + puzzleSaveData.HasLocalID);
                
                var galleryTile = Instantiate(
                    puzzleGalleryItemPrefab,
                    puzzleLocalGalleryItemContainer.transform, 
                    false
                );

                galleryTile.OnTileClicked += Select;

                galleryTile.DisplayPuzzle(puzzleSaveData);
            }
        }
        
        private async void FillOnlinePuzzles()
        {
            try
            {
                OnUpdateOnline();
                Debug.Log("Filling Online Puzzles");
                var metadataDtos = await PuzzleServerApi.Instance.Puzzles.GetAllPuzzles();
                
                foreach (var metadataDto in metadataDtos)
                {
                    var galleryTile = Instantiate(
                        puzzleOnlineGalleryItemPrefab, 
                        puzzleOnlineGalleryItemContainer.transform, 
                        false
                    );

                    var image = await PuzzleServerApi.Instance.Content.DownloadImage(metadataDto.content.id);

                    var metadata = new PuzzleMetadata(
                        null,
                        metadataDto.onlineID,
                        metadataDto.name,
                        metadataDto.author,
                        metadataDto.layout,
                        image
                    );
                    
                    galleryTile.DisplayPuzzle(metadata);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }


        private void ClearLocalPuzzles()
        {
            var tiles = puzzleLocalGalleryItemContainer.GetComponentsInChildren<PuzzleGalleryTile>();
            Debug.Log("Child count " + tiles.Length);
            
            foreach (var tile in tiles)
            {
                if (Application.isPlaying)
                {
                    Destroy(tile.gameObject);
                }
                else
                {
                    DestroyImmediate(tile.gameObject);
                }
            }

        }
        
        private void OnPuzzleSaved(List<PuzzleSaveData> _)
        {
            ClearLocalPuzzles();
            FillLocalPuzzles();
        }
        
        private void OnUpdateOnline()
        {
            var tiles = puzzleOnlineGalleryItemContainer.GetComponentsInChildren<PuzzleOnlineGalleryTile>();
            
            Debug.Log("Child count " + tiles.Length);
            
            foreach (var tile in tiles)
            {
                if (Application.isPlaying)
                {
                    Destroy(tile.gameObject);
                }
                else
                {
                    DestroyImmediate(tile.gameObject);
                }
            }
        }

        private void Select(PuzzleSaveData puzzleSaveData)
        {
            OnPuzzleSelected?.Invoke(puzzleSaveData);
        }

        [Button("Create Puzzle")]
        private void OnCreateButtonClicked()
        {
            OnCreateOptionSelected?.Invoke();
        }
    }
}
