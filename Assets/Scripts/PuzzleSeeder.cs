using System.Collections.Generic;
using PuzzleGeneration;
using UnityEngine;

// For Testing
public class PuzzleSeeder : MonoBehaviour
{
    [SerializeField] private ChunkFactory chunkFactory;
    [SerializeField] private Texture2D puzzleImage;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RandomPuzzle();
    }

    void RandomPuzzle()
    {
        JigsawPuzzleGenerator generator =  new JigsawPuzzleGenerator();
            
        Material backMaterial = new Material(Shader.Find("Unlit/Color"));
        backMaterial.color = Color.gray;
        
        var puzzleLayout = generator.Generate(puzzleImage, 9, 1);
        var puzzleRenderData = new PuzzleRenderData(puzzleImage, backMaterial, puzzleLayout);
        
        foreach (var cut in puzzleLayout.PieceCuts)
        {
            Debug.Log(cut.SolutionLocation);
            
            chunkFactory.CreateSinglePieceChunk(
                cut.SolutionLocation + 
                new Vector3(cut.SolutionLocation.x * 1.5f, cut.SolutionLocation.y * 1.5f, 0), 
                Quaternion.identity, 
                cut,
                puzzleRenderData
            );
        }
    }
}
