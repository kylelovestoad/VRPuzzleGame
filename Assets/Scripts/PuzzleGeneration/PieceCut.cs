using System;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleGeneration
{
    
    [Serializable]
    public record PieceCut
    {
        public int pieceIndex;
        public Vector3 solutionLocation;
        public List<Vector2> borderPoints;

        public PieceCut(int pieceIndex, Vector3 solutionLocation, List<Vector2> borderPoints)
        {
            this.pieceIndex = pieceIndex;
            this.solutionLocation = solutionLocation;
            this.borderPoints = borderPoints;
        }
    }
}
