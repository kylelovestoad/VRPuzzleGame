using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Persistence
{
    [Serializable]
    public record PieceSaveData
    {
        public Vector3 solutionLocation;
        public Vector3 position;
        public Quaternion rotation;
    }
}