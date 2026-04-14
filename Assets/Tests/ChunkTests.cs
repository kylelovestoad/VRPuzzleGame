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

            var rigidbody = chunk.GetComponent<Rigidbody>();
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
        
            chunk.Merge(otherChunk);
        
            Assert.AreEqual(3, chunk.transform.childCount, "Merged chunk should contain both pieces");
            Assert.AreEqual(2, chunk.PieceCount, "Should have 2 pieces");
        }
        
        #endregion
    }
}
