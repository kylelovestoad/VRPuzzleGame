#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using Oculus.Interaction;
using Persistence;
using PuzzleGeneration;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

// [RequireComponent(typeof(BoxCollider))]
public class Chunk : MonoBehaviour
{
    [SerializeField] 
    private Piece piecePrefab;

    public event Action<Chunk> OnChunkDropped;

    public Piece[] Pieces => GetComponentsInChildren<Piece>();
    public int PieceCount => Pieces.Length;

    public int mergeProcedureRunning;

    public void InitializeSinglePieceChunk(
        PieceCut cut, 
        PuzzleSaveData saveData
    )
    {
        gameObject.SetActive(true);
        
        var piece = Instantiate(piecePrefab, transform);
        piece.InitializePiece(cut, saveData);
        
        piece.OnDropped += OnPieceDropped;
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
            var currCut = initialPieceCuts[currPiece.pieceIndex];
                
            var piece = Instantiate(piecePrefab, currPiece.position, currPiece.rotation, transform);
            piece.InitializePiece(currCut, saveData);

            piece.OnDropped += OnPieceDropped;
        }
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

    public void Merge(Chunk other)
    {
        var repPiece = Pieces[0];

        foreach (var otherPiece in other.Pieces.ToArray())
        {
            otherPiece.SnapIntoPlace(repPiece);
            otherPiece.transform.SetParent(transform);

            otherPiece.OnDropped -= other.OnPieceDropped;
            otherPiece.OnDropped += OnPieceDropped;
        }
    }

    private void OnPieceDropped()
    {
        OnChunkDropped.Invoke(this);
    }

    public bool IsCloseEnough(Chunk otherChunk)
    {
        return Pieces.Any(piece => otherChunk.Pieces.Any(piece.IsCloseEnough));
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
