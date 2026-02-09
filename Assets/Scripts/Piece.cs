using System.Linq;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(BoxCollider))]
public class Piece : MonoBehaviour
{
    private const float ConnectionDistanceThreshold = 0.01f;
    
    private Vector3 _solutionLocation;
    
    public void InitializeVariant(
        Vector3 solutionLocation, 
        Mesh mesh,
        Material material
    ) {
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material = material;
        
        Debug.Log(material);
        
        Bounds bounds = mesh.bounds;
        BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
        boxCollider.center = bounds.center;
        boxCollider.size = bounds.size;
        
        _solutionLocation = solutionLocation;
    }

    public Vector3[] Verticies()
    {
        return gameObject
            .GetComponent<MeshFilter>()
            .mesh
            .vertices
            .Select(vertex => transform.TransformPoint(vertex))
            .ToArray();
    }

    public float SolutionOffsetX()
    {
        return transform.position.x - _solutionLocation.x;
    }
    
    public float SolutionOffsetY()
    {
        return transform.position.y - _solutionLocation.y;
    }
    
    public float SolutionOffsetZ()
    {
        return transform.position.z - _solutionLocation.z;
    }

    public bool IsRelativelyClose(Piece other)
    {
        float xOffset = SolutionOffsetX();
        float yOffset = SolutionOffsetY();
        float zOffset = SolutionOffsetZ();
        
        float xOffsetOther = other.SolutionOffsetX();
        float yOffsetOther = other.SolutionOffsetY();
        float zOffsetOther = other.SolutionOffsetZ();

        float xDiff = math.abs(xOffset - xOffsetOther);
        float yDiff = math.abs(yOffset - yOffsetOther);
        float zDiff = math.abs(zOffset - zOffsetOther);
        
        Debug.Log("XDiff: " + xDiff);
        Debug.Log("YDiff: " + yDiff);
        Debug.Log("ZDiff: " + zDiff);
        
        return xDiff < ConnectionDistanceThreshold 
               && yDiff < ConnectionDistanceThreshold 
               && zDiff < ConnectionDistanceThreshold;
    }
}
