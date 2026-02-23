using System;
using UnityEngine;

namespace Persistence
{
    [Serializable]
    public record PieceSaveData
    {
        public Vector3[] vertices;
        public int[] triangles;
        public Vector3 solutionLocation;
        public Vector3 position;
        public Quaternion rotation;
    }
}