using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameModules.Pool
{
    public class ListPool<T> : PoolBase
    {
        private static ListPool<T> Instance;

        private Stack<List<T>> pool;

        private ListPool()
        {
        }

        private static void Init()
        {
            if (Instance == null)
            {
                Instance = new ListPool<T>();
                Instance.pool = new Stack<List<T>>();
                PublicPoolMgr.AllPool.Add(Instance);
            }
        }

        public static List<T> Get()
        {
            Init();

            if (Instance.pool.Count > 0)
            {
                return Instance.pool.Pop();
            }
            else
            {
                return new List<T>();
            }
        }

        public static void Release(List<T> list)
        {
            if (list == null || Instance == null) return;
            list.Clear();
            Instance.pool.Push(list);
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