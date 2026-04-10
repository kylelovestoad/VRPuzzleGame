using System.Collections.Generic;
using System.Linq;
using Oculus.Interaction;
using Persistence;
using PuzzleGeneration;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(BoxCollider))]
public class Piece : MonoBehaviour
{
    private const float ConnectionDistanceThreshold = 0.01f;
    private const float ConnectionRotationThreshold = 45f;

    [SerializeField] 
    private Shader defaultFrontShader;
    [SerializeField] 
    private Shader defaultBackAndSidesShader;
    
    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;
    private Grabbable _grabbable;
    
    private Material[] _normalPuzzleMaterials;
    
    private PieceCut _cut;
    
    private Vector2 SolutionLocation => _cut.solutionLocation;
    public int PieceIndex => _cut.pieceIndex;
    public List<int> NeighborIndices => _cut.neighborIndices;
    public List<Vector2> BorderPoints => _cut.borderPoints;
    
    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshFilter = GetComponent<MeshFilter>();
        _grabbable = GetComponent<Grabbable>();
    }
    
    public void InitializePiece(
        PieceCut pieceCut,
        PuzzleSaveData saveData
    )
    {
        gameObject.SetActive(true);
        
        var pieceMesh = PieceMeshGenerator.PieceMesh(pieceCut.borderPoints);

        _meshFilter.sharedMesh = pieceMesh;
        
        var frontMaterial = new Material(defaultFrontShader)
        {
            mainTexture = saveData.PuzzleImage
        };

        var pieceSolutionLocation = pieceCut.solutionLocation;
        var pieceBounds = pieceMesh.bounds;
        var pieceWidth = pieceBounds.max.x - pieceBounds.min.x;
        var pieceHeight = pieceBounds.max.y - pieceBounds.min.y;
        var puzzleLayout = saveData.layout;
        
        Debug.Log($"Puzzle Width: {puzzleLayout.width}, Puzzle Height: {puzzleLayout.height}");
        
        var uvScale = new Vector2(pieceWidth / puzzleLayout.width, pieceHeight / puzzleLayout.height);
        var uvOffset = new Vector2(
            (pieceSolutionLocation.x + pieceBounds.min.x) / puzzleLayout.width,
            (pieceSolutionLocation.y + pieceBounds.min.y) / puzzleLayout.height
        );
        
        Debug.LogError($"UV Offset: {uvOffset} {pieceCut.pieceIndex}");
        
        frontMaterial.mainTextureOffset = uvOffset;
        frontMaterial.mainTextureScale = uvScale;
        
        Material backAndSidesMaterial = new(defaultBackAndSidesShader)
        {
            color = Color.gray
        };
        
        _normalPuzzleMaterials = new[] { frontMaterial, backAndSidesMaterial };
        _meshRenderer.sharedMaterials = _normalPuzzleMaterials;
        
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
    
    public Bounds GetVertexBounds()
    {
        var vertices = Vertices();
        var min = vertices[0];
        var max = vertices[0];

        foreach (var v in vertices)
        {
            min = Vector3.Min(min, v);
            max = Vector3.Max(max, v);
        }

        var bounds = new Bounds();
        bounds.SetMinMax(min, max);
        return bounds;
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

    public void SetMaterials(Material frontMaterial, Material backAndSidesMaterial)
    {
        Debug.LogError($"UV Scale: {_meshRenderer.sharedMaterials[0].mainTextureScale} {_cut.pieceIndex}");
        Debug.LogError($"UV Offset: {_meshRenderer.sharedMaterials[0].mainTextureOffset} {_cut.pieceIndex}");
        
        var normalFrontMaterial = _normalPuzzleMaterials[0];
        
        frontMaterial.mainTexture = normalFrontMaterial.mainTexture;
        frontMaterial.mainTextureOffset = normalFrontMaterial.mainTextureOffset;
        frontMaterial.mainTextureScale = normalFrontMaterial.mainTextureScale;

        _meshRenderer.sharedMaterials = new[] { frontMaterial, backAndSidesMaterial };
    }
    
    public void ResetMaterials()
    {
        _meshRenderer.sharedMaterials = _normalPuzzleMaterials;
    }
    
    public bool IsGrabbed()
    {
        return _grabbable.SelectingPointsCount > 0;
    }

    public PieceSaveData ToData()
    {
        Debug.Log("Saving Piece: " + _cut.pieceIndex);
        
        return new PieceSaveData
        {
            pieceIndex = _cut.pieceIndex,
            position = transform.position,
            rotation = transform.rotation
        };
    }
}
