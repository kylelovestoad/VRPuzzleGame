using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class Chunk : MonoBehaviour
{
    private const float CollisionDistanceThreshold = 0.1f;
    
    private BoxCollider _boxCollider;
    private Rigidbody _rigidbody;

    private List<Piece> _pieces;
    private bool _isCombined;

    void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void InitializeVariant(Piece piece)
    {
        Debug.Log("Initializing Single Piece Chunk");

        InitializeBoxCollider(piece.Verticies());
        InsertInitialPiece(piece);

        piece.transform.SetParent(transform);
    }

    private void InitializeBoxCollider(Vector3[] vertices)
    {
        // _boxCollider = gameObject.AddComponent<BoxCollider>();
        // _boxCollider.isTrigger = true;
        
        Bounds tightBounds = VertexBounds(vertices);
        
        _boxCollider.center = transform.InverseTransformPoint(tightBounds.center);
        _boxCollider.size = tightBounds.size + Vector3.one * (CollisionDistanceThreshold * 2);
    }

    private void UpdateBoxCollider(Vector3[] vertices)
    {
        _boxCollider.size -= new Vector3(
            CollisionDistanceThreshold * 2,
            CollisionDistanceThreshold * 2,
            CollisionDistanceThreshold * 2
        );
            
        Bounds tempBounds = VertexBounds(vertices);
        tempBounds.Encapsulate(_boxCollider.bounds);
        
        _boxCollider.center = transform.InverseTransformPoint(tempBounds.center);
        _boxCollider.size = tempBounds.size + Vector3.one * (CollisionDistanceThreshold * 2);
    }

    private static Bounds VertexBounds(Vector3[] vertices)
    {
        Bounds bounds = new Bounds(vertices[0], Vector3.zero);
    
        foreach (Vector3 vertex in vertices)
        {
            bounds.Encapsulate(vertex);
        }

        return bounds;
    }

    // private void CreateRigidBody()
    // {
    //     _rigidbody = gameObject.AddComponent<Rigidbody>();
    //     _rigidbody.isKinematic = true;
    //     _rigidbody.useGravity = false;
    // }

    private void InsertInitialPiece(Piece piece)
    {
        _pieces = new List<Piece> { piece };
    }

    private void Combine(Chunk other)
    {
        Piece repPiece = _pieces[0];
        float offsetX = repPiece.SolutionOffsetX();
        float offsetY = repPiece.SolutionOffsetY();
        float offsetZ = repPiece.SolutionOffsetZ();

        // avoid unity complaining about modifying piece during iteration, no foreach
        int piecesCount = other._pieces.Count;
        
        for (int i = 0; i < piecesCount; i++)
        {
            Piece otherPiece = other._pieces[i];
            
            float otherOffsetX = otherPiece.SolutionOffsetX();
            float otherOffsetY = otherPiece.SolutionOffsetY();
            float otherOffsetZ = otherPiece.SolutionOffsetZ();
            
            otherPiece.transform.position += new Vector3(
                offsetX - otherOffsetX, 
                offsetY - otherOffsetY, 
                offsetZ - otherOffsetZ
            );
            
            otherPiece.transform.SetParent(transform);
            _pieces.Add(otherPiece);
        }

        foreach (Piece piece in other._pieces)
        {
            UpdateBoxCollider(piece.Verticies());
        }

        other._isCombined = true;
        Destroy(other.gameObject);
        
        Debug.Log(transform.position);
    }
    
    void OnTriggerStay(Collider other)
    {
        Chunk otherChunk = other.GetComponent<Chunk>();
        
        if (otherChunk != null)
        {
            Debug.Log("Collided with Other Chunk");
            
            // avoid duplicate merging
            if (otherChunk._isCombined || GetInstanceID() >= otherChunk.GetInstanceID()) return;
        
            Piece rep = _pieces[0];
            Piece otherRep = otherChunk._pieces[0];

            if (rep.IsRelativelyClose(otherRep))
            {
                Combine(otherChunk);
            }
        }
    }
}
