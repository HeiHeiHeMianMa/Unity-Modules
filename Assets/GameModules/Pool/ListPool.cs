using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameModules
{
    public class ListPool<T> : PoolBase
    {
        private static ListPool<T> _instance;

        private Stack<List<T>> _pool;

        private ListPool()
        {
        }

        private static void Init()
        {
            if (_instance == null)
            {
                _instance = new ListPool<T>();
                _instance._pool = new Stack<List<T>>();
                PublicPoolMgr.AllPool.Add(_instance);
            }
        }

        public static List<T> Get()
        {
            Init();

            if (_instance._pool.Count > 0)
            {
                return _instance._pool.Pop();
            }
            else
            {
                return new List<T>();
            }
        }

        public static void Release(List<T> list)
        {
            if (list == null || _instance == null) return;
            list.Clear();
            _instance._pool.Push(list);
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