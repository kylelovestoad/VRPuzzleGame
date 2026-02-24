using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace PuzzleGeneration
{
    [Serializable]
    public class PuzzleLayout
    {
        public float width;
        public float height;
        public PieceShape shape;
        public List<PieceCut> initialPieceCuts;

        public PuzzleLayout(float width, float height, PieceShape shape, List<PieceCut> initialPieceCuts)
        {
            this.width = width;
            this.height = height;
            this.shape = shape;
            this.initialPieceCuts = initialPieceCuts;
        }
    }
}
