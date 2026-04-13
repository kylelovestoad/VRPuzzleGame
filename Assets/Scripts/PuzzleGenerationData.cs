using PuzzleGeneration;
using UnityEngine;

public class PuzzleGenerationData
{
    public Texture2D PuzzleImage { get; }
    public PuzzleLayout Layout { get; }

    public PuzzleGenerationData(Texture2D puzzleImage, PuzzleLayout layout)
    {
        PuzzleImage = puzzleImage;
        Layout = layout;
    }
}
