using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LiteDB;
using UnityEngine;

namespace Persistence
{
    public sealed class LocalSave
    {
        private static LocalSave _instance;

        public static LocalSave Instance => _instance ?? throw new NullReferenceException("There must be one instance of LocalSave");
        
        public LiteDatabase DB;
        private ILiteCollection<BsonDocument> _puzzles;

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
            return saveData;
        }
        
        private LocalSave(string dbPath)
        {
            DB = new LiteDatabase(dbPath);
            _puzzles = DB.GetCollection("puzzles");
        }
        
        public static void Initialize(string dbPath)
        {
            _instance = new LocalSave(dbPath);
        }

        public static void Shutdown()
        {
            _instance?.DB.Dispose();
            _instance = null;
        }
        
        public void Create(PuzzleSaveData saveData)
        {
            var id = _puzzles.Insert(ToDocument(saveData));
            saveData.localID = id.AsObjectId.ToString();
        }

        public void Save(PuzzleSaveData saveData)
        {
            _puzzles.Upsert(ToDocument(saveData));
        }

        public void SaveAll(IEnumerable<PuzzleSaveData> saveDataList)
        {
            DB.BeginTrans();
            try
            {
                foreach (var saveData in saveDataList)
                {
                    _puzzles.Upsert(ToDocument(saveData));
                }

                DB.Commit();
            }
            catch
            {
                DB.Rollback();
                throw;
            }
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
        }
    }
}