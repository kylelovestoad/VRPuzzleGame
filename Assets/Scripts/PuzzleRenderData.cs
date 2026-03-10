using PuzzleGeneration;
using UnityEngine;

public class PuzzleRenderData
{
    private static readonly Material DefaultBackMaterial = new(Shader.Find("Unlit/Color"))
    {
        color = Color.gray
    };

    public Texture2D PuzzleImage { get; }
    public Material BackAndSidesMaterial { get; }
    public PuzzleLayout Layout { get; }

    public PuzzleRenderData(Texture2D puzzleImage, PuzzleLayout layout, Material backAndSidesMaterial = null)
    {
        PuzzleImage = puzzleImage;
        Layout = layout;
        BackAndSidesMaterial = backAndSidesMaterial ?? DefaultBackMaterial;
    }
}
