using System;

namespace Networking.Request
{
    [Serializable]
    public class UpsertLeaderboardEntryRequest
    {
        public float time;

        public UpsertLeaderboardEntryRequest(float time)
        {
            this.time = time;
        }
    }
}