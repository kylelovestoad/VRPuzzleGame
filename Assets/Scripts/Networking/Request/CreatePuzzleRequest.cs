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

        public CreatePuzzleRequest(string name, string author, PuzzleLayout layout)
        {
            this.name = name;
            this.author = author;
            this.layout = layout;
        }
    }
}