using PuzzleGeneration;
using PuzzleGeneration.Jigsaw;
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
        }

        void RandomPuzzle()
        {
            IPuzzleGenerator generator = new JigsawPuzzleGenerator();
        
            Material backMaterial = new Material(Shader.Find("Unlit/Color"));
            backMaterial.color = Color.gray;
        
            var puzzleLayout = generator.Generate(puzzleImage, 4, 4, 1);
            var puzzleRenderData = new PuzzleRenderData(puzzleImage, backMaterial, puzzleLayout);

            new Puzzle(
                "Donkey Kong", 
                "DK's puzzle is optimal", 
                "Donkey Kong", 
                puzzleLayout, 
                puzzleRenderData
            );
        }
    }
}
