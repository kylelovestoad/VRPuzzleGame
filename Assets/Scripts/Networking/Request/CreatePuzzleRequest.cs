using System;
using PuzzleGeneration;

namespace Networking.Request
{
    [Serializable]
    public class CreatePuzzleRequest
    {
        public string name;
        public string author;
        public PuzzleLayout layout;
    }
}