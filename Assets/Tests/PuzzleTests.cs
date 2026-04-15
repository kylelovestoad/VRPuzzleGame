using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Oculus.Interaction;
using Persistence;
using PuzzleGeneration;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    public class PuzzleTests
    {
        private Puzzle _puzzlePrefab;
        private PuzzleSaveData _puzzleSaveData;
        private PuzzleSaveData _inProgressPuzzleSaveData;

        [SetUp]
        public void SetUp()
        {
            _puzzlePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/Prefabs/Puzzle.prefab"
            ).GetComponent<Puzzle>();

            _puzzleSaveData = TestUtils.MakePuzzle();
            _inProgressPuzzleSaveData = TestUtils.MakeInProgressPuzzle();
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var puzzle in Object.FindObjectsOfType<Puzzle>())
            {
                Object.DestroyImmediate(puzzle.gameObject);
            }
        }

        private Puzzle MakeAndInitPuzzle(PuzzleSaveData saveData)
        {
            var puzzle = Object.Instantiate(_puzzlePrefab);
            puzzle.InitializePuzzle(saveData);
            return puzzle;
        }

        #region Initialization Tests

        [Test]
        public void NewPuzzleHasOneChunkPerPiece()
        {
            var puzzle = MakeAndInitPuzzle(_puzzleSaveData);

            Assert.AreEqual(
                _puzzleSaveData.layout.initialPieceCuts.Count,
                puzzle.Chunks.Length
            );
        }

        [Test]
        public void InProgressPuzzleHasCorrectChunkCount()
        {
            var puzzle = MakeAndInitPuzzle(_inProgressPuzzleSaveData);

            Assert.AreEqual(
                _inProgressPuzzleSaveData.chunks.Count,
                puzzle.Chunks.Length
            );
        }

        [Test]
        public void InProgressPuzzleHasCorrectPieceCount()
        {
            var puzzle = MakeAndInitPuzzle(_inProgressPuzzleSaveData);

            var totalExpectedPieces = 0;
            foreach (var chunk in _inProgressPuzzleSaveData.chunks)
                totalExpectedPieces += chunk.pieces.Count;

            var totalActualPieces = 0;
            foreach (var chunk in puzzle.Chunks)
                totalActualPieces += chunk.PieceCount;

            Assert.AreEqual(totalExpectedPieces, totalActualPieces);
        }

        [Test]
        public void ElapsedTimeInitializesFromSaveData()
        {
            var puzzle = MakeAndInitPuzzle(_puzzleSaveData);

            Assert.AreEqual(_puzzleSaveData.elapsedTime, puzzle.ElapsedTime);
        }

        #endregion

        #region Progress Tracking Tests

        [Test]
        public void NewPuzzleCurrentConnectionsIsZero()
        {
            var puzzle = MakeAndInitPuzzle(_puzzleSaveData);

            Assert.AreEqual(0, puzzle.CurrentConnections);
        }

        [Test]
        public void GoalConnectionsMatchesPieceCutCount()
        {
            var puzzle = MakeAndInitPuzzle(_puzzleSaveData);

            Assert.AreEqual(
                _puzzleSaveData.layout.initialPieceCuts.Count,
                puzzle.GoalConnections
            );
        }

        [Test]
        public void NewPuzzleNotCompleted()
        {
            var puzzle = MakeAndInitPuzzle(_puzzleSaveData);

            Assert.IsFalse(puzzle.IsCompleted);
        }

        [Test]
        public void NewPuzzlePercentCompleteIsZero()
        {
            var puzzle = MakeAndInitPuzzle(_puzzleSaveData);

            Assert.AreEqual(0f, puzzle.PercentComplete, 0.001f);
        }

        [Test]
        public void InProgressPuzzleCurrentConnectionsGreaterThanZero()
        {
            var puzzle = MakeAndInitPuzzle(_inProgressPuzzleSaveData);

            Assert.Greater(puzzle.CurrentConnections, 0);
        }

        #endregion

        #region Online Status Tests

        [Test]
        public void IsOnlineFalseWhenNoOnlineID()
        {
            var puzzle = MakeAndInitPuzzle(_puzzleSaveData);

            Assert.IsFalse(puzzle.IsOnline);
        }

        #endregion

        #region Serialization Tests

        [Test]
        public void ToDataPreservesLocalID()
        {
            var puzzle = MakeAndInitPuzzle(_puzzleSaveData);
            var data = puzzle.ToData();

            Assert.AreEqual(_puzzleSaveData.localID, data.localID);
        }

        [Test]
        public void ToDataPreservesName()
        {
            var puzzle = MakeAndInitPuzzle(_puzzleSaveData);
            var data = puzzle.ToData();

            Assert.AreEqual(_puzzleSaveData.name, data.name);
        }

        [Test]
        public void ToDataChunkCountMatchesLiveChunks()
        {
            var puzzle = MakeAndInitPuzzle(_puzzleSaveData);
            var data = puzzle.ToData();

            Assert.AreEqual(puzzle.Chunks.Length, data.chunks.Count);
        }

        [Test]
        public void ToDataPreservesLayout()
        {
            var puzzle = MakeAndInitPuzzle(_puzzleSaveData);
            var data = puzzle.ToData();

            Assert.AreEqual(_puzzleSaveData.layout.initialPieceCuts.Count, data.layout.initialPieceCuts.Count);
        }

        #endregion
        
        [Test]
        public void RandomUnconnectedPiecePairReturnsTwoDifferentPieces()
        {
            var puzzle = MakeAndInitPuzzle(_puzzleSaveData);

            var (piece0, piece1) = puzzle.RandomUnconnectedPiecePair();

            Assert.IsNotNull(piece0);
            Assert.IsNotNull(piece1);
            Assert.AreNotEqual(piece0.PieceIndex, piece1.PieceIndex);
        }

        [Test]
        public void RandomUnconnectedPiecePairReturnedPiecesAreNeighbors()
        {
            var puzzle = MakeAndInitPuzzle(_puzzleSaveData);

            var (piece0, piece1) = puzzle.RandomUnconnectedPiecePair();

            Assert.IsTrue(piece0.NeighborIndices.Contains(piece1.PieceIndex));
            Assert.IsTrue(piece1.NeighborIndices.Contains(piece0.PieceIndex));
        }
        
        [Test]
        public void DroppingChunkNearNeighborMergesIntoSingleChunk()
        {
            var puzzle = Object.Instantiate(_puzzlePrefab);
            puzzle.InitializePuzzle(_puzzleSaveData);

            foreach (var chunk in puzzle.Chunks)
                chunk.transform.position = Vector3.zero;

            var chunks = puzzle.Chunks;
            var piece0 = chunks[0].FirstPiece();
            var piece1 = chunks[1].FirstPiece();

            var cut0 = _puzzleSaveData.layout.initialPieceCuts[piece0.PieceIndex];
            var cut1 = _puzzleSaveData.layout.initialPieceCuts[piece1.PieceIndex];

            var solutionDelta = new Vector3(
                cut1.solutionLocation.x - cut0.solutionLocation.x,
                cut1.solutionLocation.y - cut0.solutionLocation.y,
                0
            );
            
            chunks[0].transform.position = Vector3.zero;
            chunks[0].transform.rotation = Quaternion.identity;
            
            chunks[1].transform.position = solutionDelta;
            chunks[1].transform.rotation = Quaternion.identity;

            var pushups = typeof(Piece).GetMethod(
                "OnPointerEvent",
                BindingFlags.Instance | BindingFlags.NonPublic
            );

            var pointerEvent = new PointerEvent(0, PointerEventType.Unselect, Pose.identity, null);
            pushups.Invoke(piece0, new object[] { pointerEvent });

            Assert.AreEqual(1, puzzle.Chunks.Length, "Should have 1 chunk after merge");
            Assert.AreEqual(2, puzzle.Chunks[0].PieceCount, "Merged chunk should have 2 pieces");
        }
    }
}