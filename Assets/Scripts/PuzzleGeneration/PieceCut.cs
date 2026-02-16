using UnityEngine;

namespace PuzzleGeneration
{
    public record PieceCut(float Width, float Height, Vector3 SolutionLocation, Mesh Mesh)
    {
        public float Width { get; } = Width;
        public float Height { get; } = Height;
        public Vector3 SolutionLocation { get; } = SolutionLocation;
        public Mesh Mesh { get; } = Mesh;
    }
}
