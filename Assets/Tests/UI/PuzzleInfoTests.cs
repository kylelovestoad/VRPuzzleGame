using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Persistence;
using PuzzleGeneration;
using TMPro;
using UI;
using UnityEditor;
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
        
        private PuzzleManager _puzzleManager;
        private Puzzle _puzzlePrefab;

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
            
            _puzzleManager = new GameObject().AddComponent<PuzzleManager>();
            
            _puzzlePrefab = AssetDatabase.LoadAssetAtPath<Puzzle>(
                "Assets/Prefabs/Puzzle.prefab"
            );
            
            var field = typeof(PuzzleManager).GetField(
                "puzzlePrefab", 
                BindingFlags.Instance | BindingFlags.NonPublic
            );
            field.SetValue(_puzzleManager, _puzzlePrefab);
            
            var puzzleManagerAwakeMethod = typeof(PuzzleManager).GetMethod(
                "Awake", 
                BindingFlags.Instance | BindingFlags.NonPublic
            );
            
            puzzleManagerAwakeMethod.Invoke(_puzzleManager, null);
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
            Object.DestroyImmediate(_puzzleManager.gameObject);
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
            var puzzleSaveData = TestUtils.MakePuzzle();
            
            _puzzleInfo.DisplayLocalPuzzle(puzzleSaveData);
            
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
            var puzzleSaveData = TestUtils.MakePuzzle();
            
            _puzzleInfo.DisplayLocalPuzzle(puzzleSaveData);
            
            var playMethod = typeof(PuzzleInfo).GetMethod(
                "OnPlay", 
                BindingFlags.Instance | BindingFlags.NonPublic
            );
            
            playMethod.Invoke(_puzzleInfo, null);
            
            Assert.NotNull(PuzzleManager.Instance.CurrentPuzzle);
        }
    }
}
