using System.Threading.Tasks;
using NUnit.Framework;
using PuzzleGeneration;
using PuzzleGeneration.Rectangle;
using PuzzleGeneration.Triangle;
using UnityEngine;

namespace Tests
{
    public class TrianglePuzzleGeneratorTests
    {
        private const float Tolerance = 1e-6f;
    
        TrianglePuzzleGenerator _generator;
        Texture2D _image;

        [SetUp]
        public void SetUp()
        {
            _generator = new TrianglePuzzleGenerator();
            _image = new Texture2D(42, 128);
        }
    
        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_image);
        }

        #region PuzzleSettingTests
    
        [Test]
        public async Task GeneratesCorrectPieceDimensions()
        {
            var rows = 2;
            var cols = 4;

            var renderData = await _generator.Generate(_image, rows, cols, 1f);
            var layout = renderData.Layout;

            Assert.AreEqual(1f, layout.height, Tolerance);
            Assert.AreEqual(42f / 128, layout.width, Tolerance);
        }
    
        [Test]
        public async Task GeneratesCorrectNumberOfPieces()
        {
            var rows = 2;
            var cols = 4;

            var renderData = await _generator.Generate(_image, rows, cols, 1f);
            var layout = renderData.Layout;

            Assert.AreEqual(layout.initialPieceCuts.Count, 16);
        }
    
        [Test]
        public async Task GeneratesCorrectShape()
        {
            var rows = 2;
            var cols = 4;

            var renderData = await _generator.Generate(_image, rows, cols, 1f);
            var layout = renderData.Layout;

            Assert.AreEqual(layout.shape, PieceShape.Triangle);
        }
    
        #endregion
    
        #region PieceCutTests
    
        [Test]
        public async Task CorrectPieceIndicesAssigned()
        {
            var image = new Texture2D(42, 128);

            var rows = 2;
            var cols = 4;
            var renderData = await _generator.Generate(_image, rows, cols, 1f);
            var layout = renderData.Layout;
        
            var expectedIndex = 0;
        
            foreach (var pieceCut in layout.initialPieceCuts)
            {
                Assert.AreEqual(pieceCut.pieceIndex, expectedIndex);
                expectedIndex++;
            }
        }
    
        [Test]
        public async Task CorrectPieceFaceVerticesCount()
        {
            int rows = 2;
            int cols = 4;
            var renderData = await _generator.Generate(_image, rows, cols, 1f);
            var layout = renderData.Layout;
        
            foreach (var pieceCut in layout.initialPieceCuts)
            {
                Assert.AreEqual(pieceCut.borderPoints.Count, 3);
            }
        }
    
        #endregion
    }
}
