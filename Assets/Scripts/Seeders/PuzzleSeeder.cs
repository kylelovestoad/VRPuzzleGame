using System.Collections.Generic;
using System.Linq;
using Persistence;
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
        Material backMaterial = new Material(Shader.Find("Unlit/Color"));
        backMaterial.color = Color.gray;
        
        var puzzleLayout = JigsawPuzzleGenerator.Generate(puzzleImage, 4, .2f);
        var puzzleRenderData = new PuzzleRenderData(puzzleImage, backMaterial, puzzleLayout);
        
        var chunks = new List<Chunk>();
        
        foreach (var cut in puzzleLayout.PieceCuts)
        {
            Debug.Log(cut.SolutionLocation);

            chunks.Add(chunkFactory.CreateSinglePieceChunk(
                cut.SolutionLocation +
                new Vector3(cut.SolutionLocation.x * 1.5f, cut.SolutionLocation.y * 1.5f, 0),
                Quaternion.identity,
                cut,
                puzzleRenderData
            ));
        }
        
        // TODO puzzle object creation needs to be done better most likely
        var puzzleData = new PuzzleSaveData(
            0,
            null,
            "Donkey Kong",
            "DK's puzzle is optimal",
            "Donkey Kong",
            0,
            PieceShape.Square,
            chunks.Select(chunk => chunk.ToData()).ToList()
        );
        
    }
}
