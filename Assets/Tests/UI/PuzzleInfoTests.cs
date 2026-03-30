using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Persistence;
using PuzzleGeneration;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Tests.UI
{
    public class PuzzleInfoTests
    {
        private PuzzleInfo _puzzleInfo;
        private Button _playButton;
        private TMP_Text _pieceCountField;
        private TMP_Text _pieceShapeField;
        private Image _puzzleImage;
        private TMP_Text _elapsedTimeField;
        private TMP_Text _percentCompleteField;
        private TMP_Text _pieceProgressField;

        [SetUp]
        public void SetUp()
        {
            _puzzleInfo = new GameObject().AddComponent<PuzzleInfo>();

            _playButton = new GameObject().AddComponent<Button>();
            _pieceCountField = new GameObject().AddComponent<TextMeshProUGUI>();
            _pieceShapeField = new GameObject().AddComponent<TextMeshProUGUI>();
            _puzzleImage = new GameObject().AddComponent<Image>();
            _elapsedTimeField = new GameObject().AddComponent<TextMeshProUGUI>();
            _percentCompleteField = new GameObject().AddComponent<TextMeshProUGUI>();
            _pieceProgressField = new GameObject().AddComponent<TextMeshProUGUI>();

            SetPrivateField("playButton", _playButton);
            SetPrivateField("pieceCountField", _pieceCountField);
            SetPrivateField("pieceShapeField", _pieceShapeField);
            SetPrivateField("puzzleImage", _puzzleImage);
            SetPrivateField("elapsedTimeField", _elapsedTimeField);
            SetPrivateField("percentCompleteField", _percentCompleteField);
            SetPrivateField("pieceProgressField", _pieceProgressField);

            var startMethod = typeof(PuzzleInfo).GetMethod(
                "Start", 
                BindingFlags.Instance | BindingFlags.NonPublic
            );
            
            startMethod.Invoke(_puzzleInfo, null);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_puzzleInfo.gameObject);
            Object.DestroyImmediate(_playButton.gameObject);
            Object.DestroyImmediate(_pieceCountField.gameObject);
            Object.DestroyImmediate(_pieceShapeField.gameObject);
            Object.DestroyImmediate(_puzzleImage.gameObject);
            Object.DestroyImmediate(_elapsedTimeField.gameObject);
            Object.DestroyImmediate(_percentCompleteField.gameObject);
            Object.DestroyImmediate(_pieceProgressField.gameObject);
        }

        private PuzzleSaveData MakePuzzle(string name = "Test Puzzle")
        {
            var vertices = new List<Vector2>
            {
                new(0, 0), 
                new(0, 1), 
                new(1, 0), 
                new(1, 1)
            };
            
            var piece0Cut = new PieceCut(0, new List<int> {1}, Vector2.zero, vertices);
            var piece1Cut = new PieceCut(1, new List<int> {0}, new Vector2(1, 0), vertices);
            
            var pieceCuts = new List<PieceCut> { piece0Cut, piece1Cut };
            
            return new(
                null,
                null, 
                name, 
                "Author", 
                new PuzzleLayout(2, 2, PieceShape.Rectangle, pieceCuts), 
                null,
                new Texture2D(2, 2)
            );
        }
            

        private void SetPrivateField(string fieldName, object value)
        {
            var field = typeof(PuzzleInfo).GetField(
                fieldName, 
                BindingFlags.Instance | BindingFlags.NonPublic
            );
            
            field.SetValue(_puzzleInfo, value);
        }

        [Test]
        public void FieldsSetCorrectly()
        {
            var puzzleSaveData = MakePuzzle();
            
            _puzzleInfo.DisplayPuzzle(puzzleSaveData);
            
            Assert.AreEqual(
                $"Piece Count: {puzzleSaveData.PieceCount}", 
                _pieceCountField.text
            );
            
            Assert.AreEqual(
                $"Piece Shape: {puzzleSaveData.layout.shape.ToString()}", 
                _pieceShapeField.text
            );
            
            Assert.AreEqual(
                $"{puzzleSaveData.elapsedTime}",
                _elapsedTimeField.text
            );
            
            Assert.AreEqual(
                $"{puzzleSaveData.PercentComplete():F0}% Complete", 
                _percentCompleteField.text
            );
            
            Assert.AreEqual(
                $"{puzzleSaveData.CurrentConnections()}/{puzzleSaveData.PieceCount}", 
                _pieceProgressField.text
            );
        }

        [Test]
        public void PlayButtonOpensPuzzle()
        {
            var puzzleSaveData = MakePuzzle();
            
            _puzzleInfo.DisplayPuzzle(puzzleSaveData);
            
            var playMethod = typeof(PuzzleInfo).GetMethod(
                "OnPlay", 
                BindingFlags.Instance | BindingFlags.NonPublic
            );
            
            playMethod.Invoke(_puzzleInfo, null);
            
            Assert.NotNull(PuzzleManager.Instance.CurrentPuzzle);
        }
    }
}
