using System;
using PuzzleGeneration;
using UnityEngine;

namespace Persistence
{
    
    // TODO use this design as it makes much more sense over having puzzle metadata combined with puzzle save data
    [Serializable]
    public record PuzzleMetadata
    {
        public string localID;
        public string onlineID;
        public string name;
        public string author;
        public PuzzleLayout layout;
        
        [NonSerialized]
        public Texture2D PuzzleImage;
        public bool HasLocalID => localID != null;
        public bool HasOnlineID => onlineID != null;
        public int PieceCount => layout.initialPieceCuts.Count;

        public PuzzleMetadata(
            string localID,
            string onlineID,
            string name,
            string author,
            PuzzleLayout layout,
            Texture2D puzzleImage
        ) {
            this.localID = localID;
            this.onlineID = onlineID;
            this.name = name;
            this.author = author;
            this.layout = layout;
            PuzzleImage = puzzleImage;
        }
    }
}