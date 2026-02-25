using UnityEngine;

namespace PuzzleGeneration
{
    public interface IPuzzleGenerator
    {
        public const float Thickness = 0.00f;
        
        public PuzzleLayout Generate(Texture2D image, int rows, int cols, float puzzleHeight);
    }
}
