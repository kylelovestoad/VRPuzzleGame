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
                    
                    var borderPoints = RectanglePieceBorderPoints(
                        pieceWidth,
                        pieceHeight
                    );
                
                    Vector3 solutionLocation = new Vector3(leftBoundary, pieceHeight * r, 0);
                    
                    PieceCut cut = new PieceCut(solutionLocation, borderPoints);
                    pieceCuts.Add(cut);
                
                    leftBoundary = rightBoundary;
                }
            }
        
            return new PuzzleLayout(puzzleWidth, puzzleHeight, PieceShape.Rectangle, pieceCuts);
        }
        
        private static List<Vector2> RectanglePieceBorderPoints(
            float width,
            float height
        ) {
            // must always be clockwise as expected when generating the actual Mesh
            var vertices = new List<Vector2>
            {
                new(0, 0),
                new(0, height),
                new(width, height),
                new(width, 0),
            };

            return vertices;
        }
    }
}
