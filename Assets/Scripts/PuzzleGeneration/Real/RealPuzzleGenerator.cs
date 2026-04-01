using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace PuzzleGeneration.Real
{
    public class RealPuzzleGenerator : IPuzzleGenerator
    {
        private const string URL = "http://localhost:6969/real-puzzle";
        private const string MultiPartFormFilename = "puzzle.png";
        
        public async Task<PuzzleGenerationData> Generate(
            Texture2D image, 
            int rows, 
            int cols, 
            float puzzleHeight
        )
        {
            try
            {
                var imageBytes = image.EncodeToPNG();
                var formData = new List<IMultipartFormSection>
                {
                    new MultipartFormFileSection(
                        "file", 
                        imageBytes, 
                        MultiPartFormFilename, 
                        "image/png"
                    )
                };

                var request = UnityWebRequest.Post(URL, formData);
                request.downloadHandler = new DownloadHandlerBuffer();

                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    var response = JsonUtility.FromJson<PuzzleResponse>(request.downloadHandler.text);
                    return response.ToPuzzleRenderData();
                }
                else
                {
                    Debug.LogError(request.error);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Puzzle generation failed: {e}");
            }

            return null;
        }
    }
}
