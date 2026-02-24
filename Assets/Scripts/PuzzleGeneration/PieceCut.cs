using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace PuzzleGeneration
{
    
    [Serializable]
    public record PieceCut
    {
        public Vector3 solutionLocation;
        public List<Vector2> borderPoints;

        public PieceCut(Vector3 solutionLocation, List<Vector2> borderPoints)
        {
            this.solutionLocation = solutionLocation;
            this.borderPoints = borderPoints;
        }
    }
}
