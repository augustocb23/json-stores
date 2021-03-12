namespace JsonStores.NamingStrategies
{
    public class ClassNameNamingStrategy : INamingStrategy
    {
        public string GetName<T>() => typeof(T).Name;
    }
}