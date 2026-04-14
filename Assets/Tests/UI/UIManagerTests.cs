using System.Collections.Generic;
using System.IO;
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
    public class UIManagerTests
    {
        private UIManager _uiManager;
        private PuzzleCreationBehaviour _puzzleCreation;
        private PuzzleInfo _puzzleInfo;
        private PuzzleGallery _puzzleGallery;
        private HUD _gameplayHUD;
        private CompletionDialog _completionDialog;
        private PuzzleSettings _puzzleSettings;
        private RealPuzzleDetectionReport _realPuzzleDetectionReport;
        private PuzzleLeaderboard _leaderboard;

        [SetUp]
        public void SetUp()
        {
            LocalSave.Initialize(Path.Combine(Application.persistentDataPath, "puzzles.db"));
            
            _uiManager = new GameObject().AddComponent<UIManager>();
            _puzzleCreation = new GameObject().AddComponent<PuzzleCreationBehaviour>();
            _puzzleInfo = new GameObject().AddComponent<PuzzleInfo>();
            _puzzleGallery = new GameObject().AddComponent<PuzzleGallery>();
            
            _completionDialog = new GameObject().AddComponent<CompletionDialog>();
            var flags = BindingFlags.NonPublic | BindingFlags.Instance;
            var dialogType = typeof(CompletionDialog);
            
            dialogType.GetField("finishTime", flags)
                .SetValue(_completionDialog, new GameObject("FinishTime").AddComponent<TextMeshProUGUI>());
            
            _gameplayHUD = new GameObject().AddComponent<HUD>();
            _gameplayHUD.exitButton = new GameObject("ExitButton").AddComponent<Button>();

            var hudType = typeof(HUD);

            hudType.GetField("timerField", flags)
                .SetValue(_gameplayHUD, new GameObject("TimerField").AddComponent<TextMeshProUGUI>());
            hudType.GetField("progressPercentField", flags)
                .SetValue(_gameplayHUD, new GameObject("ProgressPercentField").AddComponent<TextMeshProUGUI>());
            hudType.GetField("progressConnectionsField", flags)
                .SetValue(_gameplayHUD, new GameObject("ProgressConnectionsField").AddComponent<TextMeshProUGUI>());

            _puzzleSettings = new GameObject().AddComponent<PuzzleSettings>();
            _realPuzzleDetectionReport = new GameObject().AddComponent<RealPuzzleDetectionReport>();
            _leaderboard = new GameObject().AddComponent<PuzzleLeaderboard>();

            SetPrivateField("puzzleSettings", _puzzleSettings);
            SetPrivateField("realPuzzleDetectionReport", _realPuzzleDetectionReport);
            SetPrivateField("leaderboard", _leaderboard);
            SetPrivateField("puzzleCreation", _puzzleCreation);
            SetPrivateField("puzzleInfo", _puzzleInfo);
            SetPrivateField("puzzleGallery", _puzzleGallery);
            SetPrivateField("gameplayHUD", _gameplayHUD);
            SetPrivateField("completionDialog", _completionDialog);

            TestUtils.MakePuzzleManager();

            var uiManagerAwakeMethod = typeof(UIManager).GetMethod(
                "Awake", 
                BindingFlags.Instance | BindingFlags.NonPublic
            );
            
            uiManagerAwakeMethod.Invoke(_uiManager, null);
            
            var uiManagerStartMethod = typeof(UIManager).GetMethod(
                "Start", 
                BindingFlags.Instance | BindingFlags.NonPublic
            );
            
            uiManagerStartMethod.Invoke(_uiManager, null);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_uiManager.gameObject);
            Object.DestroyImmediate(_puzzleCreation.gameObject);
            Object.DestroyImmediate(_puzzleInfo.gameObject);
            Object.DestroyImmediate(_puzzleGallery.gameObject);
            Object.DestroyImmediate(_gameplayHUD.gameObject);
            Object.DestroyImmediate(_completionDialog.gameObject);
            
            LocalSave.Instance.DB.DropCollection("puzzles");
            LocalSave.Shutdown();
        }

        private void SetPrivateField(string fieldName, object value)
        {
            var field = typeof(UIManager).GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic
            );
            
            field.SetValue(_uiManager, value);
        }

        [Test]
        public void PuzzleOpen()
        {
            var puzzleSaveData = TestUtils.MakePuzzle();
            
            PuzzleManager.Instance.OpenPuzzle(puzzleSaveData);
            
            Assert.IsFalse(_puzzleCreation.gameObject.activeSelf);
            Assert.IsFalse(_puzzleGallery.gameObject.activeSelf);
            Assert.IsFalse(_puzzleInfo.gameObject.activeSelf);
            
            Assert.IsTrue(_gameplayHUD.gameObject.activeSelf);
        }
        
        [Test]
        public void PuzzleCompleted()
        {
            var puzzleSaveData = TestUtils.MakePuzzle();
            puzzleSaveData.layout.initialPieceCuts = new List<PieceCut>();
            
            PuzzleManager.Instance.OpenPuzzle(puzzleSaveData);
            
            var m = typeof(UIManager).GetMethod(
                "OnProgressUpdated", 
                BindingFlags.Instance | BindingFlags.NonPublic
            );
            
            m.Invoke(_uiManager, null);
            
            Assert.IsTrue(_completionDialog.gameObject.activeSelf);
        }
        
        [Test]
        public void PuzzleClose()
        {
            var puzzleSaveData = TestUtils.MakePuzzle();
            
            PuzzleManager.Instance.OpenPuzzle(puzzleSaveData);
            PuzzleManager.Instance.ClosePuzzle();
            
            Assert.IsTrue(_puzzleCreation.gameObject.activeSelf);
            Assert.IsTrue(_puzzleGallery.gameObject.activeSelf);
            
            Assert.IsFalse(_puzzleInfo.gameObject.activeSelf);
            Assert.IsFalse(_gameplayHUD.gameObject.activeSelf);
            Assert.IsFalse(_completionDialog.gameObject.activeSelf);
        }
    }
}
