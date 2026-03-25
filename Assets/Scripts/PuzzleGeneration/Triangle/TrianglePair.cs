using System.Collections.Generic;
using UnityEngine;

namespace PuzzleGeneration.Triangle
{
    public record TrianglePair(
        List<Vector2> LeftVertices,
        List<Vector2> RightVertices,
        bool PositiveSlopingDiagonal
    ) {
        public List<Vector2> LeftVertices { get; } = LeftVertices;
        public List<Vector2> RightVertices { get; } = RightVertices;
        public bool PositiveSlopingDiagonal { get; } = PositiveSlopingDiagonal;
    }
}
