using System;
using System.Linq;
using Persistence;
using PuzzleGeneration;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(BoxCollider))]
[Serializable]
public class Piece : MonoBehaviour
{
    // TODO: make better thresholds
    private const float ConnectionDistanceThreshold = 0.01f;
    private const float ConnectionRotationThreshold = 45f;
    
    private PieceCut _cut;
    private Vector2 SolutionLocation => _cut.SolutionLocation;
    private Mesh Mesh => _cut.Mesh;
    
    public void InitializePiece(
        PieceCut pieceCut,
        PuzzleRenderData puzzleRenderData
    ) {
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        meshFilter.sharedMesh = pieceCut.Mesh;
        
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        
        Material puzzleImageMaterial = new Material(Shader.Find("Unlit/Texture"));
        puzzleImageMaterial.mainTexture = puzzleRenderData.PuzzleImage;

        var pieceSolutionLocation = pieceCut.SolutionLocation;
        var pieceBounds = pieceCut.Mesh.bounds;
        var pieceWidth = pieceBounds.max.x - pieceBounds.min.x;
        var pieceHeight = pieceBounds.max.y - pieceBounds.min.y;
        var puzzleLayout = puzzleRenderData.Layout;
        
        Vector2 uvScale = new Vector2(pieceWidth / puzzleLayout.Width, pieceHeight / puzzleLayout.Height);
        Vector2 uvOffset = new Vector2(
            (pieceSolutionLocation.x + pieceBounds.min.x) / puzzleLayout.Width,
            (pieceSolutionLocation.y + pieceBounds.min.y) / puzzleLayout.Height
        );
        
        puzzleImageMaterial.mainTextureOffset = uvOffset;
        puzzleImageMaterial.mainTextureScale = uvScale;
    
        meshRenderer.sharedMaterials = new[] { puzzleImageMaterial, puzzleRenderData.BackAndSidesMaterial };
        
        Bounds bounds = pieceCut.Mesh.bounds;
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
    
    private Vector3 SolutionOffset(Piece other)
    {
        return SolutionLocation - other.SolutionLocation;
    }
    
    private Vector3 ExpectedPosition(Piece other)
    {
        return other.transform.position + other.transform.rotation * SolutionOffset(other);
    }
    
    public bool IsRelativelyClose(Piece other)
    {
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
            vertices = Mesh.vertices,
            triangles = Mesh.triangles, // might be better to derive
            solutionLocation = SolutionLocation,
            position = transform.position,
            rotation = transform.rotation
        };
    }
}
