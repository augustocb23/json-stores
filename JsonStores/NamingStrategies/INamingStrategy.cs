namespace JsonStores.NamingStrategies
{
    public interface INamingStrategy
    {
        string GetName<T>();
    }
}