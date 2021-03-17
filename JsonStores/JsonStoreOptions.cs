using System.IO;
using JsonStores.NamingStrategies;

namespace JsonStores
{
    public class JsonStoreOptions
    {
        public string Location { get; set; } = Directory.GetCurrentDirectory();
        public INamingStrategy NamingStrategy { get; set; } = new ClassNameNamingStrategy();
        public string FileExtension { get; set; } = "json";
    }
}