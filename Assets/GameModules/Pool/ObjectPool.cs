using System;
using System.Collections;
using System.Collections.Generic;

namespace GameModules
{
    public class ObjectPool<T> : PoolBase where T : new()
    {
        private int _maxNum;
        private Stack<T> _pool;

        public ObjectPool(int maxNum = 10)
        {
            this._maxNum = maxNum;
            _pool = new Stack<T>();
        }

        public T Get()
        {
            if (_pool.Count > 0)
            {
                return _pool.Pop();
            }
            else
            {
                return new T();
            }
        }

        public void Release(T obj)
        {
            if (obj == null) return;
            if (_pool.Count >= _maxNum)return;
            if (_pool.Contains(obj)) return;

            if (obj is IObject interfac)
            {
                interfac.OnRelease();
            }
            

            _pool.Push(obj);
        }

        public void Dispose()
        {
            _pool?.Clear();
            _pool = null;
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
