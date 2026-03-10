using System;
using System.Linq;
using Persistence;
using UnityEngine;
using TMPro;

public class PuzzleGallery : MonoBehaviour
{
    [SerializeField]
    private PuzzleGalleryTile puzzleGalleryItemPrefab;

    private void Start()
    {
        FillPuzzles();
    }

    private void FillPuzzles()
    {
        var localSave = LocalSave.Instance;

        var puzzles = localSave.LoadAll();

        foreach (var puzzleSaveData in puzzles)
        {
            var galleryTile = Instantiate(puzzleGalleryItemPrefab, transform, false);
            
            galleryTile.gameObject.SetActive(true);
            galleryTile.SetFields(puzzleSaveData);
        }
    }
}
