using System;
using System.Collections.Generic;
using System.Linq;
using Persistence;
using PuzzleGeneration;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Chunk : MonoBehaviour
{
    private const float CollisionDistanceThreshold = 0.01f;

    [SerializeField] 
    private Piece piecePrefab;
    
    private BoxCollider _boxCollider;

    private Piece[] pieces => GetComponentsInChildren<Piece>();
    public int PieceCount => pieces.Length;

    private Puzzle _puzzle;

    public void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
    }

    public void InitializeSinglePieceChunk(PieceCut cut, Puzzle puzzle)
    {
        _boxCollider = GetComponent<BoxCollider>();
        _puzzle = puzzle;
        
        var piece = GetComponentInChildren<Piece>();
        piece.InitializePiece(cut, puzzle.RenderData);
        
        InitializeBoxCollider();
    }
    
    public void InitializeMultiplePieceChunk(
        ChunkSaveData chunkStateData,
        Puzzle puzzle
    ) {
        _boxCollider = GetComponent<BoxCollider>();
        _puzzle = puzzle;
        
        var piecesSaveDataList = chunkStateData.pieces;
        
        var firstPiece = GetComponentInChildren<Piece>();
        var firstPieceSaveData = piecesSaveDataList[0];
        var initialPieceCuts = puzzle.Layout.initialPieceCuts;
        
        firstPiece.transform.position = firstPieceSaveData.position;
        firstPiece.transform.rotation = firstPieceSaveData.rotation;
        firstPiece.InitializePiece(initialPieceCuts[firstPieceSaveData.pieceIndex], puzzle.RenderData);
        
        for (int i = 1; i < piecesSaveDataList.Count; i++)
        {
            PieceSaveData currPiece = piecesSaveDataList[i];
            PieceCut currCut = initialPieceCuts[currPiece.pieceIndex];
                
            Piece piece = Instantiate(piecePrefab, currPiece.position, currPiece.rotation);
            piece.InitializePiece(currCut, puzzle.RenderData);
        }

        InitializeBoxCollider();
    }

    private void InitializeBoxCollider()
    {
        var initialPieces = GetComponentsInChildren<Piece>();
        
        var tightBounds = VertexBounds(initialPieces[0].Vertices());

        for (int i = 1; i < initialPieces.Length; i++)
        {
            var currBounds = VertexBounds(initialPieces[i].Vertices());
            tightBounds.Encapsulate(currBounds);
        }
        
        _boxCollider.center = transform.InverseTransformPoint(tightBounds.center);
        _boxCollider.size = tightBounds.size + Vector3.one * (CollisionDistanceThreshold * 2);
    }

    private void UpdateBoxCollider(Piece[] pieces)
    {
        _boxCollider.size -= Vector3.one * (CollisionDistanceThreshold * 2);
        var tempBounds = _boxCollider.bounds;
        
        foreach (var piece in pieces)
        {
            var currBounds = VertexBounds(piece.Vertices());
            tempBounds.Encapsulate(currBounds);
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

    private void Merge(Chunk other)
    {
        UpdateBoxCollider(other.pieces);

        Piece repPiece = pieces[0];

        // avoid unity complaining about modifying piece during iteration, no foreach
        int piecesCount = other.PieceCount;

        for (int i = 0; i < piecesCount; i++)
        {
            Piece otherPiece = other.pieces[i];

            otherPiece.SnapIntoPlace(repPiece);
            otherPiece.transform.SetParent(transform);
        }

        _puzzle.RemoveChunk(other);

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
        Debug.Log("OnTriggerStay");
        
        Chunk otherChunk = other.GetComponent<Chunk>();

        if (otherChunk != null)
        {
            Debug.Log("Collided with Other Chunk");

            // avoid duplicate merging
            if (GetInstanceID() >= otherChunk.GetInstanceID()) return;

            Piece rep = pieces[0];
            Piece otherRep = otherChunk.pieces[0];

            if (rep.IsRelativelyClose(otherRep))
            {
                Merge(otherChunk);
            }
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
