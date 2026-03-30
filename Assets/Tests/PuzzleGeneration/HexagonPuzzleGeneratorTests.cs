using System.Threading.Tasks;
using NUnit.Framework;
using PuzzleGeneration;
using PuzzleGeneration.Hexagon;
using PuzzleGeneration.Rectangle;
using UnityEngine;

namespace Tests
{
    public class HexagonPuzzleGeneratorTests
    {
        private const float Tolerance = 1e-6f;
    
        HexagonPuzzleGenerator _generator;
        Texture2D _image;

        [SetUp]
        public void SetUp()
        {
            _generator = new HexagonPuzzleGenerator();
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

            Assert.AreEqual(layout.initialPieceCuts.Count, 9);
        }
    
        [Test]
        public async Task GeneratesCorrectShape()
        {
            var rows = 2;
            var cols = 4;

            var renderData = await _generator.Generate(_image, rows, cols, 1f);
            var layout = renderData.Layout;

            Assert.AreEqual(layout.shape, PieceShape.Hexagon);
        }
    
        #endregion
    
        #region PieceCutTests
    
        [Test]
        public async Task CorrectPieceIndicesAssigned()
        {
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
            var rows = 5;
            var cols = 8;
            
            var renderData = await _generator.Generate(_image, rows, cols, 1f);
            var layout = renderData.Layout;
        
            foreach (var pieceCut in layout.initialPieceCuts)
            {
                var expectedPoints = 6;
                var index = pieceCut.pieceIndex;

                var slot = index + index / (cols * 2 + 1);
                var currRow = slot / (cols + 1);
                var currCol = slot % (cols + 1);
                
                if ((currRow & 1) == 0 && (currCol == 0 || currCol == cols)) expectedPoints = 4;
                else if (currRow == 0 || currRow == rows - 1) expectedPoints = 5;
                
                Assert.AreEqual(pieceCut.borderPoints.Count, expectedPoints);
            }
        }
    
        #endregion
    }
}