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
        
        PuzzleInfo puzzleInfo = new PuzzleInfo(puzzleImage, backMaterial);
        
        Vector3 piece0Pos = new Vector3(0f, 0f, 0f);
        Vector3 piece0Sol = new Vector3(0f, 0f, 0f);
        PieceInfo piece0Info = new PieceInfo(piece0Sol, pieceMesh, puzzleInfo);
        
        Chunk piece0  = chunkFactory.CreateSinglePieceChunk(
            piece0Pos, 
            rotation, 
            piece0Info
        );
        
        Vector3 piece1Pos = new Vector3(-0.4f, 0f, 0f);
        Vector3 piece1Sol = new Vector3(0f, 0.1f, 0f);
        PieceInfo piece1Info = new PieceInfo(piece1Sol, pieceMesh, puzzleInfo);
        
        Chunk piece1 = chunkFactory.CreateSinglePieceChunk(
            piece1Pos, 
            rotation, 
            piece1Info
        );
        
        Vector3 piece2Pos = new Vector3(0.4f, 0.2f, 0f);
        Vector3 piece2Sol = new Vector3(0.1f, 0f, 0f);
        PieceInfo piece2Info = new PieceInfo(piece2Sol, pieceMesh, puzzleInfo);
        
        Chunk piece2  = chunkFactory.CreateSinglePieceChunk(
            piece2Pos, 
            rotation, 
            piece2Info
        );
        
        Vector3 piece3Pos = new Vector3(-0.4f, 0.2f, 0f);
        Vector3 piece3Sol = new Vector3(0.1f, 0.1f, 0f);
        PieceInfo piece3Info = new PieceInfo(piece3Sol, pieceMesh, puzzleInfo);
        
        Chunk piece3 = chunkFactory.CreateSinglePieceChunk(
            piece3Pos, 
            rotation, 
            piece3Info
        );
        
        puzzle.InitializeChunks(new List<Chunk> { piece0, piece1, piece2, piece3 });
        
        Debug.Log("Generated Puzzle");
    }
}
