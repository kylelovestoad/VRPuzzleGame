using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace Persistence
{
    [Serializable]
    public record PuzzleSaveData
    {
        public long localID;
        public long? OnlineID;
        public string name;
        public string description;
        public string author;
        public long seed;
        public PieceShape shape;
        public List<ChunkSaveData> chunks;

        public PuzzleSaveData(
            long localID,
            long? onlineID,
            string name,
            string description,
            string author,
            long seed,
            PieceShape shape,
            List<ChunkSaveData> chunks)
        {
            this.localID = localID;
            this.OnlineID = onlineID;
            this.name = name;
            this.description = description;
            this.author = author;
            this.seed = seed;
            this.shape = shape;
            this.chunks = chunks;
        }
    }
}