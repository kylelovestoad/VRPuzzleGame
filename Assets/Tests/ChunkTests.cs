using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Persistence;
using PuzzleGeneration;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    public class ChunkTests
    {
        private const float Tolerance = 1e-6f;

        private Chunk _chunkPrefab;
        private PuzzleSaveData _puzzleSaveData;
        private PieceCut _piece0Cut;
        private PieceCut _piece1Cut;
        private Puzzle _puzzle;

        [SetUp]
        public void SetUp()
        {
            var puzzlePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/Prefabs/Puzzle.prefab"
            );
            
            _puzzle = puzzlePrefab.GetComponent<Puzzle>();
            _chunkPrefab = _puzzle.chunkPrefab;

            _puzzleSaveData = TestUtils.MakePuzzle();

            var initialPieceCuts = _puzzleSaveData.layout.initialPieceCuts;

            _piece0Cut = initialPieceCuts[0];
            _piece1Cut = initialPieceCuts[1];
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var chunk in Object.FindObjectsOfType<Chunk>())
            {
                Object.DestroyImmediate(chunk.gameObject);
            }
            
            Object.DestroyImmediate(_puzzle.gameObject);
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
            
            chunk.InitializeSinglePieceChunk(_piece0Cut, _puzzleSaveData);
        
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
            
            chunk.InitializeSinglePieceChunk(_piece0Cut, _puzzleSaveData);
            
            GameObject newPieceObject = chunk.transform.Find("Piece").gameObject;
            Piece piece = newPieceObject.AddComponent<Piece>();
            piece.transform.position = new Vector3(1, 0, 0);
            piece.InitializePiece(_piece1Cut, _puzzleSaveData);
        
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
            
            chunk.InitializeSinglePieceChunk(_piece0Cut, _puzzleSaveData);
            
            var otherChunk = Object.Instantiate(
                _chunkPrefab, 
                new Vector3(1, 0, 0), 
                Quaternion.identity
            );
            
            otherChunk.InitializeSinglePieceChunk(_piece1Cut, _puzzleSaveData);
        
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
            
            chunk.InitializeSinglePieceChunk(_piece0Cut, _puzzleSaveData);
            
            var otherChunk = Object.Instantiate(
                _chunkPrefab, 
                new Vector3(1, 0, 0), 
                Quaternion.identity
            );
            
            otherChunk.InitializeSinglePieceChunk(_piece1Cut, _puzzleSaveData);
            
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
            
            chunk.InitializeSinglePieceChunk(_piece0Cut, _puzzleSaveData);
            
            var otherChunk = Object.Instantiate(
                _chunkPrefab, 
                Vector3.zero, 
                Quaternion.identity
            );
            
            otherChunk.InitializeSinglePieceChunk(_piece1Cut, _puzzleSaveData);
        
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
