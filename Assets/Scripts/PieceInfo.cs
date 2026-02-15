using UnityEngine;

public record PieceInfo(Vector3 SolutionLocation, Mesh Mesh, PuzzleInfo PuzzleInfo)
{
    public Vector3 SolutionLocation { get; } = SolutionLocation;
    public Mesh Mesh { get; } = Mesh;
    public PuzzleInfo PuzzleInfo { get; } = PuzzleInfo;
};
