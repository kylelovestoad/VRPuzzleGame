using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PuzzleGeneration
{
    public class JigsawPuzzleGenerator : IPuzzleGenerator
    {
        private const float Thickness = 0.01f;
        
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

        private static readonly Vector2[] SocketDown = ReflectSocketControlPointsY(SocketUp);
        
        private static readonly Vector2[] SocketLeft = SwappedSocketControlPoints(SocketUp);
        
        private static readonly Vector2[] SocketRight = ReflectSocketControlPointsX(SocketLeft);

        private static Vector2[] SwappedSocketControlPoints(Vector2[] controlPoints)
        {
            return controlPoints.Select(point => new Vector2(point.y, point.x)).ToArray();
        }
        
        private static Vector2[] ReflectSocketControlPointsX(Vector2[] controlPoints)
        {
            return controlPoints.Select(point => new Vector2(-point.x, point.y)).ToArray();
        }
        
        private static Vector2[] ReflectSocketControlPointsY(Vector2[] controlPoints)
        {
            return controlPoints.Select(point => new Vector2(point.x, -point.y)).ToArray();
        }
    
        public PuzzleLayout Generate(Texture2D image, int rows, int cols, float puzzleHeight)
        {
            float widthHeightRatio = (float) image.width / image.height;
            float puzzleWidth = puzzleHeight * widthHeightRatio;
            
            float pieceWidth = puzzleWidth / cols;
            float pieceHeight = puzzleHeight / rows;

            List<PieceCut> pieceCuts = new List<PieceCut>();
            List<JigsawPieceBorder> prevRowBorders = null;
        
            for (int r = 0; r < rows; r++)
            {
                float leftBoundary = 0;
                var currRowBorders = new List<JigsawPieceBorder>();

                for (int c = 0; c < cols; c++)
                {
                    float rightBoundary = pieceWidth * (c + 1);
                    Vector3 solutionLocation = new Vector3(leftBoundary, pieceHeight * r, 0);

                    JigsawPieceBorder border = new JigsawPieceBorder(
                        r == rows - 1 ? JigsawPieceEdge.Straight : JigsawPieceEdgeUtil.RandomSocket(),
                        prevRowBorders == null ? JigsawPieceEdge.Straight : prevRowBorders[c].Top.Match(),
                        c == 0 ? JigsawPieceEdge.Straight : currRowBorders[c - 1].Right.Match(),
                        c == cols - 1 ? JigsawPieceEdge.Straight : JigsawPieceEdgeUtil.RandomSocket()
                    );
                    
                    currRowBorders.Add(border);
                    
                    var mesh = JigsawPieceMesh(pieceWidth, pieceHeight, border);
                    
                    PieceCut cut = new PieceCut(solutionLocation, mesh);
                    pieceCuts.Add(cut);
                    
                    leftBoundary = rightBoundary;
                }

                prevRowBorders = currRowBorders;
            }
        
            return new PuzzleLayout(puzzleWidth, puzzleHeight, pieceCuts);
        }

        private static Mesh JigsawPieceMesh(float boxWidth, float boxHeight, JigsawPieceBorder border)
        {
            var edgePoints = new List<Vector2>();

            if (border.Top == JigsawPieceEdge.Straight)
            {
                edgePoints.Add(new Vector2(0, boxHeight));
            }
            else
            {
                Vector2[] baseControlPoints = border.Top == JigsawPieceEdge.SocketOut ? SocketUp : SocketDown;
                var controlPoints = JigsawEdgeControlPoints(baseControlPoints, new(0, boxHeight), boxWidth);
                edgePoints.AddRange(CalculateJigsawEdgePoints(controlPoints));
            }

            if (border.Right == JigsawPieceEdge.Straight)
            {
                edgePoints.Add(new Vector2(boxWidth, boxHeight));
            }
            else
            {
                Vector2[] baseControlPoints = border.Right == JigsawPieceEdge.SocketOut ? SocketRight : SocketLeft;
                var controlPoints = JigsawEdgeControlPoints(baseControlPoints, new(boxWidth, 0), boxHeight)
                    .Reverse()
                    .ToArray();
                edgePoints.AddRange(CalculateJigsawEdgePoints(controlPoints));
            }

            if (border.Bottom == JigsawPieceEdge.Straight)
            {
                edgePoints.Add(new Vector2(boxWidth, 0));
            }
            else
            {
                Vector2[] baseControlPoints = border.Bottom == JigsawPieceEdge.SocketOut ? SocketDown : SocketUp;
                var controlPoints = JigsawEdgeControlPoints(baseControlPoints, new(0, 0), boxWidth)
                    .Reverse()
                    .ToArray();
                edgePoints.AddRange(CalculateJigsawEdgePoints(controlPoints));
            }

            if (border.Left == JigsawPieceEdge.Straight)
            {
                edgePoints.Add(new Vector2(0, 0));
            }
            else
            {
                Vector2[] baseControlPoints = border.Left == JigsawPieceEdge.SocketOut ? SocketLeft : SocketRight;
                var controlPoints = JigsawEdgeControlPoints(baseControlPoints, new(0, 0), boxHeight);
                edgePoints.AddRange(CalculateJigsawEdgePoints(controlPoints));
            }

            var vertices = edgePoints.ToArray();
            var triangles = Triangulate(vertices);

            Mesh mesh = new Mesh();
            mesh.vertices = vertices.Select(v => new Vector3(v.x, v.y, 0)).ToArray();
            mesh.subMeshCount = 1;
            mesh.SetTriangles(triangles, 0);
            ConfigureUVMapping(mesh);
            
            Debug.Log("Piece Border " + string.Join(", ", edgePoints.Select(p => $"({p.x:F10}, {p.y:F10})")));

            return mesh;
        }

        private static void ConfigureUVMapping(Mesh mesh)
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
            const int pointsPerSegment = 10;
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
            
            Debug.Log("Controls " + string.Join(", ", controlPoints.Select(p => $"({p.x:F10}, {p.y:F10})")));
            Debug.Log("Piece Border " + string.Join(", ", edgePoints.Select(p => $"({p.x:F10}, {p.y:F10})")));
            
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
        
        private static int[] Triangulate(Vector2[] pieceBorder)
        {
            var verticesRemaining = Enumerable.Range(0, pieceBorder.Length).ToList();
            int[] triangles = new int[(pieceBorder.Length - 2) * 3];
            int currTriangleIndex = 0;

            while (verticesRemaining.Count > 3)
            {
                bool madeTriangle = false;

                for (int i0 = 0; i0 < verticesRemaining.Count; i0++)
                {
                    int i1 = (i0 + 1) % verticesRemaining.Count;
                    int i2 = (i0 + 2) % verticesRemaining.Count;

                    Vector2 p0 = pieceBorder[verticesRemaining[i0]];
                    Vector2 p1 = pieceBorder[verticesRemaining[i1]];
                    Vector2 p2 = pieceBorder[verticesRemaining[i2]];

                    if (!IsConvex(p0, p1, p2)) continue;

                    bool noneInside = true;
                    
                    for (int j = 0; j < verticesRemaining.Count; j++)
                    {
                        if (j == i0 || j == i1 || j == i2) continue;

                        Vector2 currPoint = pieceBorder[verticesRemaining[j]];
                        
                        if (InsideTriangle(currPoint, p0, p1, p2))
                        {
                            noneInside = false;
                            break;
                        }
                    }

                    if (noneInside)
                    {
                        triangles[currTriangleIndex++] = verticesRemaining[i0];
                        triangles[currTriangleIndex++] = verticesRemaining[i1];
                        triangles[currTriangleIndex++] = verticesRemaining[i2];
                        
                        verticesRemaining.RemoveAt(i1);
                        
                        madeTriangle = true;
                        
                        break;
                    }
                }
                
                Debug.Assert(madeTriangle, "Failed to triangulate jigsaw puzzle piece");
            }

            foreach (var vertex in verticesRemaining)
            {
                triangles[currTriangleIndex++] = vertex;
            }

            return triangles;
        }
        
        private static float CrossProduct(Vector2 p0, Vector2 p1, Vector2 p2)
        {
            return (p1.x - p0.x) * (p2.y - p1.y) - (p1.y - p0.y) * (p2.x - p1.x);
        }

        
        private static bool IsConvex(Vector2 p0, Vector2 p1, Vector2 p2)
        {
            return CrossProduct(p0, p1, p2) < 0f;
        }
        
        private static bool InsideTriangle(Vector2 point, Vector2 t0, Vector2 t1, Vector2 t2)
        {
            float dir0 = CrossProduct(t0, t1, point);
            float dir1 = CrossProduct(t1, t2, point);
            float dir2 = CrossProduct(t2, t0, point);
            
            return (dir0 < 0 && dir1 < 0 && dir2 < 0) 
                   || (dir0 > 0 && dir1 > 0 && dir2 > 0);
        }
    }
}
