using System;
using PuzzleGeneration.Hexagon;
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
        Hexagon,
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
                PieceShape.Hexagon => new HexagonPuzzleGenerator(),
                PieceShape.Real => throw new NotImplementedException(),
                _ => throw new ArgumentOutOfRangeException(nameof(shape), shape, null)
            };
        }
    }
    
}