using System.Collections.Concurrent;

namespace Nazh.Pool
{
    public class DefaultObjectPool<T> : ObjectPool<T>
        where T : class, new()
    {
        private ConcurrentQueue<T> _pool;
        public DefaultObjectPool() : this(new DefaultPooledObjectStrategy<T>()) { }
        public DefaultObjectPool(PooledObjectStrategy<T> pooledObjectStrategy) : base(pooledObjectStrategy)
        {
            _pool = new ConcurrentQueue<T>();
        }

        public override T Get()
        {
            if (_pool.TryDequeue(out T obj))
            {
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
            _pool.Enqueue(obj);
            return true;
        }
    }
}
