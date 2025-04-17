using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameModules
{
    public class DictionaryPool<Key, Value> : PoolBase
    {
        private static DictionaryPool<Key, Value> Instance;

        private Stack<Dictionary<Key, Value>> pool;

        private DictionaryPool()
        {
        }

        private static void Init()
        {
            if (Instance == null)
            {
                Instance = new DictionaryPool<Key, Value>();
                Instance.pool = new Stack<Dictionary<Key, Value>>();
                PublicPoolMgr.AllPool.Add(Instance);
            }
        }

        public static Dictionary<Key, Value> Get()
        {
            Init();

            if (Instance.pool.Count > 0)
            {
                return Instance.pool.Pop();
            }
            else
            {
                return new Dictionary<Key, Value>();
            }
        }

        public static void Release(Dictionary<Key, Value> dict)
        {
            if (dict == null || Instance == null) return;
            dict.Clear();
            Instance.pool.Push(dict);
        }

        public void Dispose()
        {
            if (Instance != null)
            {
                if (Instance.pool != null)
                {
                    Instance.pool.Clear();
                    Instance.pool = null;
                }

                Instance = null;
            }
        }
    }
}
