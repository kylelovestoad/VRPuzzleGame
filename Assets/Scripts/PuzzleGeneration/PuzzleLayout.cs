using System.Collections.Generic;

namespace PuzzleGeneration
{
    public record PuzzleLayout(float Width, float Height, PieceShape Shape, List<PieceCut> PieceCuts)
    {
        public float Width { get; } = Width;
        public float Height { get; } = Height;
        public PieceShape Shape { get; } =  Shape;
        public List<PieceCut> PieceCuts { get; } = PieceCuts;
    }
}
