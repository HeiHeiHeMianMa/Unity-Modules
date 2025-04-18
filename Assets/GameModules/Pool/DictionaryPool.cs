using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameModules
{
    public class DictionaryPool<Key, Value> : PoolBase
    {
        public static Dictionary<Key, Value> Get()
        {
            return PublicPool<Dictionary<Key, Value>>.Get();
        }
        public static void Release(Dictionary<Key, Value> dict)
        {
            dict.Clear();
            PublicPool<Dictionary<Key, Value>>.Release(dict);
        }
        public void Dispose()
        {
            PublicPool<Dictionary<Key, Value>>.Dispose();
        }
    }
}
