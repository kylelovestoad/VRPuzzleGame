using UnityEngine;

public class Chunk : MonoBehaviour
{
    private BoxCollider boxCollider;
    
    private float _min_x = float.MaxValue;
    private float _max_x = float.MinValue;
    
    private float _min_y = float.MaxValue;
    private float _max_y = float.MinValue;
    
    private float _min_z = float.MaxValue;
    private float _max_z = float.MinValue;

    private Piece _rep_piece;
    
    public static Chunk CreateSinglePieceChunk(Piece piece)
    {
        Debug.Log("Creating Single Piece Chunk");
        
        GameObject chunkObject = new GameObject("Chunk");
        chunkObject.tag = "Chunk";
        chunkObject.transform.position = piece.transform.position;
        
        Rigidbody chunkRigidBody = chunkObject.AddComponent<Rigidbody>();
        chunkRigidBody.isKinematic = true;
        chunkRigidBody.useGravity = false;
        
        Chunk chunk = chunkObject.AddComponent<Chunk>();
        chunk.SetBounds(piece.Verticies());
        chunk.CreateBoxCollider();

        piece.transform.SetParent(chunkObject.transform);
        
        return chunk;
    }

    private void SetBounds(Vector3[] vertices)
    {
        foreach (Vector3 vertex in vertices)
        {
            _min_x = Mathf.Min(_min_x, vertex.x);
            _max_x = Mathf.Max(_max_x, vertex.x);
    
            _min_y = Mathf.Min(_min_y, vertex.y);
            _max_y = Mathf.Max(_max_y, vertex.y);
    
            _min_z = Mathf.Min(_min_z, vertex.z);
            _max_z = Mathf.Max(_max_z, vertex.z);
        }
        
        Debug.Log("X: " + _min_x + " " + _max_x);
        Debug.Log("Y: " + _min_y + " " + _max_y);
        Debug.Log("Z: " + _min_z  + " " + _max_z);
    }

    private void CreateBoxCollider()
    {
        boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        
        // TODO: Add Threshold
        float width = _max_x - _min_x;
        float height = _max_y - _min_y;
        float depth = _max_z - _min_z;
        
        boxCollider.size = new Vector3(width, height, depth);
        boxCollider.center = new Vector3(width / 2, height / 2, depth / 2);
        
        Debug.Log("Box Collider: " + boxCollider.transform.position);
    }
    
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Chunk"))
        {
            Debug.Log("Collided with Other Chunk");
        }
    }
}
