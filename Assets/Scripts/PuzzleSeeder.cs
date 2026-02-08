using System.Collections.Generic;
using UnityEngine;

// For Testing
public class PuzzleSeeder : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // TODO: Fix Color
        
        Mesh mesh = PieceMeshGenerator.CreateSquarePieceMesh(1, 0.1f);
        
        Vector3 piece0Pos = new Vector3(4f, 0f, 0f);
        Vector3 piece0Sol = new Vector3(0f, 0f, 0f);
        Material mat0 = new Material(Shader.Find("Standard"));
        mat0.color = Color.red;
        
        Piece piece0  = Piece.Create(piece0Pos, piece0Sol, mesh, mat0);
        
        Vector3 piece1Pos = new Vector3(-4f, 0f, 0f);
        Vector3 piece1Sol = new Vector3(0f, 1f, 0f);
        Material mat1 = new Material(Shader.Find("Standard"));
        mat1.color = Color.blue;
        
        Piece piece1 = Piece.Create(piece1Pos, piece1Sol, mesh, mat1);
        
        Vector3 piece2Pos = new Vector3(4f, 2f, 0f);
        Vector3 piece2Sol = new Vector3(1f, 0f, 0f);
        Material mat2 = new Material(Shader.Find("Standard"));
        mat0.color = Color.darkGreen;
        
        Piece piece2  = Piece.Create(piece2Pos, piece2Sol, mesh, mat2);
        
        Vector3 piece3Pos = new Vector3(-4f, 2f, 0f);
        Vector3 piece3Sol = new Vector3(1f, 1f, 0f);
        Material mat3 = new Material(Shader.Find("Standard"));
        mat1.color = Color.purple;
        
        Piece piece3 = Piece.Create(piece3Pos, piece3Sol, mesh, mat3);
        
        new Puzzle(new List<Piece> { piece0, piece1, piece2, piece3 });
        
        Debug.Log("Generated Puzzle");
    }
}
