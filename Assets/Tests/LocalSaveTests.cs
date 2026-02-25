using System.Collections;
using System.Collections.Generic;
using System.IO;
using LiteDB;
using NUnit.Framework;
using Persistence;
using PuzzleGeneration;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class LocalSaveTests
    {
        
        
        [SetUp]
        public void SetUp()
        {
            LocalSave.Initialize(Path.Combine(Application.persistentDataPath, "puzzles.db"));
        }

        [TearDown]
        public void TearDown()
        {
            LocalSave.Instance.DB.DropCollection("puzzles");
            LocalSave.Shutdown();
        }

        private PuzzleSaveData MakePuzzle(string name = "Test Puzzle") =>
            new(
                null, 
                name, 
                "A description", 
                "Author", 
                new PuzzleLayout(2, 2, PieceShape.Rectangle, new List<PieceCut>()), 
                null
                );

        [Test]
        public void Create_AssignsLocalID()
        {
            var puzzle = MakePuzzle();
            LocalSave.Instance.Create(puzzle);
            Assert.IsNotNull(puzzle.localID);
        }

        [Test]
        public void Create_CanBeLoadedBack()
        {
            var puzzle = MakePuzzle();
            LocalSave.Instance.Create(puzzle);

            var all = new List<PuzzleSaveData>(LocalSave.Instance.LoadAll());
            Assert.AreEqual(1, all.Count);
            Assert.AreEqual(puzzle.name, all[0].name);
        }

        [Test]
        public void Save_UpdatesExistingRecord()
        {
            var puzzle = MakePuzzle();
            LocalSave.Instance.Create(puzzle);

            puzzle.name = "Updated Name";
            LocalSave.Instance.Save(puzzle);

            var all = new List<PuzzleSaveData>(LocalSave.Instance.LoadAll());
            Assert.AreEqual(1, all.Count);
            Assert.AreEqual("Updated Name", all[0].name);
        }

        [Test]
        public void SaveAll_SavesMultiplePuzzles()
        {
            var puzzles = new List<PuzzleSaveData>
            {
                MakePuzzle("Puzzle A"),
                MakePuzzle("Puzzle B"),
                MakePuzzle("Puzzle C"),
            };

            LocalSave.Instance.SaveAll(puzzles);

            var all = new List<PuzzleSaveData>(LocalSave.Instance.LoadAll());
            Assert.AreEqual(3, all.Count);
        }

        [Test]
        public void Delete_RemovesPuzzle()
        {
            var puzzle = MakePuzzle();
            LocalSave.Instance.Create(puzzle);
            var allBeforeDelete = new List<PuzzleSaveData>(LocalSave.Instance.LoadAll());
            Assert.AreEqual(1, allBeforeDelete.Count);
            LocalSave.Instance.Delete(puzzle.localID);

            var allAfterDelete = new List<PuzzleSaveData>(LocalSave.Instance.LoadAll());
            Assert.AreEqual(0, allAfterDelete.Count);
        }

        [Test]
        public void LoadAll_ReturnsEmpty_WhenNoPuzzles()
        {
            var all = new List<PuzzleSaveData>(LocalSave.Instance.LoadAll());
            Assert.AreEqual(0, all.Count);
        }
    }
}