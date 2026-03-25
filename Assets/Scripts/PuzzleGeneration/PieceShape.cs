using System;
using PuzzleGeneration.Jigsaw;
using PuzzleGeneration.Rectangle;
using PuzzleGeneration.Triangle;

namespace PuzzleGeneration
{
    public enum PieceShape
    {
        Rectangle,
        Jigsaw,
        Triangle,
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
                PieceShape.Triangle => new TrianglePuzzleGenerator(),
                _ => throw new ArgumentOutOfRangeException(nameof(shape), shape, null)
            };
        }
    }
    
}