using System;
using UnityEngine;

namespace Networking
{
    public class PuzzleServerApiInitializer : MonoBehaviour
    {
        [SerializeField] private MetaQuestAuthenticationManager authManager;
        [SerializeField] private string baseUrl = "http://localhost:8080";

        private void Awake()
        {
            PuzzleServerApi.Initialize(authManager, baseUrl);
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            PuzzleServerApi.Shutdown();
        }
    }
}