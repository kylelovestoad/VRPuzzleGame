using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Chunk : MonoBehaviour
{
    private const float CollisionDistanceThreshold = 0.01f;
    
    private BoxCollider _boxCollider;

    private List<Piece> _pieces;
    private bool _isCombined;

    void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
    }

    public void InitializeVariant(
        Vector3 solutionLocation,
        Mesh mesh,
        Material material
    ) {
        _boxCollider = GetComponent<BoxCollider>();
        Debug.Log("Initializing Single Piece Chunk");
        
        Piece piece = GetComponentInChildren<Piece>();
        
        piece.InitializeVariant(solutionLocation, mesh, material);

        InitializeBoxCollider(piece.Verticies());
        InsertInitialPiece(piece);

        piece.transform.SetParent(transform);
    }

    private void InitializeBoxCollider(Vector3[] vertices)
    {
        Bounds tightBounds = VertexBounds(vertices);
        
        _boxCollider.center = transform.InverseTransformPoint(tightBounds.center);
        _boxCollider.size = tightBounds.size + Vector3.one * (CollisionDistanceThreshold * 2);
    }

    private void UpdateBoxCollider(List<Piece> pieces)
    {
        _boxCollider.size -= Vector3.one * (CollisionDistanceThreshold * 2);
        Bounds tempBounds = _boxCollider.bounds;
        
        foreach (Piece piece in pieces)
        {
            tempBounds.Encapsulate(VertexBounds(piece.Verticies()));
        }

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

    private void InsertInitialPiece(Piece piece)
    {
        _pieces = new List<Piece> { piece };
    }

    private void Combine(Chunk other)
    {
        UpdateBoxCollider(other._pieces);
            
        Piece repPiece = _pieces[0];

        // avoid unity complaining about modifying piece during iteration, no foreach
        int piecesCount = other._pieces.Count;
        
        for (int i = 0; i < piecesCount; i++)
        {
            Piece otherPiece = other._pieces[i];

            otherPiece.SnapIntoPlace(repPiece);
            otherPiece.transform.SetParent(transform);
            
            _pieces.Add(otherPiece);
        }

        other._isCombined = true;
        
        #if UNITY_EDITOR
        if (Application.isPlaying)
            Destroy(other.gameObject);
        else
            DestroyImmediate(other.gameObject);
        #else
            Destroy(other.gameObject);
        #endif
        
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
