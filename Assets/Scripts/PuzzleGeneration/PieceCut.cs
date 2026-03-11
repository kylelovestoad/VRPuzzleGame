using System;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleGeneration
{
    
    [Serializable]
    public record PieceCut
    {
        public int pieceIndex;
        public List<int> neighborIndices;
        public Vector3 solutionLocation;
        public List<Vector2> borderPoints;

        public PieceCut(int pieceIndex, List<int> neighborIndices, Vector3 solutionLocation, List<Vector2> borderPoints)
        {
            this.pieceIndex = pieceIndex;
            this.neighborIndices = neighborIndices;
            this.solutionLocation = solutionLocation;
            this.borderPoints = borderPoints;
        }
    }
}
