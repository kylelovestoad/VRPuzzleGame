using UnityEngine;

public record PieceRenderData(Vector3 SolutionLocation, Mesh Mesh, PuzzleRenderData PuzzleRenderData)
{
    public Vector3 SolutionLocation { get; } = SolutionLocation;
    public Mesh Mesh { get; } = Mesh;
    public PuzzleRenderData PuzzleRenderData { get; } = PuzzleRenderData;
};
