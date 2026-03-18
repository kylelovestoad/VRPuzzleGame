using System.Collections.Generic;
using System.Linq;
using Persistence;
using UnityEngine;

namespace UI
{
    public class PuzzleGallery : MonoBehaviour
    {
        [SerializeField]
        private PuzzleGalleryTile puzzleGalleryItemPrefab;

        public void Start()
        {
            FillPuzzles();
            LocalSave.Instance.OnSaved += OnPuzzleSaved;
        }

        public void OnDestroy()
        {
            LocalSave.Instance.OnSaved -= OnPuzzleSaved;
        }
        
        private void FillPuzzles()
        {
            var localSave = LocalSave.Instance;

            var puzzles = localSave.LoadAll();

            var p = puzzles.ToList();
            Debug.Log("Filling Puzzles " + p.Count);

            foreach (var puzzleSaveData in p)
            {
                var galleryTile = Instantiate(puzzleGalleryItemPrefab, transform, false);
            
                galleryTile.gameObject.SetActive(true);
                galleryTile.SetFields(puzzleSaveData);
            }
        }

        private void OnPuzzleSaved(List<PuzzleSaveData> _)
        {
            var tiles = GetComponentsInChildren<PuzzleGalleryTile>();
            Debug.Log("Child count " + tiles.Length);
            
            foreach (var tile in tiles)
            {
                DestroyImmediate(tile.gameObject);
            }
            
            FillPuzzles();
        }
        
    }
}
