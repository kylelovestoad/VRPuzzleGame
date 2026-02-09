using System.Collections.Generic;
using UnityEngine;

// For Testing
public class PuzzleSeeder : MonoBehaviour
{
    [SerializeField] 
    private PieceFactory pieceFactory;
    [SerializeField] 
    private Puzzle puzzle;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // TODO: Fix Color
        
        Mesh mesh = PieceMeshGenerator.CreateSquarePieceMesh(0.1f, 0.01f);
        Quaternion rotation = Quaternion.identity;
        
        Vector3 piece0Pos = new Vector3(0f, 0f, 0f);
        Vector3 piece0Sol = new Vector3(0f, 0f, 0f);
        Material mat0 = new Material(Shader.Find("Standard"));
        mat0.color = Color.red;
        
        Piece piece0  = pieceFactory.CreatePiece(
            piece0Pos, 
            rotation, 
            piece0Sol, 
            mesh, 
            mat0
        );
        
        Vector3 piece1Pos = new Vector3(-0.4f, 0f, 0f);
        Vector3 piece1Sol = new Vector3(0f, 0.1f, 0f);
        Material mat1 = new Material(Shader.Find("Standard"));
        mat1.color = Color.blue;
        
        Piece piece1 = pieceFactory.CreatePiece(
            piece1Pos, 
            rotation, 
            piece1Sol, 
            mesh, 
            mat1
        );
        
        Vector3 piece2Pos = new Vector3(0.4f, 0.2f, 0f);
        Vector3 piece2Sol = new Vector3(0.1f, 0f, 0f);
        Material mat2 = new Material(Shader.Find("Standard"));
        mat0.color = Color.darkGreen;
        
        Piece piece2  = pieceFactory.CreatePiece(
            piece2Pos, 
            rotation, 
            piece2Sol, 
            mesh, 
            mat2
        );
        
        Vector3 piece3Pos = new Vector3(-0.4f, 0.2f, 0f);
        Vector3 piece3Sol = new Vector3(0.1f, 0.1f, 0f);
        Material mat3 = new Material(Shader.Find("Standard"));
        mat1.color = Color.purple;
        
        Piece piece3 = pieceFactory.CreatePiece(
            piece3Pos, 
            rotation, 
            piece3Sol,
            mesh, 
            mat3
        );
        
        puzzle.InitializeChunks(new List<Piece> { piece0, piece1, piece2, piece3 });
        
        Debug.Log("Generated Puzzle");
    }
}
