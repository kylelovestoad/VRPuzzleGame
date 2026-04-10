using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace PuzzleGeneration.Hexagon
{
    public class HexagonPuzzleGenerator: IPuzzleGenerator
    {
        public async Task<PuzzleGenerationData> Generate(
            Texture2D image, 
            int rows, 
            int cols, 
            float puzzleHeight
        ) {
            var dimensions = new PuzzleGenerationDimensions(image, rows, cols, puzzleHeight);
            var pieceHeight = dimensions.PieceHeight;
            var pieceWidth = dimensions.AvgPieceWidth;
            
            var pieceCuts = new List<PieceCut>();
        
            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < cols; c++)
                {
                    AddPieceCut(pieceCuts, pieceWidth, pieceHeight, r, c, rows, cols);
                }

                if ((r & 1) == 0)
                {
                    AddEvenRowEndPiece(pieceCuts, pieceWidth, pieceHeight, r, rows, cols);
                }
            }
        
            var layout = new PuzzleLayout(
                rows, 
                cols + 1, 
                dimensions.PuzzleWidth, 
                puzzleHeight, 
                PieceShape.Hexagon, 
                pieceCuts
            );
            var renderData = new PuzzleGenerationData(image, layout);

            return renderData;
        }

        private static void AddPieceCut(
            List<PieceCut> pieceCuts,
            float pieceWidth,
            float pieceHeight,
            int r,
            int c,
            int rows,
            int cols
        )
        {
            var solutionLocation = new Vector3(pieceWidth * c, pieceHeight * r);
            var pieceIndex = pieceCuts.Count;

            var (neighbors, borderPoints) = (r & 1) == 0
                ? (EvenRowNeighbors(r, c, rows, cols, pieceIndex), 
                    EvenRowBorderPoints(pieceWidth, pieceHeight, r, c, rows))
                : (OddRowNeighbors(r, c, rows, cols, pieceIndex), 
                    OddRowBorderPoints(pieceWidth, pieceHeight, r, rows));
                    
            var cut = new PieceCut(pieceIndex, neighbors, solutionLocation, borderPoints);
            pieceCuts.Add(cut);
        }
        
        private static List<Vector2> OddRowBorderPoints(float width,
            float height,
            int row,
            int rows
        )
        {
            var vertices = new List<Vector2>
            {
                new(width, height / 2),
                new(width, 0),
                new(width / 2, -height / 2),
                new(0, 0),
                new(0, height / 2),
            };

            if (row < rows - 1)
            {
                vertices.Add(new(width / 2, height));
            }

            return vertices;
        }
        
        private static List<Vector2> EvenRowBorderPoints(
            float width,
            float height,
            int row,
            int col,
            int rows
        )
        {
            return col == 0 
                ? EvenRowFirstColBorderPoints(width, height, row, rows) 
                : EvenRowNotFirstColBorderPoints(width, height, row, rows);
        }

        private static List<Vector2> EvenRowFirstColBorderPoints(
            float width,
            float height,
            int row,
            int rows
        )
        {
            var vertices = new List<Vector2>
            {
                new(width / 2, height / 2),
                new(width / 2, 0)
            };

            if (row == 0)
            {
                vertices.Add(new(0, 0));
                vertices.Add(new(0, height));
            }
            else if (row == rows - 1)
            {
                vertices.Add(new(0, -height / 2));
                vertices.Add(new(0, height / 2));
            }
            else
            {
                vertices.Add(new(0, -height / 2));
                vertices.Add(new(0, height));
            }
            
            return vertices;
        }
        
        private static List<Vector2> EvenRowNotFirstColBorderPoints(float width,
            float height,
            int row,
            int rows
        )
        {
            var vertices = new List<Vector2>
            {
                new(-width / 2, 0),
                new(-width / 2, height / 2)
            };

            if (row < rows - 1)
            {
                vertices.Add(new(0, height));
            }
                    
            vertices.Add(new(width / 2, height / 2));
            vertices.Add(new(width / 2, 0));

            if (row > 0)
            {
                vertices.Add(new(0, -height / 2));
            }
            
            return vertices;
        }

        private static void AddEvenRowEndPiece(
            List<PieceCut> pieceCuts, 
            float pieceWidth, 
            float pieceHeight, 
            int r,
            int rows,
            int cols
        )
        {
            var solutionLocation = new Vector3(pieceWidth * cols, pieceHeight * r);

            var pieceIndex = pieceCuts.Count;
            
            var neighbors = new List<int> { pieceIndex - 1 };

            var vertices = new List<Vector2>
            {
                new(-pieceWidth / 2, 0),
                new(-pieceWidth / 2, pieceHeight / 2)
            };

            if (r == rows - 1)
            {
                vertices.Add(new(0, pieceHeight / 2));
            }
            else
            {
                neighbors.Add(pieceIndex + cols);
                vertices.Add(new(0, pieceHeight));
            }

            if (r == 0)
            {
                vertices.Add(new(0, 0));
            }
            else
            {
                neighbors.Add(pieceIndex - cols - 1);
                vertices.Add(new(0, -pieceHeight / 2));
            }
                    
            PieceCut cut = new PieceCut(pieceIndex, neighbors, solutionLocation, vertices);
            pieceCuts.Add(cut);
        }

        private static List<int> EvenRowNeighbors(int r, int c, int rows, int cols, int pieceIndex)
        {
            var neighbors = new List<int>();

            bool notFirstCol = c > 0;
            bool notLastCol = c < cols;

            if (r > 0)
            {
                if (notFirstCol) neighbors.Add(pieceIndex - cols - 1);

                if (notLastCol) neighbors.Add(pieceIndex - cols);
            }

            if (r < rows - 1)
            {
                if (notFirstCol) neighbors.Add(pieceIndex + cols);

                if (notLastCol) neighbors.Add(pieceIndex + cols + 1);
            }
            
            if (notFirstCol) neighbors.Add(pieceIndex - 1);
            
            if (notLastCol) neighbors.Add(pieceIndex + 1);

            return neighbors;
        }
        
        private static List<int> OddRowNeighbors(int r, int c, int rows, int cols, int pieceIndex)
        {
            var neighbors = new List<int>
            {
                pieceIndex - cols - 1, 
                pieceIndex - cols
            };

            if (r < rows - 1)
            {
                neighbors.Add(pieceIndex + cols);
                neighbors.Add(pieceIndex + cols + 1);
            }
            
            if (c > 0) neighbors.Add(pieceIndex - 1);
            
            if (c < cols - 1) neighbors.Add(pieceIndex + 1);

            return neighbors;
        }
    }
}
