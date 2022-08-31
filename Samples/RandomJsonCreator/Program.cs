using System;
using System.IO;
using System.Linq;
using System.Text;
using JsonStores.NamingStrategies;

namespace JsonStores.Samples.RandomJsonCreator
{
    internal static class Program
    {
        private const string FilePath = "out";

        private static int Main(string[] args)
        {
            if (args.Length is < 1 or > 4)
            {
                PrintMessage("Random JSON Creator", ConsoleColor.DarkGray);
                PrintInstructions();
            }

            int filesCount;
            int itemsPerFile;
            try
            {
                filesCount = Convert.ToInt32(args[0]);
                if (args.Length == 1 || !int.TryParse(args[1], out itemsPerFile))
                    itemsPerFile = 200;
            }
            catch (FormatException)
            {
                PrintMessage("Invalid parameters!", ConsoleColor.Red);
                PrintInstructions();
                return 1;
            }

            var verbose = args.Contains("--verbose");
            var deletePreviousFiles = args.Contains("--deletePrevious");
            var disableIndex = args.Contains("--disableIndex");

            var fullFilePath = Path.Join(Environment.CurrentDirectory, FilePath);
            if (deletePreviousFiles && Directory.GetFiles(fullFilePath).Length > 0)
            {
                if (verbose) PrintMessage($"Deleting previous files on {fullFilePath}");
                foreach (var file in Directory.GetFiles(fullFilePath)) File.Delete(file);
            }

            if (verbose) PrintMessage($"Creating {filesCount} files with {itemsPerFile} items...", ConsoleColor.Green);
            for (var fileIndex = 0; fileIndex < filesCount; fileIndex++)
            {
                if (verbose) PrintMessage($"Generating data for file {fileIndex}...");
                var items = new JsonRepository<FileItem, int>(new JsonStoreOptions
                {
                    NamingStrategy = new StaticNamingStrategy(GetRandomString()),
                    Location = fullFilePath,
                    ThrowOnSavingChangedFile = false,
                    UseIndexedKeys = !disableIndex
                });

                for (var itemIndex = 0; itemIndex < itemsPerFile; itemIndex++)
                    items.AddAsync(new FileItem { Id = itemIndex, Data = GetRandomString() }).Wait();

                if (verbose) PrintMessage($"Data generated. Saving file {fileIndex}...");
                items.SaveChangesAsync().Wait();

                if (verbose) PrintMessage($"File {fileIndex} saved.");
            }

            if (verbose) PrintMessage($"{filesCount} files created on {fullFilePath}.", ConsoleColor.Green);
            return 0;
        }

        private static void PrintInstructions()
        {
            var instructionsBuilder = new StringBuilder();
            instructionsBuilder.AppendLine();
            instructionsBuilder.AppendLine("Usage:");
            instructionsBuilder.AppendLine("filesCount: int - Number of files to generate");
            instructionsBuilder.AppendLine("itemsPerFile: int = 200 - Number of items per file");
            instructionsBuilder.AppendLine("--verbose - Show additional messages");
            instructionsBuilder.AppendLine("--deletePrevious - Clear the output folder before run");

            Console.WriteLine(instructionsBuilder.ToString());
        }

        private static string GetRandomString()
        {
            return Guid.NewGuid().ToString("N");
        }

        private static void PrintMessage(string message, ConsoleColor color = default)
        {
            if (color != default) Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}