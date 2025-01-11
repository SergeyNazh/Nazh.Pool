namespace Nazh.Pool
{
    public abstract class ObjectPool<T>
        where T : class, new()
    {
        protected int _capacity;
        protected int _count;
        public int Capacity => _capacity;
        public int Count => _count;
        public PooledObjectStrategy<T> PooledObjectStrategy { get; set; }
        public ObjectPool(PooledObjectStrategy<T> pooledObjectStrategy, int capacity)
        {
            PooledObjectStrategy = pooledObjectStrategy;
            _capacity = capacity;
        }
        public abstract T Get();
        public abstract bool Return(T obj);
        public abstract void Resize(int capacity);
    }
}
