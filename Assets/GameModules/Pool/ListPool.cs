using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameModules
{
    public class ListPool<T> : PoolBase
    {
        public static List<T> Get()
        {
            return PublicPool<List<T>>.Get();
        }
        public static void Release(List<T> list)
        {
            list.Clear();
            PublicPool<List<T>>.Release(list);
        }
        public void Dispose()
        {
            PublicPool<List<T>>.Dispose();
        }
    }
}