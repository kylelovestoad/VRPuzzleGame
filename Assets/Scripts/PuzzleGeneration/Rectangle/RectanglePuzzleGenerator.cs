using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PuzzleGeneration.Rectangle
{
    public class RectanglePuzzleGenerator : IPuzzleGenerator
    {
        private const float MaxHorizontalShiftRatio = 0.25f;
    
        public PuzzleLayout Generate(Texture2D image, int rows, int cols, float puzzleHeight)
        {
            float widthHeightRatio = (float) image.width / image.height;
            float puzzleWidth = puzzleHeight * widthHeightRatio;
            
            float pieceHeight = puzzleHeight / rows;
            float avgPieceWidth = widthHeightRatio * pieceHeight;
            
            float horizontalShiftRange = avgPieceWidth * MaxHorizontalShiftRatio;

            List<PieceCut> pieceCuts = new List<PieceCut>();
        
            for (int r = 0; r < rows; r++)
            {
                float leftBoundary = 0;

                for (int c = 0; c < cols; c++)
                {
                    float rightBoundary = avgPieceWidth * (c + 1);
                    if (c < cols - 1)
                    {
                        rightBoundary += Random.Range(-horizontalShiftRange, horizontalShiftRange);
                    }

                    float pieceWidth = rightBoundary - leftBoundary;
                    
                    Mesh pieceMesh = RectanglePieceMesh(
                        pieceWidth,
                        pieceHeight,
                        IPuzzleGenerator.Thickness
                    );
                
                    Vector3 solutionLocation = new Vector3(leftBoundary, pieceHeight * r, 0);
                    
                    PieceCut cut = new PieceCut(solutionLocation, pieceMesh);
                    pieceCuts.Add(cut);
                
                    leftBoundary = rightBoundary;
                }
            }
        
            return new PuzzleLayout(puzzleWidth, puzzleHeight, pieceCuts);
        }
        
        private static Mesh RectanglePieceMesh(
            float width,
            float height,
            float thickness
        ) {
            const int totalVertices = 8;
            const int frontVertices = 4;
            
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

            int[] backAndSideTriangles =
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

            Vector2[] uv = new Vector2[totalVertices];

            for (int i = 0; i < frontVertices; i++)
            {
                Vector3 vertex = vertices[i];
                uv[i] = new Vector2(vertex.x / width, vertex.y / height);
            }

            Mesh mesh = new Mesh();

            mesh.vertices = vertices;
            mesh.uv = uv;

            mesh.subMeshCount = 2;
            mesh.SetTriangles(frontTriangles, 0);
            mesh.SetTriangles(backAndSideTriangles, 1);

            return mesh;
        }
    }
}
