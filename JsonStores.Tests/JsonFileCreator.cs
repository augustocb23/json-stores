using System.IO;
using System.Text.Json;

namespace JsonStores.Tests
{
    public static class JsonFileCreator
    {
        public static void CreateStore(string path)
        {
            var content = Constants.GetPerson();
            SaveFile(path, content);
        }
        
        public static void CreateSingleItemRepository(string path)
        {
            var content = new[] {Constants.GetPerson()};
            SaveFile(path, content);
        }

        private static void SaveFile(string path, object content)
        {
            var jsonContent = JsonSerializer.SerializeToUtf8Bytes(content);
            var fullPath = path;
            File.WriteAllBytes(fullPath, jsonContent);
        }
    }
}