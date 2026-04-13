using System;

// Solution to deal with Unity's garbage JsonUtility
// Found here: discussions.unity.com/t/jsonutilities-tojson-with-list-string-not-working-as-expected/753352/11
namespace Networking.DTO
{
    [Serializable]
    public class JsonArray<T>
    {
        public T[] array;
        public JsonArray(T[] array) => this.array = array;

        public static T[] FromJson(string json)
        {
            var wrapped = $"{{\"array\":{json}}}";
            return UnityEngine.JsonUtility.FromJson<JsonArray<T>>(wrapped).array;
        }
    }
}