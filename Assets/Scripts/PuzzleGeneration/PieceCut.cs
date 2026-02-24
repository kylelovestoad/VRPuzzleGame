using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace PuzzleGeneration
{
    
    [Serializable]
    public record PieceCut
    {
        public Vector3 solutionLocation;
        public Mesh mesh;

        public PieceCut(Vector3 solutionLocation, Mesh mesh)
        {
            this.solutionLocation = solutionLocation;
            this.mesh = mesh;   
        }
    }
}
