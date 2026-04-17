using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class HintManagerTests
    {
        private HintManager _hintManager;
        private Material _frontMaterial;
        private Material _backAndSidesMaterial;
        private Piece _piece0;
        private Piece _piece1;
        private GameObject _piece0Go;
        private GameObject _piece1Go;

        [SetUp]
        public void SetUp()
        {
            var shader = Shader.Find("Standard");
        
            _frontMaterial = new Material(shader);
            _backAndSidesMaterial = new Material(shader);

            _hintManager = new HintManager(_frontMaterial, _backAndSidesMaterial);

            var puzzle = TestUtils.MakePuzzle();

            _piece0Go = TestUtils.CreatePieceObject("piece", puzzle.layout.initialPieceCuts[0], puzzle);
            _piece1Go = TestUtils.CreatePieceObject("piece", puzzle.layout.initialPieceCuts[1], puzzle);
            
            _piece0 = _piece0Go.GetComponent<Piece>();
            _piece1 = _piece1Go.GetComponent<Piece>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_frontMaterial);
            Object.DestroyImmediate(_backAndSidesMaterial);
            Object.DestroyImmediate(_piece0Go);
            Object.DestroyImmediate(_piece1Go);
        }
    
        [Test]
        public void HintActiveFalseWhenNoHintShown()
        {
            Assert.IsFalse(_hintManager.HintActive);
        }

        [Test]
        public void HintActiveTrueAfterShowHint()
        {
            _hintManager.ShowHint(_piece0, _piece1);

            Assert.IsTrue(_hintManager.HintActive);
        }

        [Test]
        public void ShowHintOverridesMaterialsOnBothPieces()
        {
            var originalMat0 = _piece0.GetComponent<MeshRenderer>().sharedMaterials[0];
            var originalMat1 = _piece1.GetComponent<MeshRenderer>().sharedMaterials[0];

            _hintManager.ShowHint(_piece0, _piece1);

            var newMat0 = _piece0.GetComponent<MeshRenderer>().sharedMaterials[0];
            var newMat1 = _piece1.GetComponent<MeshRenderer>().sharedMaterials[0];

            Assert.AreNotSame(originalMat0, newMat0);
            Assert.AreNotSame(originalMat1, newMat1);
        }

        [Test]
        public void HintMaterialInheritsTextureFromOriginal()
        {
            var originalTexture = _piece0.GetComponent<MeshRenderer>().sharedMaterials[0].mainTexture;

            _hintManager.ShowHint(_piece0, _piece1);

            var hintTexture = _piece0.GetComponent<MeshRenderer>().sharedMaterials[0].mainTexture;
            Assert.AreEqual(originalTexture, hintTexture);
        }
    
        [Test]
        public void NoClearWhenNoHintActive()
        {
            Assert.DoesNotThrow(() =>
                _hintManager.ClearHintIfConnected(new[] { _piece0, _piece1 })
            );

            Assert.IsFalse(_hintManager.HintActive);
        }

        [Test]
        public void ClearHintOnConnection()
        {
            _hintManager.ShowHint(_piece0, _piece1);

            _hintManager.ClearHintIfConnected(new[] { _piece0, _piece1 });

            Assert.IsFalse(_hintManager.HintActive);
        }

        [Test]
        public void OriginalMaterialsSetAfterClear()
        {
            var originalMat0 = _piece0.GetComponent<MeshRenderer>().sharedMaterials[0];
            var originalMat1 = _piece1.GetComponent<MeshRenderer>().sharedMaterials[0];

            _hintManager.ShowHint(_piece0, _piece1);
            _hintManager.ClearHintIfConnected(new[] { _piece0, _piece1 });

            Assert.AreSame(originalMat0, _piece0.GetComponent<MeshRenderer>().sharedMaterials[0]);
            Assert.AreSame(originalMat1, _piece1.GetComponent<MeshRenderer>().sharedMaterials[0]);
        }
    }
}