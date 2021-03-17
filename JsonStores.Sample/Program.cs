using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using JsonStores.Sample.Commands;

namespace JsonStores.Sample
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            var command = new RootCommand("Json Stores Sample App");
            command.AddConfigCommand();

            return new CommandLineBuilder(command)
                .UseDefaults()
                .AddGlobalOption(new Option<bool>("--verbose", "Show additional messages"))
                .Build().InvokeAsync(args).Result;
        }
    }
}