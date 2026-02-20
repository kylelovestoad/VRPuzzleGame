using UnityEngine;

namespace PuzzleGeneration
{
    public static class PieceMeshGenerator
    {
        public static Mesh CreateRectanglePieceMesh(
            float width,
            float height,
            float thickness
        )
        {

            Vector3[] vertices =
            {
                // Front face
                new(0, 0, 0),
                new(width, 0, 0),
                new(0, height, 0),
                new(width, height, 0),

                // Back face
                new(0, 0, thickness),
                new(width, 0, thickness),
                new(0, height, thickness),
                new(width, height, thickness)
            };

            int[] frontTriangles =
            {
                0, 2, 1,
                1, 2, 3,
            };

            int[] notFrontTriangles =
            {
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
}
