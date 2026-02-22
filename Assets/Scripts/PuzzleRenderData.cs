using PuzzleGeneration;
using UnityEngine;

public record PuzzleRenderData(Texture2D PuzzleImage, Material BackAndSidesMaterial, PuzzleLayout Layout)
{
    public Texture2D PuzzleImage { get; } = PuzzleImage;
    public Material BackAndSidesMaterial { get; } = BackAndSidesMaterial;
    public PuzzleLayout Layout { get; } = Layout;
};
