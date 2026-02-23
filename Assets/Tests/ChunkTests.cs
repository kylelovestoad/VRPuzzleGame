using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using PuzzleGeneration;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

public class ChunkTests
{
    private const float Tolerance = 1e-6f;

    private GameObject _chunkPrefab;
    private GameObject _chunkObject;
    private Chunk _chunk;
    private Mesh _simpleMesh;

    [SetUp]
    public void SetUp()
    {
        _chunkPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Chunk.prefab");
        
        _chunkObject = Object.Instantiate(_chunkPrefab);
        _chunk = _chunkObject.GetComponent<Chunk>();
        
        _simpleMesh = new Mesh();
        _simpleMesh.vertices = new [] { Vector3.zero };
        _simpleMesh.RecalculateBounds();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_chunkObject);
    }

    #region Initialization Tests

    [Test]
    public void PrefabComponentsExist()
    {
        BoxCollider boxCollider = _chunk.GetComponent<BoxCollider>();
        Assert.IsNotNull(boxCollider, "BoxCollider should exist on chunk");

        Rigidbody rigidbody = _chunk.GetComponent<Rigidbody>();
        Assert.IsNotNull(rigidbody, "Rigidbody should exist on chunk");
    }
    
    [UnityTest]
    public IEnumerator MakesChunkWithSinglePieceInitially()
    {
        var cut = new PieceCut(Vector3.zero, _simpleMesh);
        var pieceCuts = new List<PieceCut> { cut };
        
        PuzzleLayout mockLayout = new PuzzleLayout(1, 1, PieceShape.Rectangle, pieceCuts);
        PuzzleRenderData mockRenderData = new PuzzleRenderData(null, null, mockLayout);
        
        var puzzle = new Puzzle(mockRenderData);
    
        _chunk.InitializeSinglePieceChunk(cut, puzzle);

        Assert.AreEqual(1, _chunk.PieceCount);
        
        yield return null;
    }
    
    #endregion
    
    #region Updating Chunk Tests
    
    [Test]
    public void UpdatesChunkSizeWhenMorePiecesAdded()
    {
        Vector3 solutionLocation = Vector3.zero;
        
        GameObject newPieceObject = _chunkObject.transform.Find("Piece").gameObject;
        Piece piece = newPieceObject.AddComponent<Piece>();
        piece.transform.position = new Vector3(0, 0, 0);
        
        Mesh mesh = new Mesh();
        mesh.vertices = new Vector3[]
        {
            new(0, 0, 0), 
            new(0, 1, 0), 
            new(1, 0, 0), 
            new(1, 1, 0)
        };
        mesh.RecalculateBounds();
        
        MethodInfo awakeMethod = typeof(Chunk).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
        awakeMethod?.Invoke(_chunk, null);
        
        var piece0Cut = new PieceCut(Vector3.zero, mesh);
        var piece1Cut = new PieceCut(new Vector3(1, 0, 0), mesh);
        var pieceCuts = new List<PieceCut> { piece0Cut, piece1Cut };
        
        PuzzleLayout mockLayout = new PuzzleLayout(2, 1, PieceShape.Rectangle, pieceCuts);
        PuzzleRenderData mockRenderData = new PuzzleRenderData(null, null, mockLayout);
        
        var puzzle = new Puzzle(mockRenderData);
        
        _chunk.InitializeSinglePieceChunk(piece0Cut, puzzle);
        
        MethodInfo updateBoxColliderMethod = typeof(Chunk).GetMethod(
            "UpdateBoxCollider", 
            BindingFlags.NonPublic | BindingFlags.Instance
        );
        
        List<Piece> pieces = new List<Piece> { piece };
        updateBoxColliderMethod?.Invoke(_chunk, new object[] { pieces });
        
        BoxCollider boxCollider = _chunk.GetComponent<BoxCollider>();
        
        Assert.AreEqual(boxCollider.size.x, 1.02f, Tolerance);
        Assert.AreEqual(boxCollider.size.y, 1.02f, Tolerance);
        Assert.AreEqual(boxCollider.size.z, 0.02f, Tolerance);
        
        Assert.AreEqual(boxCollider.center.x, 0.5f, Tolerance);
        Assert.AreEqual(boxCollider.center.y, 0.5f, Tolerance);
        Assert.AreEqual(boxCollider.center.z, 0.0f, Tolerance);
        
        Object.DestroyImmediate(newPieceObject);
    }
    
    [Test]
    public void CombiningChunks()
    {
        Vector3 solutionLocation = Vector3.zero;
        
        GameObject newPieceObject = _chunkObject.transform.Find("Piece").gameObject;
        Piece piece = newPieceObject.AddComponent<Piece>();
        piece.transform.position = new Vector3(0, 0, 0);
        
        Mesh mesh = new Mesh();
        mesh.vertices = new Vector3[]
        {
            new(0, 0, 0), 
            new(0, 1, 0), 
            new(1, 0, 0), 
            new(1, 1, 0)
        };
        mesh.RecalculateBounds();
        
        MethodInfo awakeMethod = typeof(Chunk).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
        awakeMethod?.Invoke(_chunk, null);
        
        var piece0Cut = new PieceCut(Vector3.zero, mesh);
        var piece1Cut = new PieceCut(new Vector3(1, 0, 0), mesh);
        var pieceCuts = new List<PieceCut> { piece0Cut, piece1Cut };
        
        PuzzleLayout mockLayout = new PuzzleLayout(2, 1, PieceShape.Rectangle, pieceCuts);
        PuzzleRenderData mockRenderData = new PuzzleRenderData(null, null, mockLayout);
        
        var puzzle = new Puzzle(mockRenderData);
        
        _chunk.InitializeSinglePieceChunk(piece0Cut, puzzle);
        
        GameObject otherChunkObject = Object.Instantiate(_chunkPrefab);
        Chunk otherChunk = otherChunkObject.GetComponent<Chunk>();
        awakeMethod?.Invoke(otherChunk, null);
        
        GameObject otherPieceObject = otherChunkObject.transform.Find("Piece").gameObject;
        Piece otherPiece = otherPieceObject.AddComponent<Piece>();
        otherPiece.transform.position = new Vector3(1, 0, 0);
        
        otherChunk.InitializeSinglePieceChunk(piece1Cut, puzzle);
        
        MethodInfo combineMethod = typeof(Chunk).GetMethod(
            "Merge", 
            BindingFlags.NonPublic | BindingFlags.Instance
        );
        combineMethod?.Invoke(_chunk, new object[] { otherChunk });
        
        Assert.AreEqual(2, _chunk.transform.childCount, "Merged chunk should contains both pieces");
        
        FieldInfo piecesField = typeof(Chunk).GetField("pieces", 
            BindingFlags.NonPublic | BindingFlags.Instance
        );
        IList pieces = (IList) piecesField.GetValue(_chunk);
    
        Assert.AreEqual(2, pieces.Count, "Should have 2 pieces");
    }
    
    #endregion
}
