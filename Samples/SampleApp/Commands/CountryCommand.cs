using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Rendering;
using JsonStores.Exceptions;
using JsonStores.Samples.SampleApp.Models;
using Microsoft.Extensions.DependencyInjection;

namespace JsonStores.Samples.SampleApp.Commands
{
    public static class CountryCommand
    {
        public static void AddCountryCommand(this RootCommand rootCommand)
        {
            var command = new Command("country", "Add, remove or list countries");

            command.ListCountries();
            command.AddCountry();
            command.RemoveCountry();

            rootCommand.Add(command);
        }

        private static void ListCountries(this Command command)
        {
            var c = new Command("list", "List all registered countries");
            c.AddAlias("ls");

            c.Handler = CommandHandler.Create(async (IConsole console) =>
            {
                var store = GetCountriesStore();
                var countries = await store.GetAllAsync();

                var view = new ListView<Country>(countries);
                if (view.IsEmpty) return;

                console.Append(view);
            });

            command.Add(c);
        }

        private static void AddCountry(this Command command)
        {
            var c = new Command("add", "Add a country")
            {
                new Argument<int>("country-code", "Code for the country"),
                new Argument<string>("name", "Name of the country")
            };

            c.Handler = CommandHandler.Create(async (Country country) =>
            {
                var store = GetCountriesStore();
                try
                {
                    await store.AddAsync(country);
                    await store.SaveChangesAsync();
                }
                catch (UniquenessConstraintViolationException e)
                {
                    ConsoleRendering.ClearLineAndWriteError(e.Message);
                }
            });

            command.Add(c);
        }

        private static void RemoveCountry(this Command command)
        {
            var c = new Command("remove", "Removes a country")
            {
                new Argument<int>("country-code", "Code for the country")
            };
            c.AddAlias("rm");

            c.Handler = CommandHandler.Create(async (int countryCode) =>
            {
                var store = GetCountriesStore();
                try
                {
                    await store.RemoveAsync(countryCode);
                    await store.SaveChangesAsync();
                }
                catch (ItemNotFoundException e)
                {
                    ConsoleRendering.ClearLineAndWriteError(e.Message);
                }
            });

            command.Add(c);
        }

        private static IJsonRepository<Country, int> GetCountriesStore()
        {
            // creates a ServiceCollection to simulate the DI container
            var provider = new ServiceCollection()
                .AddJsonRepository<Country, int>(options => options.Location = @"C:\MyFolder")
                .BuildServiceProvider();
            return provider.GetRequiredService<IJsonRepository<Country, int>>();
        }
    }
}