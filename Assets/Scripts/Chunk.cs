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

    public event Action<Chunk, Chunk> OnMerge;
    
    private BoxCollider BoxCollider => GetComponent<BoxCollider>();
    public Piece[] Pieces => GetComponentsInChildren<Piece>();
    public int PieceCount => Pieces.Length;

    private bool _isDestroyQueued;
    private int _isCollisionProcedureRunning;

    public void InitializeSinglePieceChunk(
        PieceCut cut, 
        PuzzleSaveData saveData
    )
    {
        gameObject.SetActive(true);
        
        var piece = Instantiate(piecePrefab, transform);
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

    public Piece FirstPiece()
    {
        return Pieces[0];
    }

    public List<PieceMissingConnections> MissingConnections()
    {
        Debug.Assert(PieceCount > 0, "Chunk must have some pieces");
        
        var pieceIndices = Pieces.Select(piece => piece.PieceIndex).ToHashSet();
        var missingConnections = new List<PieceMissingConnections>();
        
        foreach (var piece in Pieces)
        {
            Debug.LogError($"Neighbors: {piece.NeighborIndices.Count}");
            
            var unconnectedNeighbors = piece.NeighborIndices
                .Where(neighborIndex => !pieceIndices.Contains(neighborIndex)).ToList();

            if (unconnectedNeighbors.Count == 0) continue;
            
            var currMissingConnections = new PieceMissingConnections(piece, unconnectedNeighbors);
            missingConnections.Add(currMissingConnections);
        }

        return missingConnections;
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

        other._isDestroyQueued = true;
        
        OnMerge.Invoke(this, other);

        if (Application.isPlaying)
        {
            Destroy(other.gameObject);
        }
        else
        {
            DestroyImmediate(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        var otherChunk = other.GetComponent<Chunk>();
        
        if (otherChunk == null
            || GetInstanceID() >= otherChunk.GetInstanceID()
            || otherChunk._isDestroyQueued
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
