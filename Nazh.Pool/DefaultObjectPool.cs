using System.Collections.Concurrent;

namespace Nazh.Pool
{
    public class DefaultObjectPool<T> : ObjectPool<T>
        where T : class, new()
    {
        private ConcurrentQueue<T> _pool;
        public DefaultObjectPool() : this(new DefaultPooledObjectStrategy<T>()) { }
        public DefaultObjectPool(PooledObjectStrategy<T> pooledObjectStrategy, int capacity = int.MaxValue) : base(pooledObjectStrategy, capacity)
        {
            _pool = new ConcurrentQueue<T>();
        }

        public override T Get()
        {
            if (_pool.TryDequeue(out T obj))
            {
                Interlocked.Decrement(ref _count);
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
            if (Interlocked.Increment(ref _count) <= _capacity)
            {
                _pool.Enqueue(obj);
                return true;
            }
            Interlocked.Decrement(ref _count);
            return false;
        }
    }
}
