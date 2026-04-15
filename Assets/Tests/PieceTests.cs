using System.Reflection;
using NUnit.Framework;
using Oculus.Interaction;
using Persistence;
using PuzzleGeneration;
using UnityEngine;

namespace Tests
{
    public class PieceTests
    {
        private const float Tolerance = 1e-6f;
    
        private GameObject _pieceObject;
        private Piece _piece;
        private Mesh _simpleMesh;
        private PuzzleSaveData _puzzleSaveData;
        private PieceCut _pieceCut;
        private PieceCut _otherPieceCut;

        [SetUp]
        public void SetUp()
        {
            _pieceObject = TestUtils.CreatePieceObject("CurrPiece");
            _piece = _pieceObject.GetComponent<Piece>();
        
            _simpleMesh = new Mesh();
            _simpleMesh.vertices = new [] { Vector3.zero };
            _simpleMesh.RecalculateBounds();

            _puzzleSaveData = TestUtils.MakePuzzle();
            _pieceCut = _puzzleSaveData.layout.initialPieceCuts[0];
            _otherPieceCut = _puzzleSaveData.layout.initialPieceCuts[1];
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_pieceObject);
        }

        #region InitializeVariant Tests

        [Test]
        public void MeshSet()
        {
            var puzzle = TestUtils.MakePuzzle();

            _piece.InitializePiece(puzzle.layout.initialPieceCuts[0], puzzle);

            MeshFilter meshFilter = _pieceObject.GetComponent<MeshFilter>();
        
            Assert.IsNotNull(meshFilter.sharedMesh);
            Assert.AreEqual(8, meshFilter.sharedMesh.vertexCount);
        }

        #endregion

        #region IsCloseEnough Tests
        
        [Test]
        public void ExactSolutionLocation()
        {
            var otherObject = TestUtils.CreatePieceObject("OtherPiece");
            var otherPiece = otherObject.GetComponent<Piece>();
        
            _piece.InitializePiece(_pieceCut, _puzzleSaveData);
            otherPiece.InitializePiece(_otherPieceCut, _puzzleSaveData);
        
            _pieceObject.transform.position = _pieceCut.solutionLocation;
            otherObject.transform.position = _otherPieceCut.solutionLocation;
        
            bool result = _piece.IsCloseEnough(otherPiece);
        
            Assert.IsTrue(result);
        
            Object.DestroyImmediate(otherObject);
        }
        
        [Test]
        public void CorrectRelativeSolutionLocationAfterOffset()
        {
            var otherObject = TestUtils.CreatePieceObject("OtherPiece");
            var otherPiece = otherObject.GetComponent<Piece>();
            
            var solutionOffset = new Vector3(42, 42, 42);
        
            _piece.InitializePiece(_pieceCut, _puzzleSaveData);
            otherPiece.InitializePiece(_otherPieceCut, _puzzleSaveData);
        
            _pieceObject.transform.position = _pieceCut.solutionLocation + solutionOffset;
            otherObject.transform.position = _otherPieceCut.solutionLocation + solutionOffset;
        
            var result = _piece.IsCloseEnough(otherPiece);
        
            Assert.IsTrue(result);
        
            Object.DestroyImmediate(otherObject);
        }
        
        [Test]
        public void CorrectRelativeSolutionLocationAfterRotation()
        {
            var otherObject = TestUtils.CreatePieceObject("OtherPiece");
            var otherPiece = otherObject.GetComponent<Piece>();

            var solutionOffset = new Vector3(42, 42, 42);
        
            _piece.InitializePiece(_pieceCut, _puzzleSaveData);
            otherPiece.InitializePiece(_otherPieceCut, _puzzleSaveData);
        
            _pieceObject.transform.position = _pieceCut.solutionLocation + solutionOffset;
            otherObject.transform.position = _otherPieceCut.solutionLocation + solutionOffset;
        
            var result = _piece.IsCloseEnough(otherPiece);
        
            Assert.IsTrue(result);
        
            Object.DestroyImmediate(otherObject);
        }
        
        [Test]
        public void NotRelativeleyClosePieces()
        {
            var otherObject = TestUtils.CreatePieceObject("OtherPiece");
            var otherPiece = otherObject.GetComponent<Piece>();
        
            _piece.InitializePiece(_pieceCut, _puzzleSaveData);
            otherPiece.InitializePiece(_otherPieceCut, _puzzleSaveData);
        
            _pieceObject.transform.position = new Vector3(10, 0, 0);
            otherObject.transform.position = Vector3.zero;
        
            bool result = _piece.IsCloseEnough(otherPiece);
        
            Assert.IsFalse(result);
        
            Object.DestroyImmediate(otherObject);
        }
        
        [Test]
        public void RelativleyCloseWithinDistanceThreshold()
        {
            var otherObject = TestUtils.CreatePieceObject("OtherPiece");
            var otherPiece = otherObject.GetComponent<Piece>();
        
            _piece.InitializePiece(_pieceCut, _puzzleSaveData);
            otherPiece.InitializePiece(_otherPieceCut, _puzzleSaveData);
        
            _pieceObject.transform.position = _pieceCut.solutionLocation;
            otherObject.transform.position = _otherPieceCut.solutionLocation - new Vector3(0, 0.0099f, 0);
        
            bool result = _piece.IsCloseEnough(otherPiece);
        
            Assert.IsTrue(result);
        
            Object.DestroyImmediate(otherObject);
        }
        
        [Test]
        public void JustOutsideDistanceThreshold()
        {
            var otherObject = TestUtils.CreatePieceObject("OtherPiece");
            var otherPiece = otherObject.GetComponent<Piece>();
        
            _piece.InitializePiece(_pieceCut, _puzzleSaveData);
            otherPiece.InitializePiece(_otherPieceCut, _puzzleSaveData);
        
            _pieceObject.transform.position = _pieceCut.solutionLocation;
            otherObject.transform.position = _otherPieceCut.solutionLocation - new Vector3(0, 0.01f, 0);
        
            bool result = _piece.IsCloseEnough(otherPiece);
        
            Assert.IsFalse(result);
        
            Object.DestroyImmediate(otherObject);
        }
        
        [Test]
        public void RelativleyCloseWithinRotationThreshold()
        {
            var otherObject = TestUtils.CreatePieceObject("OtherPiece");
            var otherPiece = otherObject.GetComponent<Piece>();
        
            _piece.InitializePiece(_pieceCut, _puzzleSaveData);
            otherPiece.InitializePiece(_otherPieceCut, _puzzleSaveData);
        
            _pieceObject.transform.position = _pieceCut.solutionLocation;
            otherObject.transform.position = _otherPieceCut.solutionLocation;
            otherObject.transform.rotation = Quaternion.Euler(44, 0, 0);
        
            bool result = _piece.IsCloseEnough(otherPiece);
        
            Assert.IsTrue(result);
        
            Object.DestroyImmediate(otherObject);
        }
        
        [Test]
        public void JustOutsideRotationThreshold()
        {
            var otherObject = TestUtils.CreatePieceObject("OtherPiece");
            var otherPiece = otherObject.GetComponent<Piece>();
        
            _piece.InitializePiece(_pieceCut, _puzzleSaveData);
            otherPiece.InitializePiece(_otherPieceCut, _puzzleSaveData);
        
            _pieceObject.transform.position = _pieceCut.solutionLocation;
            otherObject.transform.position = _otherPieceCut.solutionLocation;
            otherObject.transform.rotation = Quaternion.Euler(45, 0, 0);
        
            bool result = _piece.IsCloseEnough(otherPiece);
        
            Assert.IsFalse(result);
        
            Object.DestroyImmediate(otherObject);
        }
        
        #endregion
        
        #region SnapIntoPlace Tests
        
        [Test]
        public void SnapIntoPlaceExactSolutionLocation()
        {
            var otherObject = TestUtils.CreatePieceObject("OtherPiece");
            var otherPiece = otherObject.GetComponent<Piece>();
        
            _piece.InitializePiece(_pieceCut, _puzzleSaveData);
            otherPiece.InitializePiece(_otherPieceCut, _puzzleSaveData);
        
            _pieceObject.transform.position = _pieceCut.solutionLocation;
            otherObject.transform.position = _otherPieceCut.solutionLocation;
        
            _piece.SnapIntoPlace(otherPiece);
        
            Assert.AreEqual(_pieceCut.solutionLocation, _pieceObject.transform.position);
        
            Object.DestroyImmediate(otherObject);
        }
        
        [Test]
        public void SnapIntoPlaceWithOffset()
        {
            var otherObject = TestUtils.CreatePieceObject("OtherPiece");
            var otherPiece = otherObject.GetComponent<Piece>();
        
            _piece.InitializePiece(_pieceCut, _puzzleSaveData);
            otherPiece.InitializePiece(_otherPieceCut, _puzzleSaveData);
        
            var offset = new Vector3(0.099f, 0, 0);
        
            _pieceObject.transform.position = _pieceCut.solutionLocation + offset;
            otherObject.transform.position = _otherPieceCut.solutionLocation;
        
            _piece.SnapIntoPlace(otherPiece);
        
            Assert.AreEqual(_pieceCut.solutionLocation, _pieceObject.transform.position);
        
            Object.DestroyImmediate(otherObject);
        }
        
        [Test]
        public void SnapIntoPlaceWithRotation180Degrees()
        {
            var otherObject = TestUtils.CreatePieceObject("OtherPiece");
            var otherPiece = otherObject.GetComponent<Piece>();
        
            _piece.InitializePiece(_pieceCut, _puzzleSaveData);
            otherPiece.InitializePiece(_otherPieceCut, _puzzleSaveData);
        
            var rotation = Quaternion.Euler(0, 180, 0);
            var rotatedOffset = rotation * (_otherPieceCut.solutionLocation - _pieceCut.solutionLocation);
            var expectedPosition = _pieceCut.solutionLocation + rotatedOffset;
        
            _pieceObject.transform.position = _pieceCut.solutionLocation;
            _pieceObject.transform.rotation = rotation;
            otherObject.transform.position = expectedPosition + new Vector3(0, 0.0001f, 0);
            otherObject.transform.rotation = rotation;
        
            otherPiece.SnapIntoPlace(_piece);
        
            Assert.AreEqual(expectedPosition.x, otherObject.transform.position.x, Tolerance);
            Assert.AreEqual(expectedPosition.y, otherObject.transform.position.y, Tolerance);
            Assert.AreEqual(expectedPosition.z, otherObject.transform.position.z, Tolerance);
        
            Object.DestroyImmediate(otherObject);
        }
        
        [Test]
        public void SnapIntoPlaceWithRotation()
        {
            var otherObject = TestUtils.CreatePieceObject("OtherPiece");
            var otherPiece = otherObject.GetComponent<Piece>();
        
            _piece.InitializePiece(_pieceCut, _puzzleSaveData);
            otherPiece.InitializePiece(_otherPieceCut, _puzzleSaveData);
        
            var targetRotation = Quaternion.Euler(45, 30, 60);
            otherObject.transform.rotation = targetRotation;
            _pieceObject.transform.rotation = Quaternion.identity;
        
            _piece.SnapIntoPlace(otherPiece);
        
            Assert.AreEqual(targetRotation, _pieceObject.transform.rotation);
        
            Object.DestroyImmediate(otherObject);
        }
        
        [Test]
        public void SnapIntoPlaceWithOffsetAndRotation()
        {
            var otherObject = TestUtils.CreatePieceObject("OtherPiece");
            var otherPiece = otherObject.GetComponent<Piece>();
        
            _piece.InitializePiece(_pieceCut, _puzzleSaveData);
            otherPiece.InitializePiece(_otherPieceCut, _puzzleSaveData);
        
            otherObject.transform.position = new Vector3(10, 5, 3);
            otherObject.transform.rotation = Quaternion.Euler(0, 45, 0);
        
            _piece.SnapIntoPlace(otherPiece);
        
            var rotatedOffset = otherObject.transform.rotation * 
                                (_pieceCut.solutionLocation - _otherPieceCut.solutionLocation);
            var expectedPosition = otherObject.transform.position + rotatedOffset;
        
            Assert.AreEqual(expectedPosition.x, _pieceObject.transform.position.x, Tolerance);
            Assert.AreEqual(expectedPosition.y, _pieceObject.transform.position.y, Tolerance);
            Assert.AreEqual(expectedPosition.z, _pieceObject.transform.position.z, Tolerance);
            Assert.AreEqual(otherObject.transform.rotation, _pieceObject.transform.rotation);
        
            Object.DestroyImmediate(otherObject);
        }
        
        #endregion
    }
}