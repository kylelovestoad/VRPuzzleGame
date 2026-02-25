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

                    System.Diagnostics.Debug.Assert(prevRowBorders != null, nameof(prevRowBorders) + " != null");
                    
                    JigsawPieceBorder border = new JigsawPieceBorder(
                        pieceWidth,
                        pieceHeight,
                        r == rows - 1 ? JigsawPieceEdgeType.Straight : JigsawPieceEdgeUtil.RandomVerticalSocket(),
                        r == 0 ? JigsawPieceEdgeType.Straight : prevRowBorders[c].Top,
                        c == 0 ? JigsawPieceEdgeType.Straight : currRowBorders[c - 1].Right,
                        c == cols - 1 ? JigsawPieceEdgeType.Straight : JigsawPieceEdgeUtil.RandomHorizontalSocket()
                    );
                    
                    currRowBorders.Add(border);
                    
                    var borderPoints = JigsawPieceBorderPoints(border);
                    
                    int nextIndex = pieceCuts.Count;
                    PieceCut cut = new PieceCut(nextIndex, solutionLocation, borderPoints);
                    pieceCuts.Add(cut);
                    
                    leftBoundary = rightBoundary;
                }

                prevRowBorders = currRowBorders;
            }
        
            return new PuzzleLayout(puzzleWidth, puzzleHeight, PieceShape.Jigsaw, pieceCuts);
        }

        private static List<Vector2> JigsawPieceBorderPoints(JigsawPieceBorder border)
        {
            var borderPoints = new List<Vector2>();

            AddTopPoints(borderPoints, border);
            AddRightPoints(borderPoints, border);
            AddBottomPoints(borderPoints, border);
            AddLeftPoints(borderPoints, border);

            return borderPoints;
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
    }
}
