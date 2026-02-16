using System;
using System.Linq;
using Persistence;
using UnityEngine;
using UnityEngine.Serialization;

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
        PieceRenderData pieceRenderData
    ) {
        Debug.Log("Initialize Piece Variant");
        
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        meshFilter.sharedMesh = pieceRenderData.Mesh;
        
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        
        Material puzzleImageMaterial = new Material(Shader.Find("Unlit/Texture"));
        puzzleImageMaterial.mainTexture = pieceRenderData.PuzzleRenderData.PuzzleImage;
        
        Vector2 uvScale = new Vector2(1f / 2, 1f / 2);
        Vector2 uvOffset = new Vector2(pieceRenderData.SolutionLocation.x / 0.2f, pieceRenderData.SolutionLocation.y / 0.2f);
        
        puzzleImageMaterial.mainTextureOffset = uvOffset;
        puzzleImageMaterial.mainTextureScale = uvScale;
    
        meshRenderer.sharedMaterials = new[] { puzzleImageMaterial, pieceRenderData.PuzzleRenderData.BackMaterial };
        
        Bounds bounds = pieceRenderData.Mesh.bounds;
        BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
        boxCollider.center = bounds.center;
        boxCollider.size = bounds.size;
        
        _solutionLocation = pieceRenderData.SolutionLocation;
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
