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
        [CanBeNull] public List<ChunkSaveData> chunks;
        
        [NonSerialized]
        public Texture2D PuzzleImage;
        
        public bool HasLocalID => localID != null;

        public PuzzleSaveData(
            string localID,
            string onlineID,
            string name,
            string author,
            PuzzleLayout layout,
            List<ChunkSaveData> chunks,
            Texture2D puzzleImage
        ) {
            this.localID = localID;
            this.onlineID = onlineID;
            this.name = name;
            this.author = author;
            this.layout = layout;
            this.chunks = chunks;
            this.PuzzleImage = puzzleImage;
        }

       
    }
}