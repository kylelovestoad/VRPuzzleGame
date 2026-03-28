using System;
using System.Collections.Generic;
using UnityEngine;

namespace Networking.DTO
{
    [Serializable]
    public class PuzzleMetadataListDTO
    {
        public List<PuzzleMetadataDTO> items;

        // A bit hacky
        public static PuzzleMetadataListDTO Wrap(string json)
        {
            return JsonUtility.FromJson<PuzzleMetadataListDTO>($"{{\"items\":{json}}}");
        }
    }
}