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
        private static ObjectPool<T> _instancePool;

        private static void Init()
        {
            if (_instancePool == null)
            {
                _instancePool = new ObjectPool<T>(20);
                PublicPoolMgr.AllPool.Add(_instancePool);
            }
        }

        public static T Get()
        {
            Init();
            return _instancePool.Get();
        }

        public static void Release(T obj)
        {
            if (obj == null || _instancePool == null) return;

            if (obj is IObject interfac)
            {
                interfac.OnRelease();
            }

            _instancePool.Release(obj);
        }

        public static void Dispose()
        {
            _instancePool?.Dispose();
            _instancePool = null;
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
