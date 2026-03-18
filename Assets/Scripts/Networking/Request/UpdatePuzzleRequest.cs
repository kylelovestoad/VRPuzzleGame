using System;
using JetBrains.Annotations;
using PuzzleGeneration;

namespace Networking.Request
{
    [Serializable]
    public class UpdatePuzzleRequest
    {
        [CanBeNull] public string name;
        [CanBeNull] public string author;
        [CanBeNull] public PuzzleLayout layout;
    }
}