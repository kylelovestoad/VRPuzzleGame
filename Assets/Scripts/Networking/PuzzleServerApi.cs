using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Networking.DTO;
using Networking.Request;
using UnityEngine;
using UnityEngine.Networking;

namespace Networking
{
    public class PuzzleServerApi : MonoBehaviour
    {
        public MetaQuestAuthenticationManager manager;

        [SerializeField] private string baseUrl = "http://localhost:8080";

        private const string PuzzlesEndpoint = "/api/puzzles";
        private const string ContentEndpoint = "/api/content";

        private void AddAuthHeaders(UnityWebRequest request)
        {
            request.SetRequestHeader("X-Meta-User-Id", manager.UserId);
            request.SetRequestHeader("X-Meta-Nonce", manager.Nonce);
        }

        public async Task<PuzzleMetadataDTO[]> GetAllPuzzles()
        {
            using var request = UnityWebRequest.Get($"{baseUrl}{PuzzlesEndpoint}");
            AddAuthHeaders(request);
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                throw new Exception(request.error);

            var list = PuzzleMetadataListDTO.Wrap(request.downloadHandler.text);
            return list.items.ToArray();
        }

        public async Task<PuzzleMetadataDTO> GetPuzzle(string id)
        {
            using var request = UnityWebRequest.Get($"{baseUrl}{PuzzlesEndpoint}/{id}");
            AddAuthHeaders(request);
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                throw new Exception(request.error);

            return JsonUtility.FromJson<PuzzleMetadataDTO>(request.downloadHandler.text);
        }

        public async Task<PuzzleMetadataDTO> CreatePuzzle(CreatePuzzleRequest metadata, Texture2D image)
        {
            var form = BuildMultipartForm(JsonUtility.ToJson(metadata), image);
            using var request = UnityWebRequest.Post($"{baseUrl}{PuzzlesEndpoint}", form);
            AddAuthHeaders(request);
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                throw new Exception(request.error);

            return JsonUtility.FromJson<PuzzleMetadataDTO>(request.downloadHandler.text);
        }

        public async Task<PuzzleMetadataDTO> UpdatePuzzle(string id, UpdatePuzzleRequest metadata, [CanBeNull] Texture2D image)
        {
            var form = BuildMultipartForm(JsonUtility.ToJson(metadata), image);
            using var request = UnityWebRequest.Post($"{baseUrl}{PuzzlesEndpoint}/{id}", form);
            request.method = "PUT";
            AddAuthHeaders(request);
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                throw new Exception(request.error);

            return JsonUtility.FromJson<PuzzleMetadataDTO>(request.downloadHandler.text);
        }

        public async Task DeletePuzzle(string id)
        {
            using var request = UnityWebRequest.Delete($"{baseUrl}{PuzzlesEndpoint}/{id}");
            AddAuthHeaders(request);
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                throw new Exception(request.error);
        }

        public async Task<Texture2D> DownloadImage(string fileId)
        {
            using var request = UnityWebRequestTexture.GetTexture($"{baseUrl}{ContentEndpoint}/{fileId}");
            AddAuthHeaders(request);
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                throw new Exception(request.error);

            return DownloadHandlerTexture.GetContent(request);
        }

        private static List<IMultipartFormSection> BuildMultipartForm(string metadataJson, [CanBeNull] Texture2D image)
        {
            var sections = new List<IMultipartFormSection>
            {
                new MultipartFormDataSection(
                    "metadata",
                    Encoding.UTF8.GetBytes(metadataJson),
                    "application/json")
            };

            if (image)
            {
                sections.Add(new MultipartFormFileSection(
                    "image",
                    image.EncodeToPNG(),
                    "puzzle.png",
                    "image/png"));
            }

            return sections;
        }
    }
}