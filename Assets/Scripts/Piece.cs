using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(BoxCollider))]
public class Piece : MonoBehaviour
{
    private const float ConnectionDistanceThreshold = 0.01f;
    private const float ConnectionRotationThreshold = 45f;
    
    private Vector3 _solutionLocation;
    
    public void InitializeVariant(
        Vector3 solutionLocation, 
        Mesh mesh,
        Material material
    ) {
        Debug.Log("Initialize Piece Variant");
        
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;
        
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = material;
        
        Debug.Log(material);
        
        Bounds bounds = mesh.bounds;
        BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
        boxCollider.center = bounds.center;
        boxCollider.size = bounds.size;
        
        _solutionLocation = solutionLocation;
    }

    public Vector3[] Verticies()
    {
        Debug.Log("Verticies " + gameObject.GetComponent<MeshFilter>()
            .sharedMesh
            .vertices.Select(vertex => transform.TransformPoint(vertex)).ToArray());
        
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
        Vector3 expectedPosition = ExpectedPosition(other);
        Vector3 actualPosition = transform.position;
        
        return Vector3.Distance(expectedPosition, actualPosition) < ConnectionDistanceThreshold 
               && Quaternion.Angle(transform.rotation, other.transform.rotation) < ConnectionRotationThreshold;
    }

    public void SnapIntoPlace(Piece other)
    {
        transform.position = ExpectedPosition(other);
        transform.rotation = other.transform.rotation;
    }
}
