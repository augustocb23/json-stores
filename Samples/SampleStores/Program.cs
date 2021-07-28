using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using JsonStores.Samples.SampleStores.Commands;

namespace JsonStores.Samples.SampleStores
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            var command = new RootCommand("Sample Stores");

            command.AddConfigCommand();
            command.AddCountryCommand();
            command.AddNoteCommand();

            return new CommandLineBuilder(command)
                .UseDefaults()
                .AddGlobalOption(new Option<bool>("--verbose", "Show additional messages"))
                .Build().InvokeAsync(args).Result;
        }
    }
}