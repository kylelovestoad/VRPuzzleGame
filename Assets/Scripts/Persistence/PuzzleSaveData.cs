using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using LiteDB;
using PuzzleGeneration;
using UnityEngine;
using UnityEngine.Serialization;

namespace Persistence
{
    [Serializable]
    public record PuzzleSaveData
    {
        public string localID;
        public string onlineID;
        public string name;
        public string author;
        public PuzzleLayout layout;
        public float elapsedTime;
        [CanBeNull] public List<ChunkSaveData> chunks;
        
        [NonSerialized]
        public Texture2D PuzzleImage;
        
        public bool HasLocalID => localID != null;
        
        public int PieceCount => layout.initialPieceCuts.Count;

        public PuzzleSaveData(
            string localID,
            string onlineID,
            string name,
            string author,
            PuzzleLayout layout,
            List<ChunkSaveData> chunks,
            Texture2D puzzleImage,
            float elapsedTime = 0
        ) {
            this.localID = localID;
            this.onlineID = onlineID;
            this.name = name;
            this.author = author;
            this.layout = layout;
            this.chunks = chunks;
            PuzzleImage = puzzleImage;
            this.elapsedTime = elapsedTime;
        }

        public long CurrentConnections()
        {
            if (chunks == null || chunks.Count == 0) return 0;
            
            Debug.Log(PieceCount + " " + chunks.Count);

            return PieceCount - chunks.Count + 1;
        }
        
        public float PercentComplete()
        {
            return (float) CurrentConnections() / PieceCount * 100;
        }
    }
}