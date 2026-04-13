using System;
using System.Threading.Tasks;
using UnityEngine;

namespace PuzzleGeneration
{
    public interface IPuzzleGenerator
    {
        public const float Thickness = 0.01f;
        
        public Task<PuzzleGenerationData> Generate(Texture2D image, int rows, int cols, float puzzleHeight);
    }
}
