using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PuzzleGeneration
{
    public static class JigsawPuzzleGenerator
    {
        private const float Thickness = 0.01f;
        private const float FuzzRange = 0.25f;
    
        public static PuzzleLayout Generate(Texture2D image, int numPieces, float puzzleHeight)
        {
            float widthHeightRatio = (float) image.width / image.height;

            int rows = CalculatePuzzleRows(numPieces, widthHeightRatio);
            int minCols = numPieces / rows;
            bool[] extraCol = RandomizeExtraCols(numPieces, rows, minCols);

            float pieceHeight = puzzleHeight / rows;
            float puzzleWidth = puzzleHeight * widthHeightRatio;

            List<PieceCut> pieceCuts = new List<PieceCut>();
        
            for (int r = 0; r < rows; r++)
            {
                int currCols = minCols + Convert.ToInt32(extraCol[r]);
                float currAvgWidth = puzzleWidth / currCols;
                float fuzzRange = currAvgWidth * FuzzRange;
                float leftBoundary = 0;

                for (int c = 0; c < currCols; c++)
                {
                    float rightBoundary = currAvgWidth * (c + 1);
                    if (c < currCols - 1)
                    {
                        rightBoundary += Random.Range(-fuzzRange, fuzzRange);
                    }
                
                    Vector3 solutionLocation = new Vector3(leftBoundary, pieceHeight * r, 0);
                    Mesh pieceMesh = PieceMeshGenerator.CreateRectanglePieceMesh(
                        rightBoundary - leftBoundary,
                        pieceHeight,
                        Thickness
                    );
                
                    PieceCut cut = new PieceCut(rightBoundary - leftBoundary, pieceHeight, solutionLocation, pieceMesh);
                    pieceCuts.Add(cut);
                
                    leftBoundary = rightBoundary;
                }
            }
        
            return new PuzzleLayout(puzzleWidth, puzzleHeight, pieceCuts);
        }

        private static int CalculatePuzzleRows(int numPieces, float widthHeightRatio)
        {
            int rows = (int) Math.Round(Mathf.Sqrt(numPieces * widthHeightRatio));
            return rows == 0 ? 1  : rows;
        }

        private static bool[] RandomizeExtraCols(int numPieces, int rows, int minCols)
        {
            int extraColsRemaining = numPieces - rows * minCols;
        
            bool[] extraCol = new bool[rows];

            for (int r = 0; r < rows; r++)
            {
                int rand = Random.Range(0, rows - r);

                if (rand < extraColsRemaining)
                {
                    extraCol[r] = true;
                    extraColsRemaining--;
                }
            }
        
            return extraCol;
        }
    }
}
