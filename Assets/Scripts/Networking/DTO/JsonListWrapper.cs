using System;
using System.Collections.Generic;

// Solution to deal with Unity's garbage JsonUtility
// Found here: discussions.unity.com/t/jsonutilities-tojson-with-list-string-not-working-as-expected/753352/11
namespace Networking.DTO
{
    [Serializable]
    public class JsonListWrapper<T>
    {
        public List<T> list;
        public JsonListWrapper(List<T> list) => this.list = list;
    }
}