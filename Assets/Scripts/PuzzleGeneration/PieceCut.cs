using System;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleGeneration
{
    
    [Serializable]
    public record PieceCut
    {
        public int pieceIndex;
        public int row;
        public int col;
        public List<int> neighborIndices;
        public Vector3 solutionLocation;
        public List<Vector2> borderPoints;

        public PieceCut(
            int pieceIndex, 
            int row,
            int col,
            List<int> neighborIndices, 
            Vector3 solutionLocation, 
            List<Vector2> borderPoints
        )
        {
            this.pieceIndex = pieceIndex;
            this.row = row;
            this.col = col;
            this.neighborIndices = neighborIndices;
            this.solutionLocation = solutionLocation;
            this.borderPoints = borderPoints;
        }
    }
}
