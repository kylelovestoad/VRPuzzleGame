// using System.Collections.Generic;
// using System.Reflection;
// using NUnit.Framework;
// using PuzzleGeneration;
// using UnityEditor;
// using UnityEngine;
//
// public class ChunkFactoryTests
// {
//     private GameObject _chunkFactoryGameObject;
//     private ChunkFactory _chunkFactory;
//     
//     [SetUp]
//     public void SetUp()
//     {
//         _chunkFactoryGameObject = new GameObject("TestFactory");
//         _chunkFactory = _chunkFactoryGameObject.AddComponent<ChunkFactory>();
//         
//         Chunk prefab = AssetDatabase.LoadAssetAtPath<Chunk>("Assets/Prefabs/Chunk.prefab");
//         FieldInfo field = typeof(ChunkFactory).GetField("chunkPrefab", BindingFlags.NonPublic | BindingFlags.Instance);
//         field.SetValue(_chunkFactory, prefab);
//     }
//
//     [TearDown]
//     public void TearDown()
//     {
//         Object.DestroyImmediate(_chunkFactoryGameObject);
//     }
//
//     [Test]
//     public void TestCreateChunk()
//     {
//         Mesh mesh = new Mesh();
//         mesh.vertices = new [] { Vector3.zero };
//         mesh.RecalculateBounds();
//         
//         var pieceCut = new PieceCut(Vector3.zero, mesh);
//         var pieceCuts = new List<PieceCut> { pieceCut };
//         
//         PuzzleLayout mockLayout = new PuzzleLayout(1, 1, PieceShape.Rectangle, pieceCuts);
//         PuzzleRenderData mockRenderData = new PuzzleRenderData(null, null, mockLayout);
//         
//         var puzzle = new Puzzle(mockRenderData);
//         
//         Chunk chunk = _chunkFactory.CreateSinglePieceChunk(Vector3.zero, Quaternion.identity, pieceCut, puzzle);
//         Assert.IsNotNull(chunk);
//     }
// }
