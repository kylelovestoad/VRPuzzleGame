using System;
using PuzzleGeneration;

namespace Networking.DTO
{
    [Serializable]
    public class PuzzleMetadataDTO
    {
        public string onlineID;
        public string name;
        public string author;
        public PuzzleLayout layout;
        public ContentDTO content;
    }
}