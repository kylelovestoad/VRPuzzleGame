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
                    var rightBoundary = avgPieceWidth * (c + 1);
                    if (c < cols - 1)
                    {
                        rightBoundary += Random.Range(-horizontalShiftRange, horizontalShiftRange);
                    }

                    var pieceWidth = rightBoundary - leftBoundary;
                    
                    var borderPoints = RectanglePieceBorderPoints(
                        pieceWidth,
                        pieceHeight
                    );
                
                    var solutionLocation = new Vector3(leftBoundary, pieceHeight * r, 0);

                    var pieceIndex = pieceCuts.Count;

                    var neighbors = new List<int>();
                    
                    // TODO, probably dont want this neighborhood due to not all same size
                    if (r > 0) neighbors.Add(pieceIndex - cols);
                    if (r < rows - 1) neighbors.Add(pieceIndex + cols);
                    if (c > 0) neighbors.Add(pieceIndex - 1);
                    if (c < cols - 1) neighbors.Add(pieceIndex + 1);
                    
                    PieceCut cut = new PieceCut(pieceIndex, neighbors, solutionLocation, borderPoints);
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
