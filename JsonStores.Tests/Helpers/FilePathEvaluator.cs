using System.IO;

namespace JsonStores.Tests.Helpers
{
    public static class FilePathEvaluator
    {
        public static string GetFilePath(string fileName)
        {
            var options = new JsonStoreOptions();
            return Path.Combine(options.Location, $"{fileName}.{options.FileExtension}");
        }
    }
}