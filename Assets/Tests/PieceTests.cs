using NUnit.Framework;
using UnityEngine;

public class PieceTests
{
    private GameObject _pieceObject;
    private Piece _piece;
    private Mesh _simpleMesh;
    private Material _simpleMaterial;

    [SetUp]
    public void SetUp()
    {
        _pieceObject = new GameObject("CurrPiece");
        
        _piece = _pieceObject.AddComponent<Piece>();
        
        _simpleMesh = new Mesh();
        _simpleMesh.vertices = new [] { Vector3.zero };
        _simpleMesh.RecalculateBounds();
        
        _simpleMaterial = new Material(Shader.Find("Standard"));
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_pieceObject);
    }

    #region Initialization Tests

    [Test]
    public void MeshSet()
    {
        Mesh testMesh = new Mesh();
        testMesh.name = "TestMesh";
        testMesh.vertices = new [] { Vector3.zero, Vector3.one, Vector3.up };
    
        Material testMaterial = new Material(Shader.Find("Standard"));
    
        Vector3 solutionLocation = new Vector3(1, 2, 3);

        _piece.InitializeVariant(solutionLocation, testMesh, testMaterial);

        MeshFilter meshFilter = _pieceObject.GetComponent<MeshFilter>();
        
        Assert.IsNotNull(meshFilter.sharedMesh);
        Assert.AreEqual(3, meshFilter.sharedMesh.vertexCount);
    }

    [Test]
    public void BoxColliderSizeSet()
    {
        Mesh testMesh = new Mesh();
        testMesh.vertices = new [] { 
            new Vector3(-1, -1, -1), 
            new Vector3(1, 1, 1) 
        };
        testMesh.RecalculateBounds();
        Material testMaterial = new Material(Shader.Find("Standard"));

        _piece.InitializeVariant(Vector3.zero, testMesh, testMaterial);

        BoxCollider collider = _pieceObject.GetComponent<BoxCollider>();
        Assert.AreEqual(testMesh.bounds.size, collider.size);
    }

    #endregion

    #region IsRelativelyClose Tests

    [Test]
    public void ExactSolutionLocation()
    {
        GameObject otherObject = new GameObject("OtherPiece");
        Piece otherPiece = otherObject.AddComponent<Piece>();

        Mesh mesh = _simpleMesh;
        Material material = _simpleMaterial;

        Vector3 pieceSolLocation = Vector3.zero;
        Vector3 otherSolLocation = new Vector3(1, 0, 0);

        _piece.InitializeVariant(pieceSolLocation, mesh, material);
        otherPiece.InitializeVariant(otherSolLocation, mesh, material);

        _pieceObject.transform.position = pieceSolLocation;
        otherObject.transform.position = otherSolLocation;

        bool result = _piece.IsRelativelyClose(otherPiece);

        Assert.IsTrue(result);

        Object.DestroyImmediate(otherObject);
    }
    
    [Test]
    public void CorrectRelativeSolutionLocationAfterOffset()
    {
        GameObject otherObject = new GameObject("OtherPiece");
        Piece otherPiece = otherObject.AddComponent<Piece>();

        Mesh mesh = _simpleMesh;
        Material material = _simpleMaterial;

        Vector3 pieceSolLocation = Vector3.zero;
        Vector3 otherSolLocation = new Vector3(1, 0, 0);
        Vector3 solutionOffset = new Vector3(42, 42, 42);

        _piece.InitializeVariant(pieceSolLocation, mesh, material);
        otherPiece.InitializeVariant(otherSolLocation, mesh, material);

        _pieceObject.transform.position = pieceSolLocation + solutionOffset;
        otherObject.transform.position = otherSolLocation + solutionOffset;

        bool result = _piece.IsRelativelyClose(otherPiece);

        Assert.IsTrue(result);

        Object.DestroyImmediate(otherObject);
    }
    
    [Test]
    public void CorrectRelativeSolutionLocationAfterRotation()
    {
        GameObject otherObject = new GameObject("OtherPiece");
        Piece otherPiece = otherObject.AddComponent<Piece>();

        Mesh mesh = _simpleMesh;
        Material material = _simpleMaterial;

        Vector3 pieceSolLocation = Vector3.zero;
        Vector3 otherSolLocation = new Vector3(1, 0, 0);
        Vector3 solutionOffset = new Vector3(42, 42, 42);

        _piece.InitializeVariant(pieceSolLocation, mesh, material);
        otherPiece.InitializeVariant(otherSolLocation, mesh, material);

        _pieceObject.transform.position = pieceSolLocation + solutionOffset;
        otherObject.transform.position = otherSolLocation + solutionOffset;

        bool result = _piece.IsRelativelyClose(otherPiece);

        Assert.IsTrue(result);

        Object.DestroyImmediate(otherObject);
    }

    [Test]
    public void NotRelativeleyClosePieces()
    {
        GameObject otherObject = new GameObject("OtherPiece");
        Piece otherPiece = otherObject.AddComponent<Piece>();

        Mesh mesh = _simpleMesh;
        Material material = _simpleMaterial;
        
        Vector3 pieceSolLocation = Vector3.zero;
        Vector3 otherSolLocation = new Vector3(1, 0, 0);

        _piece.InitializeVariant(pieceSolLocation, mesh, material);
        otherPiece.InitializeVariant(otherSolLocation, mesh, material);

        _pieceObject.transform.position = new Vector3(10, 0, 0);
        otherObject.transform.position = Vector3.zero;

        bool result = _piece.IsRelativelyClose(otherPiece);

        Assert.IsFalse(result);

        Object.DestroyImmediate(otherObject);
    }

    [Test]
    public void RelativleyCloseWithinDistanceThreshold()
    {
        GameObject otherObject = new GameObject("OtherPiece");
        Piece otherPiece = otherObject.AddComponent<Piece>();

        Mesh mesh = _simpleMesh;
        Material material = _simpleMaterial;

        Vector3 pieceSolLocation = Vector3.zero;
        Vector3 otherSolLocation = new Vector3(1, 0, 0);

        _piece.InitializeVariant(pieceSolLocation, mesh, material);
        otherPiece.InitializeVariant(otherSolLocation, mesh, material);

        _pieceObject.transform.position = pieceSolLocation;
        otherObject.transform.position = otherSolLocation - new Vector3(0, 0.0099f, 0);

        bool result = _piece.IsRelativelyClose(otherPiece);

        Assert.IsTrue(result);

        Object.DestroyImmediate(otherObject);
    }
    
    [Test]
    public void JustOutsideDistanceThreshold()
    {
        GameObject otherObject = new GameObject("OtherPiece");
        Piece otherPiece = otherObject.AddComponent<Piece>();

        Mesh mesh = _simpleMesh;
        Material material = _simpleMaterial;

        Vector3 pieceSolLocation = Vector3.zero;
        Vector3 otherSolLocation = new Vector3(1, 0, 0);

        _piece.InitializeVariant(pieceSolLocation, mesh, material);
        otherPiece.InitializeVariant(otherSolLocation, mesh, material);

        _pieceObject.transform.position = pieceSolLocation;
        otherObject.transform.position = otherSolLocation - new Vector3(0, 0.01f, 0);

        bool result = _piece.IsRelativelyClose(otherPiece);

        Assert.IsFalse(result);

        Object.DestroyImmediate(otherObject);
    }
    
    [Test]
    public void RelativleyCloseWithinRotationThreshold()
    {
        GameObject otherObject = new GameObject("OtherPiece");
        Piece otherPiece = otherObject.AddComponent<Piece>();

        Mesh mesh = _simpleMesh;
        Material material = _simpleMaterial;

        Vector3 pieceSolLocation = Vector3.zero;
        Vector3 otherSolLocation = new Vector3(1, 0, 0);

        _piece.InitializeVariant(pieceSolLocation, mesh, material);
        otherPiece.InitializeVariant(otherSolLocation, mesh, material);

        _pieceObject.transform.position = pieceSolLocation;
        otherObject.transform.position = otherSolLocation;
        otherObject.transform.rotation = Quaternion.Euler(44, 0, 0);

        bool result = _piece.IsRelativelyClose(otherPiece);

        Assert.IsTrue(result);

        Object.DestroyImmediate(otherObject);
    }
    
    [Test]
    public void JustOutsideRotationThreshold()
    {
        GameObject otherObject = new GameObject("OtherPiece");
        Piece otherPiece = otherObject.AddComponent<Piece>();

        Mesh mesh = _simpleMesh;
        Material material = _simpleMaterial;

        Vector3 pieceSolLocation = Vector3.zero;
        Vector3 otherSolLocation = new Vector3(1, 0, 0);

        _piece.InitializeVariant(pieceSolLocation, mesh, material);
        otherPiece.InitializeVariant(otherSolLocation, mesh, material);

        _pieceObject.transform.position = pieceSolLocation;
        otherObject.transform.position = otherSolLocation;
        otherObject.transform.rotation = Quaternion.Euler(45, 0, 0);

        bool result = _piece.IsRelativelyClose(otherPiece);

        Assert.IsFalse(result);

        Object.DestroyImmediate(otherObject);
    }

    #endregion

    #region SnapIntoPlace Tests

    [Test]
    public void SnapIntoPlaceExactSolutionLocation()
    {
        GameObject otherObject = new GameObject("OtherPiece");
        Piece otherPiece = otherObject.AddComponent<Piece>();

        Mesh mesh = _simpleMesh;
        Material material = _simpleMaterial;

        Vector3 pieceSolLocation = new Vector3(1, 0, 0);
        Vector3 otherSolLocation = Vector3.zero;

        _piece.InitializeVariant(pieceSolLocation, mesh, material);
        otherPiece.InitializeVariant(otherSolLocation, mesh, material);

        otherObject.transform.position = otherSolLocation;
        _pieceObject.transform.position = new Vector3(5, 5, 5); // Start at wrong position

        _piece.SnapIntoPlace(otherPiece);

        Assert.AreEqual(pieceSolLocation, _pieceObject.transform.position);

        Object.DestroyImmediate(otherObject);
    }

    [Test]
    public void SnapIntoPlaceWithOffset()
    {
        GameObject otherObject = new GameObject("OtherPiece");
        Piece otherPiece = otherObject.AddComponent<Piece>();

        Mesh mesh = _simpleMesh;
        Material material = _simpleMaterial;

        Vector3 pieceSolLocation = new Vector3(2, 3, 4);
        Vector3 otherSolLocation = new Vector3(1, 1, 1);
        Vector3 offset = new Vector3(10, 10, 10);

        _piece.InitializeVariant(pieceSolLocation, mesh, material);
        otherPiece.InitializeVariant(otherSolLocation, mesh, material);

        otherObject.transform.position = otherSolLocation + offset;

        _piece.SnapIntoPlace(otherPiece);

        Vector3 expectedPosition = pieceSolLocation + offset;
        Assert.AreEqual(expectedPosition, _pieceObject.transform.position);

        Object.DestroyImmediate(otherObject);
    }

    [Test]
    public void SnapIntoPlaceMatchesRotation()
    {
        GameObject otherObject = new GameObject("OtherPiece");
        Piece otherPiece = otherObject.AddComponent<Piece>();

        Mesh mesh = _simpleMesh;
        Material material = _simpleMaterial;

        Vector3 pieceSolLocation = Vector3.zero;
        Vector3 otherSolLocation = Vector3.zero;

        _piece.InitializeVariant(pieceSolLocation, mesh, material);
        otherPiece.InitializeVariant(otherSolLocation, mesh, material);

        Quaternion targetRotation = Quaternion.Euler(45, 30, 60);
        otherObject.transform.rotation = targetRotation;
        _pieceObject.transform.rotation = Quaternion.identity;

        _piece.SnapIntoPlace(otherPiece);

        Assert.AreEqual(targetRotation, _pieceObject.transform.rotation);

        Object.DestroyImmediate(otherObject);
    }

    [Test]
    public void SnapIntoPlaceWithRotation90Degrees()
    {
        GameObject otherObject = new GameObject("OtherPiece");
        Piece otherPiece = otherObject.AddComponent<Piece>();

        Mesh mesh = _simpleMesh;
        Material material = _simpleMaterial;

        Vector3 pieceSolLocation = new Vector3(1, 0, 0);
        Vector3 otherSolLocation = Vector3.zero;

        _piece.InitializeVariant(pieceSolLocation, mesh, material);
        otherPiece.InitializeVariant(otherSolLocation, mesh, material);

        otherObject.transform.position = Vector3.zero;
        otherObject.transform.rotation = Quaternion.Euler(0, 90, 0);

        _piece.SnapIntoPlace(otherPiece);

        Vector3 expectedPosition = new Vector3(0, 0, -1);
        Assert.AreEqual(expectedPosition.x, _pieceObject.transform.position.x, 0.001f);
        Assert.AreEqual(expectedPosition.y, _pieceObject.transform.position.y, 0.001f);
        Assert.AreEqual(expectedPosition.z, _pieceObject.transform.position.z, 0.001f);

        Object.DestroyImmediate(otherObject);
    }

    [Test]
    public void SnapIntoPlaceWithRotation180Degrees()
    {
        GameObject otherObject = new GameObject("OtherPiece");
        Piece otherPiece = otherObject.AddComponent<Piece>();

        Mesh mesh = _simpleMesh;
        Material material = _simpleMaterial;

        Vector3 pieceSolLocation = new Vector3(1, 0, 0);
        Vector3 otherSolLocation = Vector3.zero;

        _piece.InitializeVariant(pieceSolLocation, mesh, material);
        otherPiece.InitializeVariant(otherSolLocation, mesh, material);

        otherObject.transform.position = Vector3.zero;
        otherObject.transform.rotation = Quaternion.Euler(0, 180, 0);

        _piece.SnapIntoPlace(otherPiece);

        Vector3 expectedPosition = new Vector3(-1, 0, 0);
        Assert.AreEqual(expectedPosition.x, _pieceObject.transform.position.x, 0.001f);
        Assert.AreEqual(expectedPosition.y, _pieceObject.transform.position.y, 0.001f);
        Assert.AreEqual(expectedPosition.z, _pieceObject.transform.position.z, 0.001f);

        Object.DestroyImmediate(otherObject);
    }

    [Test]
    public void SnapIntoPlaceComplexOffsetAndRotation()
    {
        GameObject otherObject = new GameObject("OtherPiece");
        Piece otherPiece = otherObject.AddComponent<Piece>();

        Mesh mesh = _simpleMesh;
        Material material = _simpleMaterial;

        Vector3 pieceSolLocation = new Vector3(2, 1, 0);
        Vector3 otherSolLocation = Vector3.zero;

        _piece.InitializeVariant(pieceSolLocation, mesh, material);
        otherPiece.InitializeVariant(otherSolLocation, mesh, material);

        otherObject.transform.position = new Vector3(10, 5, 3);
        otherObject.transform.rotation = Quaternion.Euler(0, 45, 0);

        _piece.SnapIntoPlace(otherPiece);

        Vector3 rotatedOffset = otherObject.transform.rotation * pieceSolLocation;
        Vector3 expectedPosition = otherObject.transform.position + rotatedOffset;

        Assert.AreEqual(expectedPosition.x, _pieceObject.transform.position.x, 0.001f);
        Assert.AreEqual(expectedPosition.y, _pieceObject.transform.position.y, 0.001f);
        Assert.AreEqual(expectedPosition.z, _pieceObject.transform.position.z, 0.001f);
        Assert.AreEqual(otherObject.transform.rotation, _pieceObject.transform.rotation);

        Object.DestroyImmediate(otherObject);
    }

    [Test]
    public void SnapIntoPlaceNegativeOffset()
    {
        GameObject otherObject = new GameObject("OtherPiece");
        Piece otherPiece = otherObject.AddComponent<Piece>();

        Mesh mesh = _simpleMesh;
        Material material = _simpleMaterial;

        Vector3 pieceSolLocation = new Vector3(-2, -1, -3);
        Vector3 otherSolLocation = Vector3.zero;

        _piece.InitializeVariant(pieceSolLocation, mesh, material);
        otherPiece.InitializeVariant(otherSolLocation, mesh, material);

        otherObject.transform.position = new Vector3(5, 5, 5);

        _piece.SnapIntoPlace(otherPiece);

        Vector3 expectedPosition = new Vector3(3, 4, 2);
        Assert.AreEqual(expectedPosition, _pieceObject.transform.position);

        Object.DestroyImmediate(otherObject);
    }

    [Test]
    public void SnapIntoPlaceBothPiecesWithNonZeroSolutionLocations()
    {
        GameObject otherObject = new GameObject("OtherPiece");
        Piece otherPiece = otherObject.AddComponent<Piece>();

        Mesh mesh = _simpleMesh;
        Material material = _simpleMaterial;

        Vector3 pieceSolLocation = new Vector3(5, 3, 2);
        Vector3 otherSolLocation = new Vector3(3, 2, 1);

        _piece.InitializeVariant(pieceSolLocation, mesh, material);
        otherPiece.InitializeVariant(otherSolLocation, mesh, material);

        otherObject.transform.position = new Vector3(10, 10, 10);

        _piece.SnapIntoPlace(otherPiece);

        Vector3 solutionOffset = pieceSolLocation - otherSolLocation;
        Vector3 expectedPosition = otherObject.transform.position + solutionOffset;
        
        Assert.AreEqual(expectedPosition, _pieceObject.transform.position);

        Object.DestroyImmediate(otherObject);
    }

    #endregion
}
