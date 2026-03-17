using System;
using PuzzleGeneration.Jigsaw;
using PuzzleGeneration.Rectangle;

namespace PuzzleGeneration
{
    public enum PieceShape
    {
        Rectangle,
        Jigsaw,
        Real
    }

    public static class PieceShapeExtensions
    {
        public static IPuzzleGenerator Generator(this PieceShape shape)
        {
            return shape switch
            {
                PieceShape.Rectangle => new RectanglePuzzleGenerator(),
                PieceShape.Jigsaw => new JigsawPuzzleGenerator(),
                _ => throw new ArgumentOutOfRangeException(nameof(shape), shape, null)
            };
        }
    }
    
}