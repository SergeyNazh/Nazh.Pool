namespace Nazh.Pool
{
    public abstract class ObjectPool<T>
        where T : class, new()
    {
        protected int _capacity;
        protected int _size;
        public PooledObjectStrategy<T> PooledObjectStrategy { get; set; }
        public ObjectPool(PooledObjectStrategy<T> pooledObjectStrategy)
        {
            PooledObjectStrategy = pooledObjectStrategy;
        }
        public abstract T Get();
        public abstract bool Return(T obj);
    }
}
