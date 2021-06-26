using System.IO;
using System.Text.Json;
using JsonStores.Tests.Models;

namespace JsonStores.Tests.Helpers
{
    public static class JsonFileCreator
    {
        public static void CreateStore(string fileName)
        {
            var content = Constants.GetPerson();
            SaveFile(fileName, content);
        }
        
        public static void CreateSingleItemRepository(string fileName)
        {
            var content = new[] {Constants.GetPerson()};
            SaveFile(fileName, content);
        }

        private static void SaveFile(string fileName, object content)
        {
            var jsonContent = JsonSerializer.SerializeToUtf8Bytes(content);
            var fullPath = FilePathEvaluator.GetFilePath(fileName);
            File.WriteAllBytes(fullPath, jsonContent);
        }
    }
}