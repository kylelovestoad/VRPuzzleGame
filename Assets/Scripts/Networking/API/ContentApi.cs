using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Networking.API
{
    public class ContentApi
    {
        private readonly PuzzleServerApi _api;
        private readonly string _baseUrl;

        private const string ContentEndpoint = "/api/content";

        public ContentApi(PuzzleServerApi api, string baseUrl)
        {
            _api = api;
            _baseUrl = baseUrl;
        }

        public async Task<Texture2D> DownloadImage(string fileId)
        {
            using var request = UnityWebRequestTexture.GetTexture($"{_baseUrl}{ContentEndpoint}/{fileId}");
            await _api.AddAuthHeaders(request);
            await request.SendWebRequest();

            return request.result != UnityWebRequest.Result.Success 
                ? throw new Exception(request.error) 
                : DownloadHandlerTexture.GetContent(request);
        }
    }
}