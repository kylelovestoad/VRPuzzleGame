using PuzzleGeneration;
using UnityEngine;

public record PuzzleRenderData(Texture2D PuzzleImage, Material BackMaterial, PuzzleLayout Layout)
{
    public Texture2D PuzzleImage { get; } = PuzzleImage;
    public Material BackMaterial { get; } = BackMaterial;
    public PuzzleLayout Layout { get; } = Layout;
};
