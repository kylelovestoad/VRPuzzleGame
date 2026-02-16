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
    private const float ConnectionDistanceThreshold = 0.01f;
    private const float ConnectionRotationThreshold = 45f;
    
    private Vector3 _solutionLocation;
    
    public void InitializePiece(
        PieceCut pieceCut,
        PuzzleRenderData puzzleRenderData
    ) {
        Debug.Log("Initialize Piece Variant");
        
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        meshFilter.sharedMesh = pieceCut.Mesh;
        
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        
        Material puzzleImageMaterial = new Material(Shader.Find("Unlit/Texture"));
        puzzleImageMaterial.mainTexture = puzzleRenderData.PuzzleImage;

        var pieceSolutionLocation = pieceCut.SolutionLocation;
        var cut = puzzleRenderData.Layout;
        
        Vector2 uvScale = new Vector2(pieceCut.Width / cut.Width, pieceCut.Height / cut.Height);
        Vector2 uvOffset = new Vector2(pieceSolutionLocation.x / cut.Width, pieceSolutionLocation.y / cut.Height);
        
        puzzleImageMaterial.mainTextureOffset = uvOffset;
        puzzleImageMaterial.mainTextureScale = uvScale;
    
        meshRenderer.sharedMaterials = new[] { puzzleImageMaterial, puzzleRenderData.BackMaterial };
        
        Bounds bounds = pieceCut.Mesh.bounds;
        BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
        boxCollider.center = bounds.center;
        boxCollider.size = bounds.size;
        
        _solutionLocation = pieceCut.SolutionLocation;
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
        return _solutionLocation - other._solutionLocation;
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
            solutionLocation = _solutionLocation,
            position = transform.position,
            rotation = transform.rotation
        };
    }
}
