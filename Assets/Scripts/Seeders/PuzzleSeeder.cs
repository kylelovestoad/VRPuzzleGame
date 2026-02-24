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
            Debug.Log("dk won");
            SeedPuzzleSaveData();
        }
        
        void SeedPuzzleSaveData()
        {
            
            IPuzzleGenerator jigsawGenerator = new JigsawPuzzleGenerator();
            IPuzzleGenerator rectangleGenerator = new RectanglePuzzleGenerator();
            
            var puzzleSaves = new List<PuzzleSaveData>
            {
                new(
                    -1, 
                    "Mountain Sunset", 
                    "A serene mountain landscape at dusk.", 
                    "Alice",
                    jigsawGenerator.Generate(puzzleImage, 4, 4, 1),
                    null
                ),
                new(
                    -1, 
                    "Ocean Waves", 
                    "Crashing waves on a rocky shore.", 
                    "Bob", 
                    rectangleGenerator.Generate(puzzleImage, 1, 2, 2), 
                    null
                    ),
                new(
                    -1, 
                    "City Lights", 
                    "A sprawling city at night.", 
                    "Carol", 
                    rectangleGenerator.Generate(puzzleImage, 2, 10, 1), 
                    null
                    ),
            };   
            
            Debug.Log("dk won 1");

            
            LocalSave.Instance.Create(puzzleSaves[1]);
            Debug.Log("dk won 2");

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
            
            var puzzle = new Puzzle(
                "Donkey Kong", 
                "DK's puzzle is optimal", 
                "Donkey Kong", 
                puzzleLayout, 
                puzzleRenderData
            );

            puzzle.InitializeChunks();
        }
    }
}
