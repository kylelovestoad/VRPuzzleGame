using System;
using UnityEngine;

namespace Persistence
{
    [Serializable]
    public record PieceSaveData
    {
        public int pieceIndex;
        public Vector3 position;
        public Quaternion rotation;
    }
}