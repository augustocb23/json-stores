using System.Reflection;
using JsonStores.NamingStrategies;

namespace JsonStores
{
    public class JsonStoreOptions
    {
        public string Location { get; set; } = Assembly.GetExecutingAssembly().Location;
        public INamingStrategy NamingStrategy { get; set; } = new ClassNameNamingStrategy();
        public string FileExtension { get; set; } = "json";
    }
}