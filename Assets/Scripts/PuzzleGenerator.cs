using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class PuzzleGenerator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Mesh mesh = PieceMeshGenerator.CreateSquarePieceMesh(1, 0.1f);
        
        Vector3 piece0Pos = new Vector3(10f, 0f, 0f);
        Piece piece0  = Piece.Create(piece0Pos, mesh);
        
        Vector3 piece1Pos = new Vector3(-10f, 0f, 0f);
        Piece piece1 = Piece.Create(piece1Pos, mesh);
        
        new Puzzle(new List<Piece> { piece0, piece1 });
        
        Debug.Log("Generated Puzzle");
    }
}
