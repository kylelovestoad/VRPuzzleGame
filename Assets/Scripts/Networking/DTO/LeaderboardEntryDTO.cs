using System;

namespace Networking.DTO
{
    [Serializable]
    public class LeaderboardEntryDTO
    {
        public string id;
        public string puzzleId;
        public string userId;
        public float time;
    }
}