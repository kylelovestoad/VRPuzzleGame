using UnityEngine;

namespace PuzzleGeneration
{
    public record PieceCut(Vector3 SolutionLocation, Mesh Mesh)
    {
        public Vector3 SolutionLocation { get; } = SolutionLocation;
        public Mesh Mesh { get; } = Mesh;
    }
}
