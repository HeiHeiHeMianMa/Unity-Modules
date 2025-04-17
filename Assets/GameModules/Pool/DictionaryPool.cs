using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameModules
{
    public class DictionaryPool<Key, Value> : PoolBase
    {
        private static DictionaryPool<Key, Value> _instance;

        private Stack<Dictionary<Key, Value>> _pool;

        private DictionaryPool()
        {
        }

        private static void Init()
        {
            if (_instance == null)
            {
                _instance = new DictionaryPool<Key, Value>();
                _instance._pool = new Stack<Dictionary<Key, Value>>();
                PublicPoolMgr.AllPool.Add(_instance);
            }
        }

        public static Dictionary<Key, Value> Get()
        {
            Init();

            if (_instance._pool.Count > 0)
            {
                return _instance._pool.Pop();
            }
            else
            {
                return new Dictionary<Key, Value>();
            }
        }

        public static void Release(Dictionary<Key, Value> dict)
        {
            if (dict == null || _instance == null) return;
            dict.Clear();
            _instance._pool.Push(dict);
        }

        public void Dispose()
        {
            if (_instance != null)
            {
                if (_instance._pool != null)
                {
                    _instance._pool.Clear();
                    _instance._pool = null;
                }

                _instance = null;
            }
        }
    }
}
