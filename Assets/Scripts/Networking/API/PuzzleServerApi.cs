using System;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace Networking.API
{
    public class PuzzleServerApi
    {
        private static PuzzleServerApi _instance;
        public static PuzzleServerApi Instance => _instance ?? throw new NullReferenceException("There must be one instance of PuzzleServerApi");

        public readonly MetaQuestAuthenticationManager Manager;
        public readonly PuzzleApi Puzzles;
        public readonly LeaderboardApi Leaderboards;
        public readonly ContentApi Content;

        private PuzzleServerApi(MetaQuestAuthenticationManager manager, string baseUrl)
        {
            Manager = manager;
            Puzzles = new PuzzleApi(this, baseUrl);
            Leaderboards = new LeaderboardApi(this, baseUrl);
            Content = new ContentApi(this, baseUrl);
        }

        public static void Initialize(MetaQuestAuthenticationManager manager, string baseUrl)
        {
            if (_instance != null)
                throw new InvalidOperationException("PuzzleServerApi has already been initialized");

            _instance = new PuzzleServerApi(manager, baseUrl);
        }

        public static void Shutdown()
        {
            _instance = null;
        }

        public async Task AddAuthHeaders(UnityWebRequest request)
        {
            var user = await Manager.GetUser();
            request.SetRequestHeader("Puzzle-Meta-User-Id", user.ID.ToString());
            request.SetRequestHeader("Puzzle-Meta-User-Access-Token", await Manager.GetAccessToken());
        }
    }
}