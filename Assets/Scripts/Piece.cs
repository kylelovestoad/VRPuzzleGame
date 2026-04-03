using System;
using System.Collections.Generic;
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
    public int PieceIndex => _cut.pieceIndex;
    public List<int> NeighborIndices => _cut.neighborIndices;
    public List<Vector2> BorderPoints => _cut.borderPoints;
    
    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshFilter = GetComponent<MeshFilter>();
    }
    
    public void InitializePiece(
        PieceCut pieceCut,
        PuzzleSaveData saveData
    )
    {
        gameObject.SetActive(true);
        
        var pieceMesh = PieceMeshGenerator.PieceMesh(pieceCut.borderPoints);

        _meshFilter.sharedMesh = pieceMesh;
        
        Debug.Log("Here!!!!!!!!!");

        var shader = Shader.Find("Unlit/Texture");
        var puzzleImageMaterial = new Material(shader);
        puzzleImageMaterial.mainTexture = saveData.PuzzleImage;

        var pieceSolutionLocation = pieceCut.solutionLocation;
        var pieceBounds = pieceMesh.bounds;
        var pieceWidth = pieceBounds.max.x - pieceBounds.min.x;
        var pieceHeight = pieceBounds.max.y - pieceBounds.min.y;
        var puzzleLayout = saveData.layout;
        
        Debug.Log("Here 1!!!!!!!!!");
        
        Debug.Log($"Puzzle Width: {puzzleLayout.width}, Puzzle Height: {puzzleLayout.height}");
        
        var uvScale = new Vector2(pieceWidth / puzzleLayout.width, pieceHeight / puzzleLayout.height);
        var uvOffset = new Vector2(
            (pieceSolutionLocation.x + pieceBounds.min.x) / puzzleLayout.width,
            (pieceSolutionLocation.y + pieceBounds.min.y) / puzzleLayout.height
        );
        
        puzzleImageMaterial.mainTextureOffset = uvOffset;
        puzzleImageMaterial.mainTextureScale = uvScale;
        
        Material backAndSidesMaterial = new(Shader.Find("Unlit/Color"))
        {
            color = Color.gray
        };
        
        Debug.Log("Here 2!!!!!!!!!");
    
        _meshRenderer.sharedMaterials = new[] { puzzleImageMaterial, backAndSidesMaterial };
        
        var bounds = pieceMesh.bounds;
        var boxCollider = gameObject.GetComponent<BoxCollider>();
        boxCollider.center = bounds.center;
        boxCollider.size = bounds.size;
        
        _cut = pieceCut;
        
        Debug.Log("Finished!!!!!!!!!");
    }

    public Vector3[] Vertices()
    {
        return _meshFilter
            .sharedMesh
            .vertices
            .Select(vertex => transform.TransformPoint(vertex))
            .ToArray();
    }

    private bool IsNeighbor(Piece other)
    {
        if (_cut == null || other._cut == null) return false;
        
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
    
    public void SetOutlineMaterial(Material material)
    {
        var materials = _meshRenderer.sharedMaterials;
        var newMaterials = new Material[materials.Length + 1];
        
        materials.CopyTo(newMaterials, 0);
        newMaterials[^1] = material;
        _meshRenderer.sharedMaterials = newMaterials;
    }

    public void ClearOutlineMaterial(Material material)
    {
        _meshRenderer.sharedMaterials = _meshRenderer.sharedMaterials
            .Where(m => m != material)
            .ToArray();
    }

    public PieceSaveData ToData()
    {
        Debug.Log(("Saving Piece: " + _cut.pieceIndex));
        
        return new PieceSaveData
        {
            pieceIndex = _cut.pieceIndex,
            position = transform.position,
            rotation = transform.rotation
        };
    }
}
