using System;
using System.Collections.Generic;
using PuzzleGeneration;
using UnityEngine;

[Serializable]
public class PieceResponse
{
    public int pieceIndex;
    public List<int> neighborIndices;
    public Vector3 solutionLocation;
    public List<Vector2> borderPoints;
    
    public PieceCut ToPieceCut()
    {
        // TODO: fix row and col
        return new PieceCut(pieceIndex, 0, 0, neighborIndices, solutionLocation, borderPoints);
    }
}
