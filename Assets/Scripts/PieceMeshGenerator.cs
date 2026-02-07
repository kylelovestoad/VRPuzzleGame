using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class PieceMeshGenerator : MonoBehaviour
{
    Mesh mesh;
    
    Vector3[] vertices;
    int[] triangles;
    
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateSquarePiece(1, 0.1f);
    }

    // TODO: make more general location
    void CreateSquarePiece(
        float size,
        float thickness
    ) {
        vertices = new Vector3[]
        {
            // Front face
            new Vector3(0, 0, 0),
            new Vector3(size, 0, 0),
            new Vector3(0, size, 0),
            new Vector3(size, size, 0),

            // Back face
            new Vector3(0, 0, thickness),
            new Vector3(size, 0, thickness),
            new Vector3(0, size, thickness),
            new Vector3(size, size, thickness)
        };

        triangles = new int[]
        {
            // Front face
            0, 2, 1,
            1, 2, 3,

            // Back face
            5, 6, 4,
            7, 6, 5,

            // Left face
            4, 6, 0,
            0, 6, 2,

            // Right face
            1, 3, 5,
            5, 3, 7,

            // Bottom face
            4, 0, 5,
            5, 0, 1,

            // Top face
            2, 6, 3,
            3, 6, 7
        };
        
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        
        print("Created Rectangle Puzzle Piece");
    }
}