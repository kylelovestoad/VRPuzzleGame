using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Networking.API;
using Networking.DTO;
using Networking.Request;
using UnityEngine;
using UnityEngine.Networking;

namespace Networking.API
{
    public class PuzzleApi
    {
        private readonly PuzzleServerApi _api;
        private readonly string _baseUrl;

        private const string PuzzlesEndpoint = "/api/puzzles";

        public PuzzleApi(PuzzleServerApi api, string baseUrl)
        {
            _api = api;
            _baseUrl = baseUrl;
        }

        public async Task<PuzzleMetadataDTO[]> GetAllPuzzles()
        {
            using var request = UnityWebRequest.Get($"{_baseUrl}{PuzzlesEndpoint}");
            await _api.AddAuthHeaders(request);
            await request.SendWebRequest();

            return request.result != UnityWebRequest.Result.Success
                ? throw new HttpRequestException(request.error)
                : JsonArray<PuzzleMetadataDTO>.FromJson(request.downloadHandler.text);
        }

        public async Task<PuzzleMetadataDTO> GetPuzzle(string id)
        {
            using var request = UnityWebRequest.Get($"{_baseUrl}{PuzzlesEndpoint}/{id}");
            await _api.AddAuthHeaders(request);
            await request.SendWebRequest();

            return request.result != UnityWebRequest.Result.Success
                ? throw new HttpRequestException(request.error)
                : JsonUtility.FromJson<PuzzleMetadataDTO>(request.downloadHandler.text);
        }

        public async Task<PuzzleMetadataDTO> CreatePuzzle(CreatePuzzleRequest metadata, Texture2D image)
        {
            var form = BuildMultipartPuzzleMetadata(JsonUtility.ToJson(metadata), image);
            using var request = UnityWebRequest.Post($"{_baseUrl}{PuzzlesEndpoint}", form);
            await _api.AddAuthHeaders(request);
            await request.SendWebRequest();

            return request.result != UnityWebRequest.Result.Success
                ? throw new HttpRequestException(request.error)
                : JsonUtility.FromJson<PuzzleMetadataDTO>(request.downloadHandler.text);
        }

        public async Task<PuzzleMetadataDTO> UpdatePuzzle(string id, UpdatePuzzleRequest metadata, [CanBeNull] Texture2D image)
        {
            var form = BuildMultipartPuzzleMetadata(JsonUtility.ToJson(metadata), image);
            using var request = UnityWebRequest.Post($"{_baseUrl}{PuzzlesEndpoint}/{id}", form);
            request.method = "PUT";
            await _api.AddAuthHeaders(request);
            await request.SendWebRequest();

            return request.result != UnityWebRequest.Result.Success
                ? throw new HttpRequestException(request.error)
                : JsonUtility.FromJson<PuzzleMetadataDTO>(request.downloadHandler.text);
        }

        public async Task DeletePuzzle(string id)
        {
            using var request = UnityWebRequest.Delete($"{_baseUrl}{PuzzlesEndpoint}/{id}");
            await _api.AddAuthHeaders(request);
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                throw new Exception(request.error);
        }

        private static List<IMultipartFormSection> BuildMultipartPuzzleMetadata(string metadataJson, Texture2D image)
        {
            return new List<IMultipartFormSection>
            {
                new MultipartFormDataSection(
                    "metadata",
                    Encoding.UTF8.GetBytes(metadataJson),
                    "application/json"),
                new MultipartFormFileSection(
                    "image",
                    image.EncodeToPNG(),
                    "puzzle.png",
                    "image/png")
            };
        }
    }
}