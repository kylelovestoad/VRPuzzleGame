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
    
    // TODO: probably make utility class could see a lot of repeated
    private byte[] GetImageBytes() => Convert.FromBase64String(solvedPuzzleImage);
        
    private Texture2D PuzzleImageTexture2D()
    { 
        var imageBytes = GetImageBytes();
        
        var texture = new Texture2D(1, 1);
        texture.LoadImage(imageBytes);
        
        return texture;
    }

    public PuzzleRenderData ToPuzzleRenderData()
    {
        var image = PuzzleImageTexture2D();
        
        var imageWidth = image.width;
        var imageHeight = image.height;
        
        var pieceCuts = initialPieceCuts.Select(pieceResponse => pieceResponse.ToPieceCut()).ToList();
        var layout = new PuzzleLayout(0.3f, 0.3f * imageHeight / imageWidth, PieceShape.Real, pieceCuts);
        
        var renderData = new PuzzleRenderData(image, layout);
        
        return renderData;
    }
}
