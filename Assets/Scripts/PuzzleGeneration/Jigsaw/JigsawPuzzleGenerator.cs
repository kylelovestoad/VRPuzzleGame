using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PuzzleGeneration.Jigsaw
{
    public class JigsawPuzzleGenerator : IPuzzleGenerator
    {
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
                        pieceWidth,
                        pieceHeight,
                        r == rows - 1 ? JigsawPieceEdgeType.Straight : JigsawPieceEdgeUtil.RandomVerticalSocket(),
                        r == 0 ? JigsawPieceEdgeType.Straight : prevRowBorders[c].Top,
                        c == 0 ? JigsawPieceEdgeType.Straight : currRowBorders[c - 1].Right,
                        c == cols - 1 ? JigsawPieceEdgeType.Straight : JigsawPieceEdgeUtil.RandomHorizontalSocket()
                    );
                    
                    currRowBorders.Add(border);
                    
                    var mesh = JigsawPieceMesh(border);
                    
                    PieceCut cut = new PieceCut(solutionLocation, mesh);
                    pieceCuts.Add(cut);
                    
                    leftBoundary = rightBoundary;
                }

                prevRowBorders = currRowBorders;
            }
        
            return new PuzzleLayout(puzzleWidth, puzzleHeight, PieceShape.Jigsaw, pieceCuts);
        }

        private static Mesh JigsawPieceMesh(JigsawPieceBorder border)
        {
            var borderPoints = new List<Vector2>();

            AddTopPoints(borderPoints, border);
            AddRightPoints(borderPoints, border);
            AddBottomPoints(borderPoints, border);
            AddLeftPoints(borderPoints, border);
            
            Debug.Log("Piece Border " + string.Join(", ", borderPoints.Select(p => $"({p.x:F10}, {p.y:F10})")));

            Mesh mesh = new Mesh();
            mesh.vertices = Vertices(borderPoints);
            
            var frontTriangles = TriangulateFront(borderPoints);
            var backTriangles = TriangulateBackAndSides(borderPoints, frontTriangles);
            
            mesh.subMeshCount = 2;
            mesh.SetTriangles(frontTriangles, 0);
            mesh.SetTriangles(backTriangles, 1);
            
            ConfigureUVMapping(mesh);
            
            return mesh;
        }

        private static void AddTopPoints(List<Vector2> borderPoints, JigsawPieceBorder border)
        {
            AddEdgePoints(
                borderPoints,
                border.Top, 
                new(0, border.BoxHeight), 
                new(border.BoxWidth, border.BoxWidth)
            );
        }
        
        private static void AddRightPoints(List<Vector2> borderPoints, JigsawPieceBorder border)
        {
            AddEdgePoints(
                borderPoints,
                border.Right,
                new(border.BoxWidth, border.BoxHeight),
                new(border.BoxHeight, -border.BoxHeight)
            );
        }
        
        private static void AddBottomPoints(List<Vector2> borderPoints, JigsawPieceBorder border)
        {
            AddEdgePoints(
                borderPoints,
                border.Bottom,
                new(border.BoxWidth, 0),
                new(-border.BoxWidth, border.BoxWidth)
            );
        }
        
        private static void AddLeftPoints(List<Vector2> borderPoints, JigsawPieceBorder border)
        {
            AddEdgePoints(
                borderPoints,
                border.Left,
                new(0, 0),
                new(border.BoxHeight, border.BoxHeight)
            );
        }
        
        private static void AddEdgePoints(
            List<Vector2> borderPoints,
            JigsawPieceEdgeType edgeType, 
            Vector2 startPoint,
            Vector2 xyScale
        ) {
            if (edgeType == JigsawPieceEdgeType.Straight)
            {
                borderPoints.Add(startPoint);
            }
            else
            {
                var baseControlPoints = edgeType.BaseControlPoints();
                
                var controlPoints = JigsawEdgeControlPoints(
                    baseControlPoints,
                    startPoint, 
                    xyScale
                );
                
                borderPoints.AddRange(CalculateJigsawEdgePoints(controlPoints));
            }
        }
        
        private static Vector2[] JigsawEdgeControlPoints(
            Vector2[] baseControlPoints,
            Vector2 startPoint,
            Vector2 xyScale
        )
        {
            var controlPoints = baseControlPoints
                .Select(controlPoint => startPoint + xyScale * controlPoint);
            
            return controlPoints.ToArray();
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
            
            // Debug.Log("Controls " + string.Join(", ", controlPoints.Select(p => $"({p.x:F10}, {p.y:F10})")));
            // Debug.Log("Piece Border " + string.Join(", ", edgePoints.Select(p => $"({p.x:F10}, {p.y:F10})")));
            
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
        
        private static Vector3[] Vertices(List<Vector2> borderPoints)
        {
            var edgePointsCount = borderPoints.Count;
            var vertices = new Vector3[borderPoints.Count << 1];
            
            for (int i = 0; i < borderPoints.Count; i++)
            {
                var point =  borderPoints[i];
                var x = point.x;
                var y = point.y;
                
                vertices[i] = new Vector3(x, y, 0);
                vertices[i + edgePointsCount] = new Vector3(x, y, IPuzzleGenerator.Thickness);
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
        
        private static int[] TriangulateFront(List<Vector2> borderPoints)
        {
            var borderPointsCounts =  borderPoints.Count;
            
            var verticesRemaining = Enumerable.Range(0, borderPointsCounts).ToList();
            int[] triangles = new int[(borderPointsCounts - 2) * 3];
            int currTriangleIndex = 0;

            while (verticesRemaining.Count > 3)
            {
                int i0 = MakeTriangle(borderPoints, verticesRemaining);
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

        private static int[] TriangulateBackAndSides(List<Vector2> borderPoints, int[] frontTriangles)
        {
            var borderPointsCount = borderPoints.Count;
            var frontTrianglesLen = frontTriangles.Length;
            
            var triangles = new int[frontTrianglesLen + borderPointsCount * 6];

            TriangulateBack(triangles, borderPoints, frontTriangles);
            TriangulateSides(triangles, borderPoints, frontTrianglesLen);
            
            return triangles;
        }

        private static void TriangulateBack(int[] triangles, List<Vector2> borderPoints, int[] frontTriangles)
        {
            var borderPointsCount = borderPoints.Count;
            var frontTrianglesLen = frontTriangles.Length;
            
            for (int i = 0; i < frontTrianglesLen; i += 3)
            {
                triangles[i] = frontTriangles[i + 2] + borderPointsCount;
                triangles[i + 1] = frontTriangles[i + 1] + borderPointsCount;
                triangles[i + 2] = frontTriangles[i] + borderPointsCount;
            }
        }
        
        private static void TriangulateSides(int[] triangles, List<Vector2> borderPoints, int startOffset)
        {
            var borderPointsCount = borderPoints.Count;
            
            for (int i = 0; i < borderPointsCount; i++)
            {
                int offset = i * 6 + startOffset;
                
                triangles[offset] = i;
                triangles[offset + 1] = i + borderPointsCount;
                triangles[offset + 2] = (i + 1) % borderPointsCount;
                
                triangles[offset + 3] = i + borderPointsCount;
                triangles[offset + 4] = (i + 1) % borderPointsCount + borderPointsCount;
                triangles[offset + 5] = (i + 1) % borderPointsCount;
            }
        }
    }
}
