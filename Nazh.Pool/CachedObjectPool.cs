using System.Collections.Concurrent;

namespace Nazh.Pool
{
    public class CachedObjectPool<T> : ObjectPool<T>
        where T : class, new()
    {
        private ConcurrentQueue<WeakReference<T>> _pool;
        public CachedObjectPool() : this(new DefaultPooledObjectStrategy<T>()) { }
        public CachedObjectPool(PooledObjectStrategy<T> pooledObjectStrategy, int capacity = int.MaxValue) : base(pooledObjectStrategy, capacity)
        {
            _pool = new ConcurrentQueue<WeakReference<T>>();
        }

        public override T Get()
        {
            while (_pool.TryDequeue(out WeakReference<T> reference))
            {
                Interlocked.Decrement(ref _count);
                if (!reference.TryGetTarget(out T obj))
                {
                    continue;
                }
                return obj;
            }
            return PooledObjectStrategy.Create();
        }

        public override void Resize(int capacity)
        {
            Interlocked.Exchange(ref _capacity, capacity);
            while (_count > _capacity)
            {
                _pool.TryDequeue(out _);
                Interlocked.Decrement(ref _count);
            }
        }

        public override bool Return(T obj)
        {
            if (!PooledObjectStrategy.Reuse(obj))
            {
                return false;
            }
            WeakReference<T> reference = new WeakReference<T>(obj);
            if (Interlocked.Increment(ref _count) <= _capacity)
            {
                _pool.Enqueue(reference);
                return true;
            }
            Interlocked.Decrement(ref _count);
            return false;
        }
    }
}
