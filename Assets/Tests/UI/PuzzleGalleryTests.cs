using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UI;
using NUnit.Framework;
using UnityEngine;
using Persistence;
using PuzzleGeneration;
using TMPro;
using UnityEngine.UI;

namespace Tests.UI
{
    public class PuzzleGalleryTests
    {
        private GameObject _galleryObject;
        private PuzzleGallery _puzzleGallery;
        private PuzzleGalleryTile _tilePrefab;

        [SetUp]
        public void SetUp()
        {
            LocalSave.Initialize(Path.Combine(Application.persistentDataPath, "puzzles.db"));
            
            _galleryObject = new GameObject("PuzzleGallery");
            _puzzleGallery = _galleryObject.AddComponent<PuzzleGallery>();

            _tilePrefab = new GameObject("TilePrefab").AddComponent<PuzzleGalleryTile>();

            var field = typeof(PuzzleGallery)
                .GetField("puzzleGalleryItemPrefab", BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(_puzzleGallery, _tilePrefab.GetComponent<PuzzleGalleryTile>());
            
            var labelObject = new GameObject("Label");
            labelObject.transform.SetParent(_tilePrefab.transform);
            var label = labelObject.AddComponent<TextMeshProUGUI>();

            var imageObject = new GameObject("Image");
            imageObject.transform.SetParent(_tilePrefab.transform);
            var image = imageObject.AddComponent<Image>();

            var flags = BindingFlags.NonPublic | BindingFlags.Instance;
            var tileType = typeof(PuzzleGalleryTile);
            tileType.GetField("puzzleNameLabel", flags).SetValue(_tilePrefab, label);
            tileType.GetField("puzzleImage", flags).SetValue(_tilePrefab, image);

            typeof(PuzzleGallery)
                .GetField("puzzleGalleryItemPrefab", flags)
                .SetValue(_puzzleGallery, _tilePrefab);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_galleryObject);
            Object.DestroyImmediate(_tilePrefab.gameObject);
            
            LocalSave.Instance.DB.DropCollection("puzzles");
            LocalSave.Shutdown();
        }
        
        [Test]
        public void PuzzleGalleryInitallyEmpty()
        {
            _puzzleGallery.Start();
            
            var tiles = _galleryObject.GetComponentsInChildren<PuzzleGalleryTile>();
            Assert.AreEqual(0, tiles.Length);
        }
        
        [Test]
        public void PuzzleFillsWhenPuzzleCreated()
        {
            _puzzleGallery.Start();
            
            var puzzle = new PuzzleSaveData(
                null,
                null, 
                "dk", 
                "Author", 
                new PuzzleLayout(2, 2, PieceShape.Rectangle, new List<PieceCut>()), 
                null,
                new Texture2D(2, 2)
            );
            
            LocalSave.Instance.Create(puzzle);
            
            var tiles = _galleryObject.GetComponentsInChildren<PuzzleGalleryTile>();
            Assert.AreEqual(1, tiles.Length);
        }
        
        [Test]
        public void CreatesMultipleTilesForMultiplePuzzles()
        {
            var numPuzzles = 4;
            
            _puzzleGallery.Start();
            
            for (var i = 0; i < numPuzzles; i++)
            {
                var puzzle = new PuzzleSaveData(
                    null,
                    null, 
                    "dk", 
                    "Author", 
                    new PuzzleLayout(2, 2, PieceShape.Rectangle, new List<PieceCut>()), 
                    null,
                    new Texture2D(2, 2)
                );
                
                LocalSave.Instance.Create(puzzle);
            }
            
            var tiles = _galleryObject.GetComponentsInChildren<PuzzleGalleryTile>();
            Assert.AreEqual(numPuzzles, tiles.Length);
        }
        
        [Test]
        public void SameAmountOfPuzzlesWhenOneIsUpdated()
        {
            _puzzleGallery.Start();
            
            var puzzle = new PuzzleSaveData(
                null,
                null, 
                "dk", 
                "Author", 
                new PuzzleLayout(1, 1, PieceShape.Rectangle, new List<PieceCut>()), 
                null,
                new Texture2D(1, 1)
            );
            
            LocalSave.Instance.Create(puzzle);
            
            var tiles = _galleryObject.GetComponentsInChildren<PuzzleGalleryTile>();
            Assert.AreEqual(1, tiles.Length);

            puzzle.author = "DK";
            
            LocalSave.Instance.Save(puzzle);
            
            tiles = _galleryObject.GetComponentsInChildren<PuzzleGalleryTile>();
            Assert.AreEqual(1, tiles.Length);
        }
    }
}
