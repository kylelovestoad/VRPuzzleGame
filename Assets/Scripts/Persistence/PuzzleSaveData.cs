using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace Persistence
{
    [Serializable]
    public class PuzzleSaveData
    {
        public long localID;
        public long? OnlineID;
        public string name;
        public string description;
        public string author;
        public long seed;
        public PieceShape shape;
        public List<ChunkSaveData> chunks;
    }
}