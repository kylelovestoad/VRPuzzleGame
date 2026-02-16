using UnityEngine;

public record PuzzleRenderData(Texture2D PuzzleImage, Material BackMaterial)
{
    public Texture2D PuzzleImage { get; } = PuzzleImage;
    public Material BackMaterial { get; } = BackMaterial;
};
