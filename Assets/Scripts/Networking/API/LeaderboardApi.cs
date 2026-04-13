using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Networking.API;
using Networking.DTO;
using Networking.Request;
using UnityEngine;
using UnityEngine.Networking;

namespace Networking
{
    public class LeaderboardApi
    {
        private readonly PuzzleServerApi _api;
        private readonly string _baseUrl;

        public LeaderboardApi(PuzzleServerApi api, string baseUrl)
        {
            _api = api;
            _baseUrl = baseUrl;
        }

        private string LeaderboardEndpointFor(string puzzleId) =>
            $"{_baseUrl}/api/puzzles/{puzzleId}/leaderboards";

        public async Task<LeaderboardEntryDTO> UpsertLeaderboardEntry(string puzzleId, float time)
        {
            var body = JsonUtility.ToJson(time);
            using var request = new UnityWebRequest(LeaderboardEndpointFor(puzzleId), "POST");
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(body));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            await _api.AddAuthHeaders(request);
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                throw new HttpRequestException(request.error);

            return JsonUtility.FromJson<LeaderboardEntryDTO>(request.downloadHandler.text);
        }

        public async Task<LeaderboardEntryDTO[]> GetLeaderboardEntries(string puzzleId)
        {
            using var request = UnityWebRequest.Get(LeaderboardEndpointFor(puzzleId));
            await _api.AddAuthHeaders(request);
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                throw new Exception(request.error);

            return JsonUtility.FromJson<JsonListWrapper<LeaderboardEntryDTO>>(request.downloadHandler.text)
                .list
                .ToArray();
        }
    }
}