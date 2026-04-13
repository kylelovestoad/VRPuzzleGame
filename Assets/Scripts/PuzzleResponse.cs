using System;
using System.Collections.Generic;
using System.Linq;
using PuzzleGeneration;
using UnityEngine;

[Serializable]
public class PuzzleResponse
{
    public string solvedPuzzleImage;
    public List<PieceResponse> initialPieceCuts;
    public int rows;
    public int cols;
    public float puzzleWidth;
    public float puzzleHeight;
    
    // TODO: probably make utility class could see a lot of repeated
    private byte[] GetImageBytes() => Convert.FromBase64String(solvedPuzzleImage);
        
    private Texture2D PuzzleImageTexture2D()
    { 
        var imageBytes = GetImageBytes();
        
        var texture = new Texture2D(1, 1);
        texture.LoadImage(imageBytes);
        
        return texture;
    }

    public PuzzleGenerationData ToPuzzleGenerationData()
    {
        var image = PuzzleImageTexture2D();
        
        var pieceCuts = initialPieceCuts.Select(pieceResponse => pieceResponse.ToPieceCut()).ToList();

        var layout = new PuzzleLayout(
            rows, 
            cols, 
            puzzleWidth, 
            puzzleHeight, 
            PieceShape.Real, 
            pieceCuts
        );
        
        var generationData = new PuzzleGenerationData(image, layout);
        
        return generationData;
    }
}
