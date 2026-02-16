using System;
using System.Collections.Generic;
using UnityEngine;

namespace Persistence
{
    [Serializable]
    public record ChunkSaveData
    {
        public Vector3 position;
        public Quaternion rotation;
        public List<PieceSaveData> pieces;
    }
}