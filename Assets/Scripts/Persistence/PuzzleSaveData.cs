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
        public string authorId;
        public string author;
        public PuzzleLayout layout;
        public float elapsedTime;
        [CanBeNull] public List<ChunkSaveData> chunks;
        
        [NonSerialized]
        public Texture2D PuzzleImage;
        
        public bool HasLocalID => !string.IsNullOrEmpty(localID);
        
        public bool HasOnlineID => !string.IsNullOrEmpty(onlineID);
        
        public int PieceCount => layout.initialPieceCuts.Count;

        public PuzzleSaveData(
            string localID,
            string onlineID,
            string name,
            string authorId,
            string author,
            PuzzleLayout layout,
            List<ChunkSaveData> chunks,
            Texture2D puzzleImage,
            float elapsedTime = 0
        ) {
            this.localID = localID;
            this.onlineID = onlineID;
            this.name = name;
            this.authorId = authorId;
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

        public PuzzleMetadata GetMetaData()
        {
            return new PuzzleMetadata(
                localID, 
                onlineID, 
                name, 
                authorId, 
                author, 
                layout, 
                PuzzleImage
            );
        }
        
        public static PuzzleSaveData FromMetaData(PuzzleMetadata metaData)
        {
            return new PuzzleSaveData(
                metaData.localID, 
                metaData.onlineID, 
                metaData.name, 
                metaData.authorId, 
                metaData.author, 
                metaData.layout, 
                null,
                metaData.PuzzleImage
            );
        }
    }
}