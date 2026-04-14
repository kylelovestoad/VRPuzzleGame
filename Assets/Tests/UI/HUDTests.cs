using System.Collections.Generic;
using System.IO;
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
    public class HUDTests
    {
        private GameObject _hudObject;
        private HUD _hud;
        private Button _exitButton;
        
        private PuzzleSaveData MakePuzzle(string name = "Test Puzzle") =>
            new(
                null,
                null, 
                name, 
                "0", 
                "Dk",
                new PuzzleLayout(0, 0, 2, 2, PieceShape.Rectangle, new List<PieceCut>()), 
                null,
                new Texture2D(2, 2)
            );
        
        [SetUp]
        public void SetUp()
        {
            LocalSave.Initialize(Path.Combine(Application.persistentDataPath, "puzzles.db"));
            
            var puzzleManagerObject = new GameObject("PuzzleManager");
            var puzzleManager = puzzleManagerObject.AddComponent<PuzzleManager>();
            
            var instanceField = typeof(PuzzleManager)
                .GetField("_instance", BindingFlags.NonPublic | BindingFlags.Static);
            instanceField.SetValue(null, puzzleManager);
            
            Puzzle puzzlePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/Prefabs/Puzzle.prefab"
            ).GetComponent<Puzzle>();

            var prefabField = typeof(PuzzleManager)
                .GetField("puzzlePrefab", BindingFlags.NonPublic | BindingFlags.Instance);
            prefabField.SetValue(puzzleManager, puzzlePrefab);

            puzzleManager.OpenPuzzle(MakePuzzle());
            
            _hudObject = new GameObject("HUD");
            _hud = _hudObject.AddComponent<HUD>();

            var buttonObject = new GameObject("ExitButton");
            buttonObject.transform.SetParent(_hudObject.transform);
            _exitButton = buttonObject.AddComponent<Button>();
            _hud.exitButton = _exitButton;

            var timerObject = new GameObject("TimerField");
            timerObject.transform.SetParent(_hudObject.transform);
            var timerField = timerObject.AddComponent<TextMeshProUGUI>();
            typeof(HUD).GetField("timerField", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(_hud, timerField);

            var percentObject = new GameObject("ProgressPercentField");
            percentObject.transform.SetParent(_hudObject.transform);
            var percentField = percentObject.AddComponent<TextMeshProUGUI>();
            typeof(HUD).GetField("progressPercentField", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(_hud, percentField);

            var connectionsObject = new GameObject("ProgressConnectionsField");
            connectionsObject.transform.SetParent(_hudObject.transform);
            var connectionsField = connectionsObject.AddComponent<TextMeshProUGUI>();
            typeof(HUD).GetField("progressConnectionsField", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(_hud, connectionsField);

            var hintObject = new GameObject("HintButton");
            hintObject.transform.SetParent(_hudObject.transform);
            var hintButton = hintObject.AddComponent<Button>();
            typeof(HUD).GetField("hintButton", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(_hud, hintButton);
        }

        [TearDown]
        public void TearDown()
        {
            LocalSave.Instance.DB.DropCollection("puzzles");
            LocalSave.Shutdown();

            Object.DestroyImmediate(_hudObject);
        }
        
        [Test]
        public void PuzzleClosedWhenExitButtonClikced()
        {
            var startMethod = typeof(HUD).GetMethod(
                "Start", 
                BindingFlags.Instance | BindingFlags.NonPublic
            );
            startMethod.Invoke(_hud, null);
            
            _exitButton.onClick.Invoke();
            
            var currentPuzzle = PuzzleManager.Instance.CurrentPuzzle;
            
            Assert.IsNull(currentPuzzle, "Puzzle should be closed");
        }
    }
}