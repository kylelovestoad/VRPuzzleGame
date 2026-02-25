using NUnit.Framework;
using PuzzleGeneration;
using PuzzleGeneration.Rectangle;
using UnityEngine;

namespace Tests
{
    public class RectanglePuzzleGeneratorTests
    {
        private const float Tolerance = 1e-6f;
    
        RectanglePuzzleGenerator _generator;
        Texture2D _image;

        [SetUp]
        public void SetUp()
        {
            _generator = new RectanglePuzzleGenerator();
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

            Assert.AreEqual(layout.shape, PieceShape.Rectangle);
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
        public void CorrectPieceFaceVerticesCount()
        {
            int rows = 2;
            int cols = 4;
            PuzzleLayout layout = _generator.Generate(_image, rows, cols, 1f);
        
            foreach (var pieceCut in layout.initialPieceCuts)
            {
                Assert.AreEqual(pieceCut.borderPoints.Count, 4);
            }
        }
    
        #endregion
    }
}
