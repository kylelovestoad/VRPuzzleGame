#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using Persistence;
using PuzzleGeneration;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(BoxCollider))]
public class Chunk : MonoBehaviour
{
    private const float CollisionDistanceThreshold = 0.01f;

    [SerializeField] 
    private Piece piecePrefab;
    
    private BoxCollider BoxCollider => GetComponent<BoxCollider>();
    private Piece[] Pieces => GetComponentsInChildren<Piece>();
    public int PieceCount => Pieces.Length;

    public bool IsMerged { set; get; }

    private int _isCollisionProcedureRunning;

    public void InitializeSinglePieceChunk(PieceCut cut, PuzzleSaveData saveData)
    {
        gameObject.SetActive(true);
        
        Piece piece = Instantiate(piecePrefab, transform);
        piece.InitializePiece(cut, saveData);
        
        InitializeBoxCollider();
    }
    
    public void InitializeMultiplePieceChunk(
        ChunkSaveData chunkStateData,
        PuzzleSaveData saveData
    ) {
        gameObject.SetActive(true);
        
        var piecesSaveDataList = chunkStateData.pieces;
        var initialPieceCuts = saveData.layout.initialPieceCuts;

        Debug.Log("Pieces Len: " + piecesSaveDataList.Count);
        
        foreach (var currPiece in piecesSaveDataList)
        {
            PieceCut currCut = initialPieceCuts[currPiece.pieceIndex];
                
            Piece piece = Instantiate(piecePrefab, currPiece.position, currPiece.rotation, transform);
            piece.InitializePiece(currCut, saveData);
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

    public List<PieceMissingConnections> MissingConnections()
    {
        var pieceIndices = Pieces.Select(piece => piece.PieceIndex).ToHashSet();
        
        var pieces = new List<PieceMissingConnections>();
        
        foreach (var piece in Pieces)
        {
            var unconnectedNeighbors = piece.NeighborIndices
                .Where(neighborIndex => !pieceIndices.Contains(neighborIndex)).ToList();

            if (unconnectedNeighbors.Count == 0) continue;
            
            var currMissingConnections = new PieceMissingConnections(piece, unconnectedNeighbors);
            pieces.Add(currMissingConnections);
        }

        return pieces;
    }
    
    public bool TryLookupPiece(int pieceIndex, out Piece? piece)
    {
        piece = Pieces.FirstOrDefault(p => p.PieceIndex == pieceIndex);
        return piece is not null;
    }

    private void Merge(Chunk other)
    {
        UpdateBoxCollider(other.Pieces);

        var repPiece = Pieces[0];

        foreach (var otherPiece in other.Pieces.ToArray())
        {
            otherPiece.SnapIntoPlace(repPiece);
            otherPiece.transform.SetParent(transform);
        }

        other.IsMerged = true;
        
    #if UNITY_INCLUDE_TESTS
        DestroyImmediate(other.gameObject);
    #else
        Destroy(other.gameObject);
    #endif
    }

    void OnTriggerStay(Collider other)
    {
        var otherChunk = other.GetComponent<Chunk>();
        
        if (otherChunk == null
            || GetInstanceID() >= otherChunk.GetInstanceID()
            || otherChunk.IsMerged
            || Interlocked.Exchange(ref _isCollisionProcedureRunning, 1) == 1)
        {
            return;
        }
        
        foreach (var piece in Pieces)
        {
            foreach (var otherPiece in otherChunk.Pieces)
            {
                if (!piece.IsCloseEnough(otherPiece)) continue;
                
                Debug.Log("Close Enough");
                // Debug.LogError($"this: {GetInstanceID()}, other: {otherChunk.GetInstanceID()}");
                Merge(otherChunk);  
                goto end;
            }
        }
        
        end:
        Interlocked.Exchange(ref _isCollisionProcedureRunning, 0);
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
