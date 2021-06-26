using System.IO;
using System.Text.Json;
using JsonStores.Tests.Models;

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

        public static void CreateMultiItemsRepository(string path)
        {
            var content = new[] {Constants.GetPerson(), Constants.GetPerson2()};
            SaveFile(path, content);
        }

        private static void SaveFile(string path, object content)
        {
            var jsonContent = JsonSerializer.SerializeToUtf8Bytes(content);
            File.WriteAllBytes(path, jsonContent);
        }
    }
}