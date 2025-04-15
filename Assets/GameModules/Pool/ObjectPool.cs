using System;
using System.Collections;
using System.Collections.Generic;

namespace GameModules.Pool
{
    public class ObjectPool<T> : PoolBase where T : new()
    {
        private int maxNUm;
        private Stack<T> pool;

        public ObjectPool(int maxNUm = 10)
        {
            this.maxNUm = maxNUm;
            pool = new Stack<T>();
        }

        public T Get()
        {
            if (pool.Count > 0)
            {
                return pool.Pop();
            }
            else
            {
                return new T();
            }
        }

        public void Release(T obj)
        {
            if (obj == null) return;
            if (pool.Count > maxNUm)return;

            if (obj is IObject interfac)
            {
                interfac.OnRelease();
            }

            pool.Push(obj);
        }

        public void Dispose()
        {
            pool?.Clear();
            pool = null;
        }
    }
    
    public interface IObject
    {
        void OnRelease();
    }

    public interface PoolBase
    {
        void Dispose();
    }
}
