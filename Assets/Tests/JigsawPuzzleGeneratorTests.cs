using NUnit.Framework;
using PuzzleGeneration;
using PuzzleGeneration.Jigsaw;
using UnityEngine;

namespace Tests
{
    public class JigsawPuzzleGeneratorTests
    {
        private const float Tolerance = 1e-6f;
    
        JigsawPuzzleGenerator _generator;
        Texture2D _image;

        [SetUp]
        public void SetUp()
        {
            _generator = new JigsawPuzzleGenerator();
            _image = new Texture2D(42, 128);
        }
    
        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_image);
        }

        #region PuzzleSettingTests

        [Test]
        public void GeneratesCorrectPieceDimensions()
        {
            var image = new Texture2D(42, 128);

            PuzzleLayout layout = _generator.Generate(image, 2, 4, 1f);

            Assert.AreEqual(1f, layout.height, Tolerance);
            Assert.AreEqual(42f / 128, layout.width, Tolerance);
        }
    
        [Test]
        public void GeneratesCorrectNumberOfPieces()
        {
            var image = new Texture2D(42, 128);

            int rows = 2;
            int cols = 4;

            PuzzleLayout layout = _generator.Generate(image, rows, cols, 1f);

            Assert.AreEqual(layout.initialPieceCuts.Count, 8);
        }
    
        [Test]
        public void GeneratesCorrectShape()
        {
            var image = new Texture2D(42, 128);

            int rows = 2;
            int cols = 4;

            PuzzleLayout layout = _generator.Generate(image, rows, cols, 1f);

            Assert.AreEqual(layout.shape, PieceShape.Jigsaw);
        }
    
        #endregion

        #region PieceCutTests
    
        [Test]
        public void CorrectPieceIndicesAssigned()
        {
            var image = new Texture2D(42, 128);

            int rows = 2;
            int cols = 4;
            PuzzleLayout layout = _generator.Generate(image, rows, cols, 1f);
        
            int expectedIndex = 0;
        
            foreach (var pieceCut in layout.initialPieceCuts)
            {
                Assert.AreEqual(pieceCut.pieceIndex, expectedIndex);
                expectedIndex++;
            }
        }
    
        [Test]
        public void EdgesHaveLessVertices()
        {
            int rows = 3;
            int cols = 4;
        
            PuzzleLayout layout = _generator.Generate(_image, rows, cols, 1f);
            var initialPieceCuts = layout.initialPieceCuts;
        
            var cornerPiece = initialPieceCuts[0];
            var edgePiece = initialPieceCuts[1];
            var middlePiece = initialPieceCuts[5];
        
            Assert.IsTrue(cornerPiece.borderPoints.Count < edgePiece.borderPoints.Count);
            Assert.IsTrue(cornerPiece.borderPoints.Count < middlePiece.borderPoints.Count);
            Assert.IsTrue(edgePiece.borderPoints.Count < middlePiece.borderPoints.Count);
        }
    
        [Test]
        public void CornersAndEdgesAssignedCorrectly()
        {
            int rows = 3;
            int cols = 4;
        
            PuzzleLayout layout = _generator.Generate(_image, rows, cols, 1f);
            var initialPieceCuts = layout.initialPieceCuts;
        
            var cornerCounts = initialPieceCuts[0].borderPoints.Count;
            var edgeCounts = initialPieceCuts[1].borderPoints.Count;
            var middleCounts = initialPieceCuts[5].borderPoints.Count;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    var currPiece = initialPieceCuts[row * cols + col];
                
                    if (row == 0 || row == rows - 1)
                    {
                        if (col == 0 || col == cols - 1)
                        {
                            Assert.IsTrue(currPiece.borderPoints.Count == cornerCounts);
                        }
                        else
                        {
                            Assert.IsTrue(currPiece.borderPoints.Count == edgeCounts);
                        }
                    } 
                    else if (col == 0 || col == cols - 1)
                    {
                        Assert.IsTrue(currPiece.borderPoints.Count == edgeCounts);
                    }
                    else
                    {
                        Assert.IsTrue(currPiece.borderPoints.Count == middleCounts);
                    }
                }
            }
        }
    
        #endregion
    }
}
