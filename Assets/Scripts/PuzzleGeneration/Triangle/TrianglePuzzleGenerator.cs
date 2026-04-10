using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PuzzleGeneration.Triangle
{
    public class TrianglePuzzleGenerator: IPuzzleGenerator
    {
        private const float MaxHorizontalShiftRatio = 0.25f;
        private const int LeftTriangleOffset = 0;
        private const int RightTriangleOffset = 1;
        
        public async Task<PuzzleGenerationData> Generate(
            Texture2D image, 
            int rows, 
            int cols, 
            float puzzleHeight
        )
        {
            var widthHeightRatio = (float) image.width / image.height;
            var puzzleWidth = puzzleHeight * widthHeightRatio;
            
            var pieceHeight = puzzleHeight / rows;
            var avgPieceWidth = puzzleWidth / cols;
            var horizontalShiftRange = avgPieceWidth * MaxHorizontalShiftRatio;

            var pieceCuts = new List<PieceCut>();
            var prevRowTrianglePairs = new TrianglePair[cols];
        
            for (var r = 0; r < rows; r++)
            {
                var leftBoundary = 0f;
                var currRowTrianglePairs = new TrianglePair[cols];

                for (var c = 0; c < cols; c++)
                {
                    var rightBoundary = avgPieceWidth * (c + 1);
                    if (c < cols - 1)
                    {
                        rightBoundary += Random.Range(-horizontalShiftRange, horizontalShiftRange);
                    }

                    var pair = CreateTrianglePair(leftBoundary, rightBoundary, pieceHeight);
                    var solutionLocation = new Vector3(leftBoundary, pieceHeight * r);
                    
                    var leftIndex = pieceCuts.Count;
                    var rightIndex = leftIndex + 1;

                    AddPieceCut(pieceCuts, leftIndex, r, c * 2, rightIndex, solutionLocation, pair.LeftVertices,
                        prevRowNeighborIndex: GetPrevRowNeighborIndex(prevRowTrianglePairs, c, leftIndex, cols),
                        isPrevRowNeighborCondition: !pair.PositiveSlopingDiagonal,
                        hasPrevCol: c > 0,
                        hasNextCol: false
                    );

                    AddPieceCut(pieceCuts, rightIndex, r, c * 2 + 1, leftIndex, solutionLocation, pair.RightVertices,
                        prevRowNeighborIndex: GetPrevRowNeighborIndex(prevRowTrianglePairs, c, leftIndex, cols),
                        isPrevRowNeighborCondition: pair.PositiveSlopingDiagonal,
                        hasPrevCol: false,
                        hasNextCol: c < cols - 1
                    );

                    currRowTrianglePairs[c] = pair;
                    leftBoundary = rightBoundary;
                }

                prevRowTrianglePairs = currRowTrianglePairs;
            }
        
            var layout = new PuzzleLayout(
                rows, 
                cols, 
                puzzleWidth, 
                puzzleHeight, 
                PieceShape.Triangle, 
                pieceCuts
            );
            var renderData = new PuzzleGenerationData(image, layout);

            return renderData;
        }

        private static void AddPieceCut(
            List<PieceCut> pieceCuts,
            int index,
            int row,
            int col,
            int siblingIndex,
            Vector3 solutionLocation,
            List<Vector2> vertices,
            int? prevRowNeighborIndex,
            bool isPrevRowNeighborCondition,
            bool hasPrevCol,
            bool hasNextCol
        ) {
            var neighbors = new List<int> { siblingIndex };

            if (hasPrevCol) neighbors.Add(index - 1);
            if (hasNextCol) neighbors.Add(index + 1);

            if (prevRowNeighborIndex.HasValue && isPrevRowNeighborCondition)
            {
                Debug.Log($"{prevRowNeighborIndex.Value} {index} {pieceCuts.Count}");
                
                neighbors.Add(prevRowNeighborIndex.Value);
                pieceCuts[prevRowNeighborIndex.Value].neighborIndices.Add(index);
            }

            var pieceCut = new PieceCut(index, neighbors, solutionLocation, vertices);
            pieceCuts.Add(pieceCut);
        }

        private static int? GetPrevRowNeighborIndex(
            TrianglePair[] prevRowTrianglePairs,
            int col,
            int leftIndex,
            int cols
        ) {
            if (col >= prevRowTrianglePairs.Length || prevRowTrianglePairs[col] == null)
            {
                return null;
            }

            var prevRowPair = prevRowTrianglePairs[col];
            var pairStartIndex = leftIndex - (cols << 1);
            var pairOffset = prevRowPair.PositiveSlopingDiagonal 
                ? LeftTriangleOffset 
                : RightTriangleOffset;
            
            return pairStartIndex + pairOffset;
        }

        private static TrianglePair CreateTrianglePair(
            float leftBoundary, 
            float rightBoundary, 
            float pieceHeight
        ) {
            var rectangleWidth = rightBoundary - leftBoundary;
            var positiveSlopDiagonal = Random.Range(0, 2) == 0;

            var leftMidVertex = positiveSlopDiagonal
                ? new Vector2(rectangleWidth, pieceHeight)
                : new Vector2(rectangleWidth, 0);

            var leftVertices = new List<Vector2>
            {
                new(0, pieceHeight),
                leftMidVertex,
                new(0, 0),
            };

            var rightMidVertex = positiveSlopDiagonal 
                ? new Vector2(0, 0) 
                : new Vector2(0, pieceHeight);
            
            var rightVertices = new List<Vector2>
            {
                new(rectangleWidth, 0),
                rightMidVertex,
                new(rectangleWidth, pieceHeight),
            };

            return new TrianglePair(leftVertices, rightVertices, positiveSlopDiagonal);
        }
    }
}
