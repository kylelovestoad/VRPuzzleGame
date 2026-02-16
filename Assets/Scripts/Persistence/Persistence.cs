using System;
using System.IO;
using UnityEngine;

namespace Persistence
{
    public sealed class Persistence : MonoBehaviour
    {
        private static Persistence _instance;
    
        public static Persistence Instance => _instance == null ? 
            throw new NullReferenceException("There must be one instance of StorageService") : _instance;

        private static readonly string PuzzleSaveDirectory = Path.Combine(Application.persistentDataPath, "puzzles");
    
        public static Puzzle CurrentPuzzle { get; private set; }
    
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
        
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
