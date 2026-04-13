using PuzzleGeneration;
using UnityEngine;

namespace UI
{
    public class PuzzleForm
    {
        public readonly string Name;
        public readonly PieceShape Shape;
        public readonly int Rows;
        public readonly int Columns;
        public readonly Texture2D PuzzleImage;

        public PuzzleForm(string name, PieceShape shape, int rows, int columns, Texture2D puzzleImage)
        {
            Name = name;
            Shape = shape;
            Rows = rows;
            Columns = columns;
            PuzzleImage = puzzleImage;
        }
    }
}