using System.Collections.Generic;
using UnityEngine;

namespace PuzzleGeneration.Triangle
{
    public class Triangle
    {
        private const float MaxHorizontalShiftRatio = 0.25f;
        
        public PuzzleLayout Generate(Texture2D image, int rows, int cols, float puzzleHeight)
        {
            var widthHeightRatio = (float) image.width / image.height;
            var puzzleWidth = puzzleHeight * widthHeightRatio;
            
            var pieceHeight = puzzleHeight / rows;
            var avgPieceWidth = puzzleWidth / cols;
            
            var horizontalShiftRange = avgPieceWidth * MaxHorizontalShiftRatio;

            List<PieceCut> pieceCuts = new List<PieceCut>();
        
            for (var r = 0; r < rows; r++)
            {
                float leftBoundary = 0;

                for (var c = 0; c < cols; c++)
                {
                    
                    
                    
                    pieceCuts.Add(cut);
                
                    leftBoundary = rightBoundary;
                }
            }
        
            return new PuzzleLayout(puzzleWidth, puzzleHeight, PieceShape.Rectangle, pieceCuts);
        }

        private static PieceCut TrianglePairPieceCuts(
            int currRow,
            int currCol,
            float leftBoundary, 
            float rightBoundary, 
            float pieceHeight
        ) {
            var pieceWidth = rightBoundary - leftBoundary;
                    
            var borderPoints = TrianglePairPieceBorderPoints(
                pieceWidth,
                pieceHeight
            );
                
            var solutionLocation = new Vector3(leftBoundary, pieceHeight * currRow, 0);

            var pieceIndex = pieceCuts.Count;

            var neighbors = new List<int>();
            
            var cut = new PieceCut(pieceIndex, neighbors, solutionLocation, borderPoints);
        }

        private static (List<Vector2> left, List<Vector2> right) TrianglePairPieceBorderPoints(
            float rectangleWidth, 
            float rectangleHeight
        ) {
            var positiveSlopDiagonal = Random.Range(0, 2) == 0;

            var leftMiddleVertex = positiveSlopDiagonal
                ? new Vector2(rectangleWidth, rectangleHeight)
                : new Vector2(rectangleWidth, 0);
            
            var leftVertices = new List<Vector2>
            {
                new(0, rectangleHeight),
                leftMiddleVertex,
                new(0, 0),
            };
            
            var rightMiddleVertex = positiveSlopDiagonal
                ? new Vector2(0, 0)
                : new Vector2(0, rectangleHeight);
            
            var rightVertices = new List<Vector2>
            {
                new(rectangleWidth, 0),
                rightMiddleVertex,
                new(rectangleWidth, rectangleHeight),
            };
            
            return (leftVertices, rightVertices);
        }
    }
}
