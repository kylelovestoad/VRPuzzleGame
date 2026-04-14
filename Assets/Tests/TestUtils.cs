using System.Collections.Generic;
using System.Reflection;
using Persistence;
using PuzzleGeneration;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    public static class TestUtils
    {
        public static PuzzleSaveData MakePuzzle(string name = "Test Puzzle")
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
                "0", 
                "DK",
                new PuzzleLayout(1, 2, 2, 2, PieceShape.Rectangle, pieceCuts), 
                null,
                new Texture2D(2, 2)
            );
        }

        public static PuzzleSaveData MakeInProgressPuzzle(string name = "Test Puzzle")
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

            var piece0SaveData = new PieceSaveData
            {
                pieceIndex = 0,
                position = Vector3.zero,
                rotation = Quaternion.identity
            };
            
            var piece1SaveData = new PieceSaveData
            {
                pieceIndex = 1,
                position = new Vector3(1, 0, 0),
                rotation = Quaternion.identity
            };
            
            var pieceSaveDataList =  new List<PieceSaveData> { piece0SaveData, piece1SaveData };

            var chunkSaveData =  new ChunkSaveData
            {
                position = Vector3.zero,
                rotation = Quaternion.identity,
                pieces = pieceSaveDataList
            };
            
            return new(
                null,
                null, 
                name, 
                "0", 
                "DK",
                new PuzzleLayout(1, 2, 2, 2, PieceShape.Rectangle, pieceCuts), 
                new List<ChunkSaveData>() {  chunkSaveData },
                new Texture2D(2, 2)
            );
        }

        public static void MakePuzzleManager()
        {
            var puzzleManager = new GameObject().AddComponent<PuzzleManager>();
            
            var puzzlePrefab = AssetDatabase.LoadAssetAtPath<Puzzle>(
                "Assets/Prefabs/Puzzle.prefab"
            );
            
            var field = typeof(PuzzleManager).GetField(
                "puzzlePrefab", 
                BindingFlags.Instance | BindingFlags.NonPublic
            );
            field.SetValue(puzzleManager, puzzlePrefab);
            
            var puzzleManagerAwakeMethod = typeof(PuzzleManager).GetMethod(
                "Awake", 
                BindingFlags.Instance | BindingFlags.NonPublic
            );
            
            puzzleManagerAwakeMethod.Invoke(puzzleManager, null);
        }
    }
}