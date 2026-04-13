// using System.Collections.Generic;
// using System.IO;
// using System.Reflection;
// using UI;
// using NUnit.Framework;
// using UnityEngine;
// using Persistence;
// using PuzzleGeneration;
// using TMPro;
// using UnityEngine.UI;
//
// namespace Tests.UI
// {
//     public class PuzzleGalleryTests
//     {
//         private GameObject _galleryObject;
//         private PuzzleGallery _puzzleGallery;
//         private GameObject _puzzleGalleryItemContainer;
//         private GameObject _puzzleOnlineGalleryItemContainer;
//         private PuzzleGalleryTile _tilePrefab;
//         private Toggle _localTab;
//         private Toggle _onlineTab;
//
//         [SetUp]
//         public void SetUp()
//         {
//             LocalSave.Initialize(Path.Combine(Application.persistentDataPath, "puzzles.db"));
//     
//             _galleryObject = new GameObject("PuzzleGallery");
//             _puzzleGallery = _galleryObject.AddComponent<PuzzleGallery>();
//
//             var flags = BindingFlags.NonPublic | BindingFlags.Instance;
//             var galleryType = typeof(PuzzleGallery);
//
//             _puzzleGalleryItemContainer = new GameObject("LocalTileContainer");
//             galleryType.GetField("puzzleLocalGalleryItemContainer", flags)
//                 .SetValue(_puzzleGallery, _puzzleGalleryItemContainer);
//
//             _puzzleOnlineGalleryItemContainer = new GameObject("OnlineTileContainer");
//             galleryType.GetField("puzzleOnlineGalleryItemContainer", flags)
//                 .SetValue(_puzzleGallery, _puzzleOnlineGalleryItemContainer);
//             
//             _tilePrefab = new GameObject("TilePrefab").AddComponent<PuzzleGalleryTile>();
//             var tileType = typeof(PuzzleGalleryTile);
//
//             var labelObject = new GameObject("Label");
//             labelObject.transform.SetParent(_tilePrefab.transform);
//             tileType.GetField("puzzleNameLabel", flags).SetValue(_tilePrefab, labelObject.AddComponent<TextMeshProUGUI>());
//
//             var imageObject = new GameObject("Image");
//             imageObject.transform.SetParent(_tilePrefab.transform);
//             tileType.GetField("puzzleImage", flags).SetValue(_tilePrefab, imageObject.AddComponent<Image>());
//
//             galleryType.GetField("puzzleGalleryItemPrefab", flags).SetValue(_puzzleGallery, _tilePrefab);
//             
//             _localTab = new GameObject("LocalTab").AddComponent<Toggle>();
//             galleryType.GetField("localTab", flags).SetValue(_puzzleGallery, _localTab);
//
//             _onlineTab = new GameObject("OnlineTab").AddComponent<Toggle>();
//             galleryType.GetField("onlineTab", flags).SetValue(_puzzleGallery, _onlineTab);
//         }
//
//         [TearDown]
//         public void TearDown()
//         {
//             Object.DestroyImmediate(_galleryObject);
//             Object.DestroyImmediate(_puzzleGalleryItemContainer);
//             Object.DestroyImmediate(_puzzleOnlineGalleryItemContainer);
//             Object.DestroyImmediate(_tilePrefab.gameObject);
//             Object.DestroyImmediate(_localTab.gameObject);
//             Object.DestroyImmediate(_onlineTab.gameObject);
//             
//             LocalSave.Instance.DB.DropCollection("puzzles");
//             LocalSave.Shutdown();
//         }
//         
//         [Test]
//         public void PuzzleGalleryInitallyEmpty()
//         {
//             var startMethod = typeof(PuzzleGallery).GetMethod(
//                 "Start", 
//                 BindingFlags.Instance | BindingFlags.NonPublic
//             );
//             startMethod.Invoke(_puzzleGallery, null);
//             
//             var tiles = _puzzleGalleryItemContainer.GetComponentsInChildren<PuzzleGalleryTile>();
//             Assert.AreEqual(0, tiles.Length);
//         }
//         
//         [Test]
//         public void PuzzleFillsWhenPuzzleCreated()
//         {
//             var startMethod = typeof(PuzzleGallery).GetMethod(
//                 "Start", 
//                 BindingFlags.Instance | BindingFlags.NonPublic
//             );
//             startMethod.Invoke(_puzzleGallery, null);
//             
//             var puzzle = new PuzzleSaveData(
//                 null,
//                 null, 
//                 "dk", 
//                 "Author", 
//                 new PuzzleLayout(0, 0, 2, 2, PieceShape.Rectangle, new List<PieceCut>()), 
//                 null,
//                 new Texture2D(2, 2)
//             );
//             
//             LocalSave.Instance.Create(puzzle);
//             
//             var tiles = _puzzleGalleryItemContainer.GetComponentsInChildren<PuzzleGalleryTile>();
//             Assert.AreEqual(1, tiles.Length);
//         }
//         
//         [Test]
//         public void CreatesMultipleTilesForMultiplePuzzles()
//         {
//             var numPuzzles = 2;
//             
//             var startMethod = typeof(PuzzleGallery).GetMethod(
//                 "Start", 
//                 BindingFlags.Instance | BindingFlags.NonPublic
//             );
//             startMethod.Invoke(_puzzleGallery, null);
//             
//             for (var i = 0; i < numPuzzles; i++)
//             {
//                 var puzzle = new PuzzleSaveData(
//                     null,
//                     null, 
//                     "dk", 
//                     "Author", 
//                     new PuzzleLayout(0, 0, 2, 2, PieceShape.Rectangle, new List<PieceCut>()), 
//                     null,
//                     new Texture2D(2, 2)
//                 );
//                 
//                 LocalSave.Instance.Create(puzzle);
//             }
//             
//             var tiles = _puzzleGalleryItemContainer.GetComponentsInChildren<PuzzleGalleryTile>();
//             Assert.AreEqual(numPuzzles, tiles.Length);
//         }
//         
//         [Test]
//         public void SameAmountOfPuzzlesWhenOneIsUpdated()
//         {
//             var startMethod = typeof(PuzzleGallery).GetMethod(
//                 "Start", 
//                 BindingFlags.Instance | BindingFlags.NonPublic
//             );
//             startMethod.Invoke(_puzzleGallery, null);
//             
//             var puzzle = new PuzzleSaveData(
//                 null,
//                 null, 
//                 "dk", 
//                 "Author", 
//                 new PuzzleLayout(0, 0, 1, 1, PieceShape.Rectangle, new List<PieceCut>()), 
//                 null,
//                 new Texture2D(1, 1)
//             );
//             
//             LocalSave.Instance.Create(puzzle);
//             
//             var tiles = _puzzleGalleryItemContainer.GetComponentsInChildren<PuzzleGalleryTile>();
//             Assert.AreEqual(1, tiles.Length);
//
//             puzzle.authorId = "DK";
//             
//             LocalSave.Instance.Save(puzzle);
//             
//             tiles = _puzzleGalleryItemContainer.GetComponentsInChildren<PuzzleGalleryTile>();
//             Assert.AreEqual(1, tiles.Length);
//         }
//     }
// }
