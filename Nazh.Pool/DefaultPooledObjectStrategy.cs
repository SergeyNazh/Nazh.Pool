namespace Nazh.Pool
{
    public class DefaultPooledObjectStrategy<T> : PooledObjectStrategy<T>
        where T : class, new()
    {
        public override T Create()
        {
            return new T();
        }

        public override bool Reuse(T obj)
        {
            if (obj is IResusable resusable)
            {
                return resusable.Resuse();
            }
            return true;
        }
    }
}
