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
        // TODO: fix row and col
        var layout = new PuzzleLayout(0, 0, puzzleWidth, puzzleHeight, PieceShape.Real, pieceCuts);
        
        var renderData = new PuzzleGenerationData(image, layout);
        
        return renderData;
    }
}
