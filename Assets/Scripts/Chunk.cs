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
    
    private BoxCollider BoxCollider => GetComponent<BoxCollider>();
    private Piece[] Pieces => GetComponentsInChildren<Piece>();
    public int PieceCount => Pieces.Length;

    public void InitializeSinglePieceChunk(PieceCut cut, Puzzle puzzle)
    {
        var piece = GetComponentInChildren<Piece>();
        piece.InitializePiece(cut, puzzle.RenderData);
        
        InitializeBoxCollider();
    }
    
    public void InitializeMultiplePieceChunk(
        ChunkSaveData chunkStateData,
        Puzzle puzzle
    ) {
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
                
            Piece piece = Instantiate(piecePrefab, currPiece.position, currPiece.rotation, transform);
            piece.InitializePiece(currCut, puzzle.RenderData);
        }

        InitializeBoxCollider();
    }

    private void InitializeBoxCollider()
    {
        var initialPieces = GetComponentsInChildren<Piece>();
        var initialBounds = VertexBounds(initialPieces[0].Vertices());;

        SetBoxColliderBounds(initialBounds, initialPieces[1..]);
    }

    private void UpdateBoxCollider(Piece[] newPieces)
    {
        BoxCollider.size -= Vector3.one * (CollisionDistanceThreshold * 2);
        var currBounds = BoxCollider.bounds;
        
        SetBoxColliderBounds(currBounds, newPieces);
    }

    private void SetBoxColliderBounds(Bounds currBounds, Piece[] newPieces)
    {
        foreach (var piece in newPieces)
        {
            var pieceBounds = VertexBounds(piece.Vertices());
            currBounds.Encapsulate(pieceBounds);
        }
        
        BoxCollider.center = transform.InverseTransformPoint(currBounds.center);
        BoxCollider.size = currBounds.size + Vector3.one * (CollisionDistanceThreshold * 2);
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
        UpdateBoxCollider(other.Pieces);

        Piece repPiece = Pieces[0];

        // avoid unity complaining about modifying piece during iteration, no foreach
        int piecesCount = other.PieceCount;

        for (int i = 0; i < piecesCount; i++)
        {
            Piece otherPiece = other.Pieces[i];

            otherPiece.SnapIntoPlace(repPiece);
            otherPiece.transform.SetParent(transform);
        }

        if (Application.isPlaying)
        {
            Destroy(other.gameObject);
        }
        else
        {
            DestroyImmediate(other.gameObject);
        }
    }

    void OnTriggerStay(Collider other)
    {
        Chunk otherChunk = other.GetComponent<Chunk>();

        if (otherChunk != null 
            && GetInstanceID() < otherChunk.GetInstanceID()
            && Pieces.Length > 0 
            && otherChunk.Pieces.Length > 0
        ) {
            Piece repPiece = Pieces[0];
            Piece otherRepPiece = otherChunk.Pieces[0];

            if (repPiece.IsRelativelyClose(otherRepPiece))
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
            pieces = Pieces.Select(piece => piece.ToData()).ToList()
        };
    }
}
