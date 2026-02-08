using System.Linq;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Piece : MonoBehaviour
{
    private const float ConnectionDistanceThreshold = 0.1f;
    
    private Vector3 solutionLocation;
    
    public static Piece Create(
        Vector3 initialPosition, 
        Vector3 solutionLocation, 
        Mesh mesh, 
        Material material
    )
    {
        GameObject pieceObject = new GameObject("Piece");
        
        MeshFilter meshFilter = pieceObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        
        MeshRenderer renderer = pieceObject.AddComponent<MeshRenderer>();
        renderer.material = material;
        
        Debug.Log(material);
        
        Bounds bounds = mesh.bounds;
        BoxCollider collider =  pieceObject.AddComponent<BoxCollider>();
        collider.center = bounds.center;
        collider.size = bounds.size;
        
        pieceObject.transform.position = initialPosition;
        
        Piece piece = pieceObject.AddComponent<Piece>();
        piece.solutionLocation = solutionLocation;
        
        return piece;
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
        return transform.position.x - solutionLocation.x;
    }
    
    public float SolutionOffsetY()
    {
        return transform.position.y - solutionLocation.y;
    }
    
    public float SolutionOffsetZ()
    {
        return transform.position.z - solutionLocation.z;
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
