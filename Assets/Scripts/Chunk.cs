using System;
using System.Collections.Generic;
using System.Linq;
using Persistence;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[Serializable]
public class Chunk : MonoBehaviour
{
    private const float CollisionDistanceThreshold = 0.01f;
    
    private BoxCollider _boxCollider;
    
    [SerializeField]
    private List<Piece> pieces;
    
    public long PieceCount => pieces.Count; 

    public void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
    }

    public void InitializeSinglePieceChunk(
        PieceRenderData pieceRenderData
    ) {
        _boxCollider = GetComponent<BoxCollider>();
        Debug.Log("Initializing Single Piece Chunk");
        
        var piece = GetComponentInChildren<Piece>();
        
        piece.InitializePiece(pieceRenderData);

        InitializeBoxCollider(piece.Vertices());
        InsertInitialPiece(piece);

        piece.transform.SetParent(transform);
    }

    private void InitializeBoxCollider(Vector3[] vertices)
    {
        var tightBounds = VertexBounds(vertices);
        
        _boxCollider.center = transform.InverseTransformPoint(tightBounds.center);
        _boxCollider.size = tightBounds.size + Vector3.one * (CollisionDistanceThreshold * 2);
    }

    private void UpdateBoxCollider(List<Piece> pieces)
    {
        _boxCollider.size -= Vector3.one * (CollisionDistanceThreshold * 2);
        var tempBounds = _boxCollider.bounds;
        
        foreach (var piece in pieces)
        {
            tempBounds.Encapsulate(VertexBounds(piece.Vertices()));
        }

        _boxCollider.center = transform.InverseTransformPoint(tempBounds.center);
        _boxCollider.size = tempBounds.size + Vector3.one * (CollisionDistanceThreshold * 2);
    }

    private static Bounds VertexBounds(Vector3[] vertices)
    {
        var bounds = new Bounds(vertices[0], Vector3.zero);
    
        foreach (var vertex in vertices)
        {
            bounds.Encapsulate(vertex);
        }

        return bounds;
    }

    private void InsertInitialPiece(Piece piece)
    {
        pieces = new List<Piece> { piece };
    }

    private void Combine(Chunk other)
    {
        UpdateBoxCollider(other.pieces);
            
        var repPiece = pieces[0];

        // avoid unity complaining about modifying piece during iteration, no foreach
        var piecesCount = other.pieces.Count;
        
        for (var i = 0; i < piecesCount; i++)
        {
            var otherPiece = other.pieces[i];

            otherPiece.SnapIntoPlace(repPiece);
            otherPiece.transform.SetParent(transform);
            
            pieces.Add(otherPiece);
        }
        
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
        var otherChunk = other.GetComponent<Chunk>();

        if (otherChunk == null) return;
        Debug.Log("Collided with Other Chunk");
        
        var rep = pieces[0];
        var otherRep = otherChunk.pieces[0];

        if (rep.IsRelativelyClose(otherRep))
        {
            Combine(otherChunk);
        }
    }
    
    public ChunkSaveData ToData()
    {
        return new ChunkSaveData
        {
            position = transform.position,
            rotation = transform.rotation,
            pieces = this.pieces.Select(p => p.ToData()).ToList()
        };
    }
}
