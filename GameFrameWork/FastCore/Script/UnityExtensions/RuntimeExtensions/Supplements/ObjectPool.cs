using System;
using System.Collections.Generic;

namespace UnityExtensions
{
    public class ObjectPool<T> where T : class, new()
    {
        Stack<T> _objects;


        public int count { get { return _objects.Count; } }


        public ObjectPool(int preallocateCount = 0)
        {
            _objects = new Stack<T>(preallocateCount > 16 ? preallocateCount : 16);
            AddObjects(preallocateCount);
        }


        public void AddObjects(int quantity)
        {
            while (quantity > 0)
            {
                _objects.Push(new T());

                quantity--;
            }
        }


        public T Spawn()
        {
            if (_objects.Count > 0) return _objects.Pop();
            return new T();
        }


        public void Despawn(T target)
        {
            _objects.Push(target);
        }


        public TempObject GetTemp()
        {
            return new TempObject(this);
        }


        public struct TempObject : IDisposable
        {
            public T item { get; private set; }
            ObjectPool<T> _pool;


            public TempObject(ObjectPool<T> objectPool)
            {
                item = objectPool.Spawn();
                _pool = objectPool;
            }


            void IDisposable.Dispose()
            {
                _pool.Despawn(item);
                item = null;
                _pool = null;
            }
        }

    } // class ObjectPool

} // namespace UnityExtensions