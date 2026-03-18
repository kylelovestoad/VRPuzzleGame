using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using PuzzleGeneration;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    public class ChunkTests
    {
        private const float Tolerance = 1e-6f;

        private Chunk _chunkPrefab;
        private PuzzleLayout _mockLayout;
        private PuzzleRenderData _mockRenderData;
        private Puzzle _puzzle;
        private PieceCut _piece0Cut;
        private PieceCut _piece1Cut;

        [SetUp]
        public void SetUp()
        {
            GameObject puzzlePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/Prefabs/Puzzle.prefab"
            );
            
            _puzzle = puzzlePrefab.GetComponent<Puzzle>();
            _chunkPrefab = _puzzle.chunkPrefab;
            
            var vertices = new List<Vector2>
            {
                new(0, 0),
                new(0, 1),
                new(1, 0),
                new(1, 1)
            };

            _piece0Cut = new PieceCut(0, new List<int> {1}, Vector2.zero, vertices);
            _piece1Cut = new PieceCut(1, new List<int> {0}, new Vector2(1, 0), vertices);
            var pieceCuts = new List<PieceCut> { _piece0Cut, _piece1Cut };

            _mockLayout = new PuzzleLayout(2, 1, PieceShape.Rectangle, pieceCuts);
            _mockRenderData = new PuzzleRenderData(null, _mockLayout);
            _puzzle = new GameObject("Puzzle").AddComponent<Puzzle>();
            _puzzle.RenderData = _mockRenderData;
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var chunk in Object.FindObjectsOfType<Chunk>())
            {
                Object.DestroyImmediate(chunk.gameObject);
            }
        }

        #region Initialization Tests

        [Test]
        public void PrefabComponentsExist()
        {
            var chunk = Object.Instantiate(
                _chunkPrefab, 
                Vector3.zero, 
                Quaternion.identity
            );

            BoxCollider boxCollider = chunk.GetComponent<BoxCollider>();
            Assert.IsNotNull(boxCollider, "BoxCollider should exist on chunk");

            Rigidbody rigidbody = chunk.GetComponent<Rigidbody>();
            Assert.IsNotNull(rigidbody, "Rigidbody should exist on chunk");
        }

        [Test]
        public void MakesChunkWithSinglePieceInitially()
        {
            var chunk = Object.Instantiate(
                _chunkPrefab, 
                Vector3.zero, 
                Quaternion.identity
            );
            
            chunk.InitializeSinglePieceChunk(_piece0Cut, _puzzle);
        
            Assert.AreEqual(1, chunk.PieceCount);
        }
        
        #endregion
        
        #region Updating Chunk Tests
        
        [Test]
        public void UpdatesChunkSizeWhenMorePiecesAdded()
        {
            var chunk = Object.Instantiate(
                _chunkPrefab, 
                Vector3.zero, 
                Quaternion.identity
            );
            
            chunk.InitializeSinglePieceChunk(_piece0Cut, _puzzle);
            
            GameObject newPieceObject = chunk.transform.Find("Piece").gameObject;
            Piece piece = newPieceObject.AddComponent<Piece>();
            piece.transform.position = new Vector3(1, 0, 0);
            piece.InitializePiece(_piece1Cut, _puzzle.RenderData);
        
            MethodInfo updateBoxColliderMethod = typeof(Chunk).GetMethod(
                "UpdateBoxCollider",
                BindingFlags.NonPublic | BindingFlags.Instance
            );
        
            var pieces = new[] { piece };
            updateBoxColliderMethod?.Invoke(chunk, new object[] { pieces });
        
            BoxCollider boxCollider = chunk.GetComponent<BoxCollider>();
        
            Assert.AreEqual(boxCollider.size.x, 2.02f, Tolerance);
            Assert.AreEqual(boxCollider.size.y, 1.02f, Tolerance);
            Assert.AreEqual(boxCollider.size.z, 0.03f, Tolerance);
        
            Assert.AreEqual(boxCollider.center.x, 1f, Tolerance);
            Assert.AreEqual(boxCollider.center.y, 0.5f, Tolerance);
            Assert.AreEqual(boxCollider.center.z, 0.005f, Tolerance);
        }
        
        [Test]
        public void CombiningChunks()
        {
            var chunk = Object.Instantiate(
                _chunkPrefab, 
                Vector3.zero, 
                Quaternion.identity
            );
            
            chunk.InitializeSinglePieceChunk(_piece0Cut, _puzzle);
            
            var otherChunk = Object.Instantiate(
                _chunkPrefab, 
                new Vector3(1, 0, 0), 
                Quaternion.identity
            );
            
            otherChunk.InitializeSinglePieceChunk(_piece1Cut, _puzzle);
        
            MethodInfo combineMethod = typeof(Chunk).GetMethod(
                "Merge",
                BindingFlags.NonPublic | BindingFlags.Instance
            );
            combineMethod?.Invoke(chunk, new object[] { otherChunk });
        
            Assert.AreEqual(3, chunk.transform.childCount, "Merged chunk should contain both pieces");
            Assert.AreEqual(2, chunk.PieceCount, "Should have 2 pieces");
        }
        
        [Test]
        public void OnTriggerStayMergeWhenPiecesAreClose()
        {
            var chunk = Object.Instantiate(
                _chunkPrefab, 
                Vector3.zero, 
                Quaternion.identity
            );
            
            chunk.InitializeSinglePieceChunk(_piece0Cut, _puzzle);
            
            var otherChunk = Object.Instantiate(
                _chunkPrefab, 
                new Vector3(1, 0, 0), 
                Quaternion.identity
            );
            
            otherChunk.InitializeSinglePieceChunk(_piece1Cut, _puzzle);
            
            MethodInfo collideMethod = typeof(Chunk).GetMethod(
                "OnTriggerStay",
                BindingFlags.NonPublic | BindingFlags.Instance
            );
            collideMethod?.Invoke(chunk, new object[] { otherChunk.GetComponent<BoxCollider>() });
        
            if (chunk.PieceCount == 2)
            {
                return;
            }
        
            collideMethod?.Invoke(otherChunk, new object[] { chunk.GetComponent<BoxCollider>() });
            
            Assert.AreEqual(2, otherChunk.PieceCount);
        }
        
        [Test]
        public void OnTriggerStayNoMergeWhenPiecesNotClose()
        {
            var chunk = Object.Instantiate(
                _chunkPrefab, 
                Vector3.zero, 
                Quaternion.identity
            );
            
            chunk.InitializeSinglePieceChunk(_piece0Cut, _puzzle);
            
            var otherChunk = Object.Instantiate(
                _chunkPrefab, 
                Vector3.zero, 
                Quaternion.identity
            );
            
            otherChunk.InitializeSinglePieceChunk(_piece1Cut, _puzzle);
        
            MethodInfo collideMethod = typeof(Chunk).GetMethod(
                "OnTriggerStay",
                BindingFlags.NonPublic | BindingFlags.Instance
            );
            collideMethod?.Invoke(chunk, new object[] { otherChunk.GetComponent<BoxCollider>() });
        
            Assert.IsTrue(chunk.PieceCount == 1 && otherChunk.PieceCount == 1);
        
            collideMethod?.Invoke(otherChunk, new object[] { chunk.GetComponent<BoxCollider>() });
        
            Assert.IsTrue(chunk.PieceCount == 1 && otherChunk.PieceCount == 1);
        }
        
        #endregion
    }
}
