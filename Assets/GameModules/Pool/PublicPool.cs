using System;
using System.Collections;
using System.Collections.Generic;

namespace GameModules
{
    /// <summary>
    /// 公共对象池
    /// </summary>
    public static class PublicPool<T> where T : new()
    {
        private static ObjectPool<T> instancePool;

        private static void Init()
        {
            if (instancePool == null)
            {
                instancePool = new ObjectPool<T>();
                PublicPoolMgr.AllPool.Add(instancePool);
            }
        }

        public static T Get()
        {
            Init();
            return instancePool.Get();
        }

        public static void Release(T obj)
        {
            if (obj == null || instancePool == null) return;

            if (obj is IObject interfac)
            {
                interfac.OnRelease();
            }

            instancePool.Release(obj);
        }

        public static void Dispose()
        {
            instancePool?.Dispose();
            instancePool = null;
        }
    }
    
    public static class PublicPoolMgr
    {
        public static readonly List<PoolBase> AllPool = new List<PoolBase>();

        public static void ReleaseAll()
        {
            foreach (var pool in AllPool)
            {
                pool.Dispose();
            }

            AllPool.Clear();
        }
    }
}
