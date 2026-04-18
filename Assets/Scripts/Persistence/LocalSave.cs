using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using UnityEngine;

namespace Persistence
{
    public sealed class LocalSave
    {
        private static LocalSave _instance;

        public static LocalSave Instance => _instance ?? throw new NullReferenceException("There must be one instance of LocalSave");
        
        public readonly LiteDatabase DB;
        private readonly ILiteCollection<BsonDocument> _puzzles;
        
        public event Action<List<PuzzleSaveData>> OnSaved;
        public event Action OnDeleted;
        
        private static BsonDocument ToDocument(PuzzleSaveData saveData)
        {
            var json = JsonUtility.ToJson(saveData);
            var doc  = JsonSerializer.Deserialize(json).AsDocument;
            doc.Remove("localID");
            
            if (saveData.HasLocalID)
            {
                Debug.Log("Has Local ID");
                doc["_id"] = new ObjectId(saveData.localID);
            }
            else
            {
                Debug.Log("No Local ID");
            }
                
            return doc;
        }

        private static PuzzleSaveData FromDocument(BsonDocument doc)
        {
            var json = JsonSerializer.Serialize(doc);
            var saveData = JsonUtility.FromJson<PuzzleSaveData>(json);
            
            saveData.localID = doc["_id"].AsObjectId.ToString();
            Debug.Assert(saveData.localID != null, "local ID must not be null"); 
            
            LoadImage(saveData);
            
            return saveData;
        }
        
        private static void SaveImage(PuzzleSaveData saveData)
        {
            var image = saveData.PuzzleImage;
            var imageBytes = image.EncodeToPNG();
            
            var filename = saveData.localID + ".png";
            var path = Path.Combine(Application.persistentDataPath, filename);

            File.WriteAllBytes(path, imageBytes);
        }
        
        private static void LoadImage(PuzzleSaveData saveData)
        {
            var filename = saveData.localID + ".png";
            var path = Path.Combine(Application.persistentDataPath, filename);
            
            var imageBytes = File.ReadAllBytes(path);
            var image = new Texture2D(1, 1);
            image.LoadImage(imageBytes);
            
            saveData.PuzzleImage = image;
        }
        
        private static void DeleteImage(string localID)
        {
            var filename = localID + ".png";
            var path = Path.Combine(Application.persistentDataPath, filename);
            
            File.Delete(path);
        }
        
        private LocalSave(string dbPath)
        {
            DB = new LiteDatabase(dbPath);
            _puzzles = DB.GetCollection("puzzles");
        }
        
        public static void Initialize(string dbPath)
        {
            if (_instance != null)
            {
                throw new InvalidOperationException("PuzzleServerApi has already been initialized.");
            }
            _instance = new LocalSave(dbPath);
        }

        public static void Shutdown()
        {
            _instance?.DB.Dispose();
            _instance = null;
        }
        
        public async Task Create(PuzzleSaveData saveData)
        {
            await Task.Run(() => {
                var id = _puzzles.Insert(ToDocument(saveData));
                saveData.localID = id.AsObjectId.ToString();
                SaveImage(saveData);
            });
            
            OnSaved?.Invoke(new List<PuzzleSaveData> { saveData });
            
        }
        
        public string InsertOnly(PuzzleSaveData saveData)
        {
            var id = _puzzles.Insert(ToDocument(saveData));
            saveData.localID = id.AsObjectId.ToString();
            return saveData.localID;
        }

        public void Save(PuzzleSaveData saveData)
        {
            Debug.Log("Saving..." + saveData.localID);
            
            _puzzles.Upsert(ToDocument(saveData));
            SaveImage(saveData);
            OnSaved?.Invoke(new List<PuzzleSaveData> { saveData });
        }
        
        public void SaveSkipImage(PuzzleSaveData saveData)
        {
            _puzzles.Upsert(ToDocument(saveData));
            OnSaved?.Invoke(new List<PuzzleSaveData> { saveData });
        }

        public void SaveAll(List<PuzzleSaveData> saveDataList)
        {
            DB.BeginTrans();
            try
            {
                foreach (var saveData in saveDataList)
                {
                    _puzzles.Upsert(ToDocument(saveData));
                    SaveImage(saveData);
                }

                DB.Commit();
            }
            catch
            {
                DB.Rollback();
                throw;
            }
            OnSaved?.Invoke(saveDataList);
        }

        public PuzzleSaveData Load(string localId)
        {
            var doc = _puzzles.FindById(localId);
            return doc == null ? null : FromDocument(doc);
        }

        public IEnumerable<PuzzleSaveData> LoadAll()
        {
            return _puzzles.FindAll().Select(FromDocument);
        }

        public void Delete(string localId)
        {
            _puzzles.Delete(new ObjectId(localId));
            DeleteImage(localId);
            
            OnDeleted?.Invoke();
        }
    }
}