using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PuzzleGeneration
{
    public class JigsawPuzzleGenerator : IPuzzleGenerator
    {
        private const float Thickness = 0.01f;
        private const float FuzzRatio = 0.25f;
        
        private static readonly Vector2[] SocketUp = {
            new(0f, 0f),
            new(0.35f, -0.15f),
            new(0.37f, -0.05f),
    
            new(0.40f, 0f),
            new(0.38f, 0.05f),
    
            new(0.20f, 0.20f),
            new(0.50f, 0.20f),
    
            new(0.80f, 0.20f),
            new(0.62f, 0.05f),
    
            new(0.60f, 0f),
            new(0.63f, -0.05f),
    
            new(0.65f, -0.15f),
            new(1f, 0f),
        };

        private static readonly Vector2[] SocketDown = ReflectSocketControlPoints(SocketUp);
        private static readonly Vector2[] SocketLeft = SwappedSocketControlPoints(SocketUp);
        private static readonly Vector2[] SocketRight = ReflectSocketControlPoints(SocketLeft);

        private static Vector2[] SwappedSocketControlPoints(Vector2[] controlPoints)
        {
            return controlPoints.Select(point => new Vector2(point.y, point.x)).ToArray();
        }
        
        private static Vector2[] ReflectSocketControlPoints(Vector2[] controlPoints)
        {
            return controlPoints.Select(point => -point).ToArray();
        }
    
        public PuzzleLayout Generate(Texture2D image, int numPieces, float puzzleHeight)
        {
            float widthHeightRatio = (float) image.width / image.height;

            int rows = CalculatePuzzleRows(numPieces, widthHeightRatio);
            int minCols = numPieces / rows;
            bool[] extraCol = RandomizeExtraCols(numPieces, rows, minCols);

            float pieceHeight = puzzleHeight / rows;
            float puzzleWidth = puzzleHeight * widthHeightRatio;

            List<PieceCut> pieceCuts = new List<PieceCut>();
        
            for (int r = 0; r < rows; r++)
            {
                int currCols = minCols + Convert.ToInt32(extraCol[r]);
                float currAvgWidth = puzzleWidth / currCols;
                float fuzzRange = currAvgWidth * FuzzRatio;
                float leftBoundary = 0;

                for (int c = 0; c < currCols; c++)
                {
                    float rightBoundary = currAvgWidth * (c + 1);
                    if (c < currCols - 1)
                    {
                        rightBoundary += Random.Range(-fuzzRange, fuzzRange);
                    }
                
                    Vector3 solutionLocation = new Vector3(leftBoundary, pieceHeight * r, 0);

                    JigsawPieceBorder boarder = new JigsawPieceBorder(
                        JigsawPieceEdge.SocketOut,
                        JigsawPieceEdge.Straight,
                        JigsawPieceEdge.Straight,
                        JigsawPieceEdge.Straight
                    );
                    
                    var mesh = JigsawPieceMesh(rightBoundary - leftBoundary, pieceHeight, boarder);
                    
                    PieceCut cut = new PieceCut(solutionLocation, mesh);
                    pieceCuts.Add(cut);
                    
                    leftBoundary = rightBoundary;
                }
            }
        
            return new PuzzleLayout(puzzleWidth, puzzleHeight, pieceCuts);
        }

        private static int CalculatePuzzleRows(int numPieces, float widthHeightRatio)
        {
            int rows = (int) Math.Round(Mathf.Sqrt(numPieces / widthHeightRatio));
            return rows == 0 ? 1 : rows;
        }

        private static bool[] RandomizeExtraCols(int numPieces, int rows, int minCols)
        {
            int extraColsRemaining = numPieces - rows * minCols;
        
            bool[] extraCol = new bool[rows];

            for (int r = 0; r < rows; r++)
            {
                int rand = Random.Range(0, rows - r);

                if (rand < extraColsRemaining)
                {
                    extraCol[r] = true;
                    extraColsRemaining--;
                }
            }
        
            return extraCol;
        }

        private static Mesh JigsawPieceMesh(float width, float height, JigsawPieceBorder border)
        {
            var edgePoints = new List<Vector2>();

            if (border.Top == JigsawPieceEdge.Straight)
            {
                edgePoints.Add(new Vector2(0, height));
            }
            else
            {
                Vector2[] baseControlPoints = border.Top == JigsawPieceEdge.SocketOut ? SocketUp : SocketDown;
                var controlPoints = JigsawEdgeControlPoints(baseControlPoints, new(0, height), width);
                edgePoints.AddRange(CalculateJigsawEdgePoints(controlPoints).SkipLast(1));
            }

            if (border.Right == JigsawPieceEdge.Straight)
            {
                edgePoints.Add(new Vector2(width, height));
            }
            else
            {
                Vector2[] baseControlPoints = border.Right == JigsawPieceEdge.SocketOut ? SocketRight : SocketLeft;
                var controlPoints = JigsawEdgeControlPoints(baseControlPoints, new(width, 0), height)
                    .Reverse()
                    .ToArray();
                edgePoints.AddRange(CalculateJigsawEdgePoints(controlPoints).SkipLast(1));
            }

            if (border.Bottom == JigsawPieceEdge.Straight)
            {
                edgePoints.Add(new Vector2(width, 0));
            }
            else
            {
                Vector2[] baseControlPoints = border.Bottom == JigsawPieceEdge.SocketOut ? SocketDown : SocketUp;
                var controlPoints = JigsawEdgeControlPoints(baseControlPoints, new(0, 0), width)
                    .Reverse()
                    .ToArray();
                edgePoints.AddRange(CalculateJigsawEdgePoints(controlPoints).SkipLast(1));
            }

            if (border.Left == JigsawPieceEdge.Straight)
            {
                edgePoints.Add(new Vector2(0, 0));
            }
            else
            {
                Vector2[] baseControlPoints = border.Left == JigsawPieceEdge.SocketOut ? SocketLeft : SocketRight;
                var controlPoints = JigsawEdgeControlPoints(baseControlPoints, new(0, 0), height);
                edgePoints.AddRange(CalculateJigsawEdgePoints(controlPoints).SkipLast(1));
            }

            var vertices = edgePoints.ToArray();
            var triangles = Triangulate(vertices);

            Mesh mesh = new Mesh();
            mesh.vertices = vertices.Select(v => new Vector3(v.x, v.y, 0)).ToArray();
            mesh.subMeshCount = 1;
            mesh.SetTriangles(triangles, 0);
            SetUVMapping(mesh);

            return mesh;
        }

        private static void SetUVMapping(Mesh mesh)
        {
            Vector3 min = mesh.bounds.min;
            Vector3 size = mesh.bounds.size;

            mesh.uv = mesh.vertices.Select(v => new Vector2(
                (v.x - min.x) / size.x,
                (v.y - min.y) / size.y
            )).ToArray();
        }
        
        private static Vector2[] JigsawEdgeControlPoints(
            Vector2[] baseControlPoints,
            Vector2 startPoint,
            float sideLength
        ) {
            return baseControlPoints
                .Select(controlPoint => startPoint + sideLength * controlPoint)
                .ToArray();
        }

        private static List<Vector2> CalculateJigsawEdgePoints(Vector2[] controlPoints)
        {
            const int pointsPerSegment = 8;
            const int segments = 6;
            
            List<Vector2> edgePoints = new List<Vector2>();

            for (int seg = 0; seg < segments; seg++)
            {
                for (int i = 0; i < pointsPerSegment; i++)
                {
                    float t = (float) i / pointsPerSegment;
                    Vector2 point = CalculateBezierCurvePoint(t, controlPoints, seg * 2);
                    
                    edgePoints.Add(point);
                }
            }
            
            return edgePoints;
        }

        private static Vector2 CalculateBezierCurvePoint(
            float t,
            Vector2[] controlPoints,
            int startIndex
        ) {
            Vector2 p0 = controlPoints[startIndex];
            Vector2 p1 = controlPoints[startIndex + 1];
            Vector2 p2 = controlPoints[startIndex + 2];

            Vector2 newP0 = Vector2.Lerp(p0, p1, t);
            Vector2 newP1 = Vector2.Lerp(p1, p2, t);

            return Vector2.Lerp(newP0, newP1, t);
        }

        private static int[] Triangulate(Vector2[] pieceBoarder)
        {
            Debug.Log("Border Length " + pieceBoarder.Length);
            
            var verticesRemaining = Enumerable.Range(0, pieceBoarder.Length).ToList();
            int[] triangles = new int[(pieceBoarder.Length - 2) * 3];
            int currTriangleIndex = 0;
            
            int i0 = 0;
            int i1 = 1;
            int i2 = 2;

            while (verticesRemaining.Count > 3)
            {
                Debug.Log("Border Length " + pieceBoarder.Length);
                
                Vector2 p0 = pieceBoarder[verticesRemaining[i0]];
                Vector2 p1 = pieceBoarder[verticesRemaining[i1]];
                Vector2 p2 = pieceBoarder[verticesRemaining[i2]];
                
                if (IsConvex(p0, p1, p2))
                {
                    int j = 0;
                    bool makeTriangle = true;
                    
                    while (j < verticesRemaining.Count)
                    {
                        if (j == i0) {
                            j += 3;
                            continue;
                        }
                        
                        Vector2 currPoint = pieceBoarder[verticesRemaining[j]];

                        if (InsideTriangle(currPoint, p0, p1, p2))
                        {
                            makeTriangle = false;
                            break;
                        }
                        
                        j++;
                    }

                    if (makeTriangle)
                    {
                        triangles[currTriangleIndex++] = verticesRemaining[i0];
                        triangles[currTriangleIndex++] = verticesRemaining[i1];
                        triangles[currTriangleIndex++] = verticesRemaining[i2];
                        
                        verticesRemaining.RemoveAt(i1);
                        continue;
                    }
                }

                i0 = i1;
                i1 = i2;
                i2 = (i2 + 1) % verticesRemaining.Count;
            }
            
            foreach (var vertex in verticesRemaining)
            {
                triangles[currTriangleIndex++] = vertex;
            }

            return triangles;
        }
        
        private static bool IsConvex(Vector2 p0, Vector2 p1, Vector2 p2)
        {
            return Cross(p0, p1, p2) < 0f;
        }
        
        private static float Cross(Vector2 p0, Vector2 p1, Vector2 p2)
        {
            return (p1.x - p0.x) * (p2.y - p1.y) - (p1.y - p0.y) * (p2.x - p1.x);
        }
        
        private static bool InsideTriangle(Vector2 point, Vector2 t0, Vector2 t1, Vector2 t2)
        {
            float d1 = Cross(t0, t1, point);
            float d2 = Cross(t1, t2, point);
            float d3 = Cross(t2, t0, point);

            bool hasNeg = (d1 < 0) || (d2 < 0) || (d3 < 0);
            bool hasPos = (d1 > 0) || (d2 > 0) || (d3 > 0);

            return !(hasNeg && hasPos);
        }
    }
}
