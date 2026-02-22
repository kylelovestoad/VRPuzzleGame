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

            Mesh mesh = new Mesh();
            mesh.vertices = Vertices(edgePoints);
            
            var frontTriangles = TriangulateFront(edgePoints);
            var backTriangles = TriangulateBackAndSides(edgePoints, frontTriangles);
            
            mesh.subMeshCount = 2;
            mesh.SetTriangles(frontTriangles, 0);
            mesh.SetTriangles(backTriangles, 1);
            
            ConfigureUVMapping(mesh);
            
            return mesh;
        }

        private static Vector3[] Vertices(List<Vector2> edgePoints)
        {
            var edgePointsCount = edgePoints.Count;
            var vertices = new Vector3[edgePoints.Count << 1];
            
            for (int i = 0; i < edgePoints.Count; i++)
            {
                var point =  edgePoints[i];
                var x = point.x;
                var y = point.y;
                
                vertices[i] = new Vector3(x, y, 0);
                vertices[i + edgePointsCount] = new Vector3(x, y, Thickness);
            }

            return vertices;
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
        
        private static int[] TriangulateFront(List<Vector2> edgePieces)
        {
            var edgePointsCounts =  edgePieces.Count;
            
            var verticesRemaining = Enumerable.Range(0, edgePointsCounts).ToList();
            int[] triangles = new int[(edgePointsCounts - 2) * 3];
            int currTriangleIndex = 0;

            while (verticesRemaining.Count > 3)
            {
                int i0 = MakeTriangle(edgePieces, verticesRemaining);
                Debug.Assert(i0 >= 0, "Failed to triangulate jigsaw puzzle piece");
                
                int i1 = (i0 + 1) % verticesRemaining.Count;
                int i2 = (i0 + 2) % verticesRemaining.Count;
                
                triangles[currTriangleIndex++] = verticesRemaining[i0];
                triangles[currTriangleIndex++] = verticesRemaining[i1];
                triangles[currTriangleIndex++] = verticesRemaining[i2];
                    
                verticesRemaining.RemoveAt(i1);
            }

            foreach (var vertex in verticesRemaining)
            {
                triangles[currTriangleIndex++] = vertex;
            }

            return triangles;
        }

        private static int MakeTriangle(List<Vector2> pieceBorder, List<int> verticesRemaining)
        {
            var verticesRemainingCount =  verticesRemaining.Count;
            
            for (int i0 = 0; i0 < verticesRemainingCount; i0++)
            {
                int i1 = (i0 + 1) % verticesRemainingCount;
                int i2 = (i0 + 2) % verticesRemainingCount;
                
                bool noneInside = ValidTriangle(i0, i1, i2, pieceBorder, verticesRemaining);

                if (noneInside)
                {
                    return i0;
                }
            }

            return -1;
        }

        private static bool ValidTriangle(int i0, int i1, int i2, List<Vector2> pieceBorder, List<int> verticesRemaining)
        {
            Vector2 p0 = pieceBorder[verticesRemaining[i0]];
            Vector2 p1 = pieceBorder[verticesRemaining[i1]];
            Vector2 p2 = pieceBorder[verticesRemaining[i2]];

            if (IsConcave(p0, p1, p2)) return false;
            
            for (int j = 0; j < verticesRemaining.Count; j++)
            {
                if (j == i0 || j == i1 || j == i2) continue;

                Vector2 currPoint = pieceBorder[verticesRemaining[j]];
                        
                if (InsideTriangle(currPoint, p0, p1, p2))
                {
                    return false;
                }
            }

            return true;
        }
        
        private static float CrossProduct(Vector2 p0, Vector2 p1, Vector2 p2)
        {
            return (p1.x - p0.x) * (p2.y - p1.y) - (p1.y - p0.y) * (p2.x - p1.x);
        }

        
        private static bool IsConcave(Vector2 p0, Vector2 p1, Vector2 p2)
        {
            return CrossProduct(p0, p1, p2) > 0f;
        }
        
        private static bool InsideTriangle(Vector2 point, Vector2 p0, Vector2 p1, Vector2 p2)
        {
            float dir0 = CrossProduct(p0, p1, point);
            float dir1 = CrossProduct(p1, p2, point);
            float dir2 = CrossProduct(p2, p0, point);
            
            return (dir0 < 0 && dir1 < 0 && dir2 < 0) 
                   || (dir0 > 0 && dir1 > 0 && dir2 > 0);
        }

        private static int[] TriangulateBackAndSides(List<Vector2> edgePieces, int[] frontTriangles)
        {
            var edgePiecesCount = edgePieces.Count;
            var frontTrianglesLen = frontTriangles.Length;
            
            var triangles = new int[frontTrianglesLen + edgePiecesCount * 6];

            TriangulateBack(triangles, edgePieces, frontTriangles);
            TriangulateSides(triangles, edgePieces, frontTrianglesLen);
            
            return triangles;
        }

        private static void TriangulateBack(int[] triangles, List<Vector2> edgePieces, int[] frontTriangles)
        {
            var edgePiecesCount = edgePieces.Count;
            var frontTrianglesLen = frontTriangles.Length;
            
            for (int i = 0; i < frontTrianglesLen; i += 3)
            {
                triangles[i] = frontTriangles[i + 2] + edgePiecesCount;
                triangles[i + 1] = frontTriangles[i + 1] + edgePiecesCount;
                triangles[i + 2] = frontTriangles[i] + edgePiecesCount;
            }
        }
        
        private static void TriangulateSides(int[] triangles, List<Vector2> edgePieces, int startOffset)
        {
            var edgePiecesCount = edgePieces.Count;
            
            for (int i = 0; i < edgePiecesCount; i++)
            {
                int offset = i * 6 + startOffset;
                
                triangles[offset] = i;
                triangles[offset + 1] = i + edgePiecesCount;
                triangles[offset + 2] = (i + 1) % edgePiecesCount;
                
                triangles[offset + 3] = i + edgePiecesCount;
                triangles[offset + 4] = (i + 1) % edgePiecesCount + edgePiecesCount;
                triangles[offset + 5] = (i + 1) % edgePiecesCount;
            }
        }
    }
}
