using System;
using System.Collections.Generic;
using Persistence;
using PuzzleGeneration;
using PuzzleGeneration.Jigsaw;
using PuzzleGeneration.Rectangle;
using UnityEngine;

namespace Seeders
{
    public class PuzzleSeeder : MonoBehaviour
    {
        [SerializeField] private Texture2D puzzleImage;
    
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            RandomPuzzle();
            SeedPuzzleSaveData();
            // LoadPuzzleSaveData();
        }

        void LoadPuzzleSaveData()
        {
            var saves = LocalSave.Instance.LoadAll();
            foreach (var puzzle in saves)
            {
                Debug.Log("Puzzle: " + puzzle);
            }
        }
        
        void SeedPuzzleSaveData()
        {
            
            IPuzzleGenerator jigsawGenerator = new JigsawPuzzleGenerator();
            IPuzzleGenerator rectangleGenerator = new RectanglePuzzleGenerator();
            
            var puzzleSaves = new List<PuzzleSaveData>
            {
                new(
                    null, 
                    "Mountain Sunset", 
                    "A serene mountain landscape at dusk.", 
                    "Alice",
                    jigsawGenerator.Generate(puzzleImage, 4, 4, 1),
                    null
                ),
                new(
                    null, 
                    "Ocean Waves", 
                    "Crashing waves on a rocky shore.", 
                    "Bob", 
                    rectangleGenerator.Generate(puzzleImage, 1, 2, 2), 
                    null
                    ),
                new(
                    null, 
                    "City Lights", 
                    "A sprawling city at night.", 
                    "Carol", 
                    rectangleGenerator.Generate(puzzleImage, 2, 10, 1), 
                    null
                    ),
            };   
            
            // LocalSave.Instance.Create(puzzleSaves[1]);

            LocalSave.Instance.Delete("699e3b61c937df123c3c08af");
        }

        void RandomPuzzle()
        {
            IPuzzleGenerator generator = new RectanglePuzzleGenerator();
        
            var backMaterial = new Material(Shader.Find("Unlit/Color"))
            {
                color = Color.gray
            };

            var puzzleLayout = generator.Generate(puzzleImage, 4, 4, 1);
            var puzzleRenderData = new PuzzleRenderData(puzzleImage, backMaterial, puzzleLayout);

            PuzzleSaveData saveData = new(
                null,
                "Donkey Kong",
                "DK's puzzle is optimal",
                "Donkey Kong",
                puzzleLayout,
                null
            );
            
            new Puzzle(
                saveData,
                puzzleRenderData
            );
        }
    }
}
