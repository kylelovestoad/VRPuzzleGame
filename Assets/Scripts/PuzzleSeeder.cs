using System.Collections.Generic;
using UnityEngine;

// For Testing
public class PuzzleSeeder : MonoBehaviour
{
    [SerializeField] private ChunkFactory chunkFactory;
    [SerializeField] private Texture2D puzzleImage;
    [SerializeField] private Puzzle puzzle;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Mesh pieceMesh = PieceMeshGenerator.CreateSquarePieceMesh(0.1f, 0.01f);
        Quaternion rotation = Quaternion.identity;
        
        Material backMaterial = new Material(Shader.Find("Unlit/Color"));
        backMaterial.color = Color.gray;
        
        PuzzleRenderData puzzleRenderData = new PuzzleRenderData(puzzleImage, backMaterial);
        
        Vector3 piece0Pos = new Vector3(0f, 0f, 0f);
        Vector3 piece0Sol = new Vector3(0f, 0f, 0f);
        PieceRenderData piece0RenderData = new PieceRenderData(piece0Sol, pieceMesh, puzzleRenderData);
        
        Chunk piece0  = chunkFactory.CreateSinglePieceChunk(
            piece0Pos, 
            rotation, 
            piece0RenderData
        );
        
        Vector3 piece1Pos = new Vector3(-0.4f, 0f, 0f);
        Vector3 piece1Sol = new Vector3(0f, 0.1f, 0f);
        PieceRenderData piece1RenderData = new PieceRenderData(piece1Sol, pieceMesh, puzzleRenderData);
        
        Chunk piece1 = chunkFactory.CreateSinglePieceChunk(
            piece1Pos, 
            rotation, 
            piece1RenderData
        );
        
        Vector3 piece2Pos = new Vector3(0.4f, 0.2f, 0f);
        Vector3 piece2Sol = new Vector3(0.1f, 0f, 0f);
        PieceRenderData piece2RenderData = new PieceRenderData(piece2Sol, pieceMesh, puzzleRenderData);
        
        Chunk piece2  = chunkFactory.CreateSinglePieceChunk(
            piece2Pos, 
            rotation, 
            piece2RenderData
        );
        
        Vector3 piece3Pos = new Vector3(-0.4f, 0.2f, 0f);
        Vector3 piece3Sol = new Vector3(0.1f, 0.1f, 0f);
        PieceRenderData piece3RenderData = new PieceRenderData(piece3Sol, pieceMesh, puzzleRenderData);
        
        Chunk piece3 = chunkFactory.CreateSinglePieceChunk(
            piece3Pos, 
            rotation, 
            piece3RenderData
        );
        
        Debug.Log("Generated Puzzle");
    }
}
