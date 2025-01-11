using System.Collections.Concurrent;

namespace Nazh.Pool
{
    public class CachedObjectPool<T> : ObjectPool<T>
        where T : class, new()
    {
        private ConcurrentQueue<WeakReference<T>> _pool;
        public CachedObjectPool() : this(new DefaultPooledObjectStrategy<T>()) { }
        public CachedObjectPool(PooledObjectStrategy<T> pooledObjectStrategy) : base(pooledObjectStrategy)
        {
            _pool = new ConcurrentQueue<WeakReference<T>>();
        }

        public override T Get()
        {
            while (_pool.Count > 0)
            {
                if (!_pool.TryDequeue(out WeakReference<T> reference) || !reference.TryGetTarget(out T obj))
                {
                    continue;
                }
                return obj;
            }
            return PooledObjectStrategy.Create();
        }

        public override bool Return(T obj)
        {
            if (!PooledObjectStrategy.Reuse(obj))
            {
                return false;
            }
            WeakReference<T> reference = new WeakReference<T>(obj);
            _pool.Enqueue(reference);
            return true;
        }
    }
}
