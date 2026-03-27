using System;
using UnityEngine;

namespace PuzzleGeneration
{
    public interface IPuzzleGenerator
    {
        public const float Thickness = 0.01f;
        
        public void Generate(Texture2D image, int rows, int cols, float puzzleHeight, Action<PuzzleRenderData> onComplete);
    }
}
