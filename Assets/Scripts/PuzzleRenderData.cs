using PuzzleGeneration;
using UnityEngine;

public class PuzzleRenderData
{
    public Texture2D PuzzleImage { get; }
    public PuzzleLayout Layout { get; }

    public PuzzleRenderData(Texture2D puzzleImage, PuzzleLayout layout)
    {
        PuzzleImage = puzzleImage;
        Layout = layout;
    }
}
