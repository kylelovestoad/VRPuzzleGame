using UnityEngine;

namespace PuzzleGeneration
{
    public interface IPuzzleGenerator
    {
        public PuzzleLayout Generate(Texture2D image, int numPieces, float puzzleHeight);
    }
}