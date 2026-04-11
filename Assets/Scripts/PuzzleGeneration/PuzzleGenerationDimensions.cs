using UnityEngine;

namespace PuzzleGeneration
{
    public class PuzzleGenerationDimensions
    {
        public readonly float PuzzleWidth;
        public readonly float WidthHeightRatio;
        public readonly float AvgPieceHeight;
        public readonly float AvgPieceWidth;
        
        public PuzzleGenerationDimensions(Texture2D image, int rows, int cols, float puzzleHeight)
        {
            WidthHeightRatio = (float) image.width / image.height;
            PuzzleWidth = puzzleHeight * WidthHeightRatio;
            
            AvgPieceHeight = puzzleHeight / rows;
            AvgPieceWidth = PuzzleWidth / cols;
        }
    }
}