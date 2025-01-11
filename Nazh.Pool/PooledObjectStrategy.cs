namespace Nazh.Pool
{
    public abstract class PooledObjectStrategy<T>
        where T : class, new()
    {
        public abstract T Create();
        public abstract bool Reuse(T obj);
    }
}
