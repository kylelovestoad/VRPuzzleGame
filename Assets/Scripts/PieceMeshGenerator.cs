using UnityEngine;

public class PieceMeshGenerator : MonoBehaviour
{
    public static Mesh CreateSquarePieceMesh(
        float size,
        float thickness
    ) {
        Vector3[] vertices = {
            // Front face
            new(0, 0, 0),
            new(size, 0, 0),
            new(0, size, 0),
            new(size, size, 0),

            // Back face
            new(0, 0, thickness),
            new(size, 0, thickness),
            new(0, size, thickness),
            new(size, size, thickness)
        };

        int[] frontTriangles = {
            0, 2, 1,
            1, 2, 3,
        };

        int[] notFrontTriangles = {
            // // Front face
            // 0, 2, 1,
            // 1, 2, 3,

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
        
        // Mesh mesh = new Mesh();
        // mesh.vertices = vertices;
        // mesh.triangles = triangles;
        
        Vector2[] uv = new Vector2[8];
        
        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 1);
        
        Mesh mesh = new Mesh();
        
        mesh.vertices = vertices;
        mesh.uv = uv;
        
        mesh.subMeshCount = 2;
        mesh.SetTriangles(frontTriangles, 0);
        mesh.SetTriangles(notFrontTriangles, 1);
        
        return mesh;
    }
}
