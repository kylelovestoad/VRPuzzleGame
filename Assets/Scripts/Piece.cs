using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Piece : MonoBehaviour
{
    private Vector3 solutionLocation;

    public static Piece Create(Vector3 initialPosition, Mesh mesh)
    {
        GameObject pieceObject = new GameObject("Piece");
        
        MeshFilter meshFilter = pieceObject.AddComponent<MeshFilter>();
        pieceObject.AddComponent<MeshRenderer>();
        meshFilter.mesh = mesh;
        
        pieceObject.transform.position = initialPosition;
        
        Piece piece = pieceObject.AddComponent<Piece>();
        
        return piece;
    }

    public Vector3[] Verticies()
    {
        return gameObject
            .GetComponent<MeshFilter>()
            .mesh
            .vertices
            .Select(vertex => transform.TransformPoint(vertex))
            .ToArray();
    }
}
