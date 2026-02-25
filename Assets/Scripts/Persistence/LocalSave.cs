using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LiteDB;
using UnityEngine;

namespace Persistence
{
    public sealed class LocalSave : MonoBehaviour
    {
        private static LocalSave _instance;

        public static LocalSave Instance => _instance == null ?
            throw new NullReferenceException("There must be one instance of LocalSave") : _instance;

        private LiteDatabase _db;
        private ILiteCollection<BsonDocument> _puzzles;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
    
            _instance = this;
            DontDestroyOnLoad(gameObject);

            Debug.Log(Application.persistentDataPath);
            
            var path = Path.Combine(Application.persistentDataPath, "puzzles.db");
            _db = new LiteDatabase(path);
            _puzzles = _db.GetCollection("puzzles");
        }

        private void OnDestroy()
        {
            _db?.Dispose();
        }

        public static BsonDocument ToDocument(PuzzleSaveData saveData)
        {
            var json = JsonUtility.ToJson(saveData);
            var doc  = JsonSerializer.Deserialize(json).AsDocument;
            doc.Remove("localID");
            if (saveData.HasLocalID)
                doc["_id"] = new ObjectId(saveData.localID);
            return doc;
        }
        
        public static PuzzleSaveData FromDocument(BsonDocument doc)
        {
            var json = JsonSerializer.Serialize(doc);
            var saveData = JsonUtility.FromJson<PuzzleSaveData>(json);
            saveData.localID = doc["_id"].AsObjectId.ToString();
            Debug.Assert(saveData.localID != null, "local ID must not be null"); 
            return saveData;
        }

        public void Create(PuzzleSaveData saveData)
        {
            var id = _puzzles.Insert(ToDocument(saveData));
            saveData.localID = id.ToString();
        }

        public void Save(PuzzleSaveData saveData)
        {
            _puzzles.Upsert(ToDocument(saveData));
        }

        public void SaveAll(IEnumerable<PuzzleSaveData> saveDataList)
        {
            _db.BeginTrans();
            try
            {
                foreach (var saveData in saveDataList)
                {
                    _puzzles.Upsert(ToDocument(saveData));
                }

                _db.Commit();
            }
            catch
            {
                _db.Rollback();
                throw;
            }
        }

        public PuzzleSaveData Load(long localId)
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
        }
    }
}