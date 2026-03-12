using System;
using System.Linq;
using Persistence;
using PuzzleGeneration;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(BoxCollider))]
public class Piece : MonoBehaviour
{
    // TODO: make better thresholds
    private const float ConnectionDistanceThreshold = 0.01f;
    private const float ConnectionRotationThreshold = 45f;
    
    private PieceCut _cut;
    private Vector2 SolutionLocation => _cut.solutionLocation;
    
    public void InitializePiece(
        PieceCut pieceCut,
        PuzzleRenderData puzzleRenderData
    )
    {
        gameObject.SetActive(true);
        
        Mesh pieceMesh = PieceMeshGenerator.PieceMesh(pieceCut.borderPoints);
        
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        meshFilter.sharedMesh = pieceMesh;
        
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();

        var shader = Shader.Find("Unlit/Texture");
        var puzzleImageMaterial = new Material(shader);
        puzzleImageMaterial.mainTexture = puzzleRenderData.PuzzleImage;

        var pieceSolutionLocation = pieceCut.solutionLocation;
        var pieceBounds = pieceMesh.bounds;
        var pieceWidth = pieceBounds.max.x - pieceBounds.min.x;
        var pieceHeight = pieceBounds.max.y - pieceBounds.min.y;
        var puzzleLayout = puzzleRenderData.Layout;
        
        Vector2 uvScale = new Vector2(pieceWidth / puzzleLayout.width, pieceHeight / puzzleLayout.height);
        Vector2 uvOffset = new Vector2(
            (pieceSolutionLocation.x + pieceBounds.min.x) / puzzleLayout.width,
            (pieceSolutionLocation.y + pieceBounds.min.y) / puzzleLayout.height
        );
        
        puzzleImageMaterial.mainTextureOffset = uvOffset;
        puzzleImageMaterial.mainTextureScale = uvScale;
        
        Material backAndSidesMaterial = new(Shader.Find("Unlit/Color"))
        {
            color = Color.gray
        };
    
        meshRenderer.sharedMaterials = new[] { puzzleImageMaterial, backAndSidesMaterial };
        
        Bounds bounds = pieceMesh.bounds;
        BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
        boxCollider.center = bounds.center;
        boxCollider.size = bounds.size;
        
        _cut = pieceCut;
    }

    public Vector3[] Vertices()
    {
        return gameObject
            .GetComponent<MeshFilter>()
            .sharedMesh
            .vertices
            .Select(vertex => transform.TransformPoint(vertex))
            .ToArray();
    }

    private bool IsNeighbor(Piece other)
    {
        return _cut.neighborIndices.Contains(other._cut.pieceIndex);
    }
    
    private Vector3 SolutionOffset(Piece other)
    {
        return SolutionLocation - other.SolutionLocation;
    }
    
    private Vector3 ExpectedPosition(Piece other)
    {
        return other.transform.position + other.transform.rotation * SolutionOffset(other);
    }
    
    public bool IsCloseEnough(Piece other)
    {
        if (!IsNeighbor(other)) return false;
        
        var expectedPosition = ExpectedPosition(other);
        var actualPosition = transform.position;
        
        return Vector3.Distance(expectedPosition, actualPosition) < ConnectionDistanceThreshold 
               && Quaternion.Angle(transform.rotation, other.transform.rotation) < ConnectionRotationThreshold;
    }

    public void SnapIntoPlace(Piece other)
    {
        transform.position = ExpectedPosition(other);
        transform.rotation = other.transform.rotation;
    }

    public PieceSaveData ToData()
    {
        return new PieceSaveData
        {
            pieceIndex = _cut.pieceIndex,
            position = transform.position,
            rotation = transform.rotation
        };
    }
}
