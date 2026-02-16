using System.Collections.Generic;

namespace PuzzleGeneration
{
    public record PuzzleLayout(float Width, float Height, List<PieceCut> PieceCuts)
    {
        public float Width { get; } = Width;
        public float Height { get; } = Height;
        public List<PieceCut> PieceCuts { get; } = PieceCuts;
    }
}
