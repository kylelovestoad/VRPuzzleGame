using System.IO;
using UnityEngine;

namespace Persistence
{
    public class LocalSaveInitializer : MonoBehaviour
    {
        private void Awake()
        {
            var path = Path.Combine(Application.persistentDataPath, "puzzles.db");
            LocalSave.Initialize(path);
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            LocalSave.Shutdown();
        }
    }
}