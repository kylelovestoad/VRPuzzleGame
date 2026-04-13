using System;
using PuzzleGeneration;

namespace Networking.Request
{
    [Serializable]
    public class CreatePuzzleRequest
    {
        public string name;
        public PuzzleLayout layout;

        public CreatePuzzleRequest(string name, PuzzleLayout layout)
        {
            this.name = name;
            this.layout = layout;
        }
    }
}