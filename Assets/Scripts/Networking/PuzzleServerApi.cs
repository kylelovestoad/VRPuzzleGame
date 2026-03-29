using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Networking.DTO;
using Networking.Request;
using UnityEngine;
using UnityEngine.Networking;

namespace Networking
{
    public class PuzzleServerApi
    {
        
        private static PuzzleServerApi _instance;
        public static PuzzleServerApi Instance => _instance ?? throw new NullReferenceException("There must be one instance of PuzzleServerApi");

        public readonly MetaQuestAuthenticationManager Manager;

        private readonly string _baseUrl;

        private const string PuzzlesEndpoint = "/api/puzzles";
        private const string ContentEndpoint = "/api/content";
        
        private PuzzleServerApi(MetaQuestAuthenticationManager manager, string baseUrl)
        {
            Manager = manager;
            _baseUrl = baseUrl;
        }

        public static void Initialize(MetaQuestAuthenticationManager manager, string baseUrl)
        {
            if (_instance != null)
            {
                throw new InvalidOperationException("PuzzleServerApi has already been initialized");
            }

            _instance = new PuzzleServerApi(manager, baseUrl);
        }

        public static void Shutdown()
        {
            _instance = null;
        }

        private async Task AddAuthHeaders(UnityWebRequest request)
        {
            if (Manager.User == null)
            {
                throw new InvalidCredentialException("User is not logged in");
            }
            request.SetRequestHeader("Puzzle-Meta-User-Id", Manager.User.ID.ToString());
            request.SetRequestHeader("Puzzle-Meta-Nonce", await Manager.GetNonce());
        }

        public async Task<PuzzleMetadataDTO[]> GetAllPuzzles()
        {
            using var request = UnityWebRequest.Get($"{_baseUrl}{PuzzlesEndpoint}");
            await AddAuthHeaders(request);
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                throw new Exception(request.error);

            var list = PuzzleMetadataListDTO.Wrap(request.downloadHandler.text);
            return list.items.ToArray();
        }

        public async Task<PuzzleMetadataDTO> GetPuzzle(string id)
        {
            using var request = UnityWebRequest.Get($"{_baseUrl}{PuzzlesEndpoint}/{id}");
            await AddAuthHeaders(request);
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                throw new Exception(request.error);

            return JsonUtility.FromJson<PuzzleMetadataDTO>(request.downloadHandler.text);
        }

        public async Task<PuzzleMetadataDTO> CreatePuzzle(CreatePuzzleRequest metadata, Texture2D image)
        {
            var form = BuildMultipartForm(JsonUtility.ToJson(metadata), image);
            using var request = UnityWebRequest.Post($"{_baseUrl}{PuzzlesEndpoint}", form);
            await AddAuthHeaders(request);
            await request.SendWebRequest();
            
            if (request.result != UnityWebRequest.Result.Success)
                throw new HttpRequestException(request.error);

            return JsonUtility.FromJson<PuzzleMetadataDTO>(request.downloadHandler.text);
        }

        public async Task<PuzzleMetadataDTO> UpdatePuzzle(string id, UpdatePuzzleRequest metadata, [CanBeNull] Texture2D image)
        {
            var form = BuildMultipartForm(JsonUtility.ToJson(metadata), image);
            using var request = UnityWebRequest.Post($"{_baseUrl}{PuzzlesEndpoint}/{id}", form);
            request.method = "PUT";
            await AddAuthHeaders(request);
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                throw new Exception(request.error);

            return JsonUtility.FromJson<PuzzleMetadataDTO>(request.downloadHandler.text);
        }

        public async Task DeletePuzzle(string id)
        {
            using var request = UnityWebRequest.Delete($"{_baseUrl}{PuzzlesEndpoint}/{id}");
            await AddAuthHeaders(request);
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                throw new Exception(request.error);
        }

        public async Task<Texture2D> DownloadImage(string fileId)
        {
            using var request = UnityWebRequestTexture.GetTexture($"{_baseUrl}{ContentEndpoint}/{fileId}");
            await AddAuthHeaders(request);
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