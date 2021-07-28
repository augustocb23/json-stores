using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Reflection;
using JsonStores.Samples.SampleStores.Models;
using Microsoft.Extensions.DependencyInjection;

namespace JsonStores.Samples.SampleStores.Commands
{
    public static class ConfigCommand
    {
        public static void AddConfigCommand(this RootCommand rootCommand)
        {
            var command = new Command("config", "Gets or sets system settings");

            command.AddGetter();
            command.AddSetter();

            rootCommand.Add(command);
        }

        private static void AddGetter(this Command command)
        {
            var c = new Command("get", "Gets a system setting")
            {
                new Argument<string>("property")
            };
            c.Handler = CommandHandler.Create(async (bool verbose, string property) =>
            {
                var store = GetSettingsStore();
                var propertyInfo = typeof(Settings).GetProperty(property,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo is null)
                {
                    var properties = typeof(Settings).GetProperties().Select(info => info.Name);
                    ConsoleRendering.ClearLineAndWriteError(
                        $"Invalid property. Available properties: {string.Join(",", properties)}");
                    return 5;
                }

                if (verbose) ConsoleRendering.ClearLineAndWrite("Loading settings...");
                var settings = await store.ReadOrCreateAsync();
                ConsoleRendering.ClearLineAndWrite($"{propertyInfo.GetValue(settings)}\n");

                return 0;
            });

            command.Add(c);
        }

        private static void AddSetter(this Command command)
        {
            var c = new Command("set", "Sets a system configuration")
            {
                new Argument<string>("property"), new Argument<string>("value")
            };
            c.Handler = CommandHandler.Create(async (bool verbose, string property, string value) =>
            {
                var store = GetSettingsStore();
                var propertyInfo = typeof(Settings).GetProperty(property,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo is null)
                {
                    var properties = typeof(Settings).GetProperties().Select(info => info.Name);
                    ConsoleRendering.ClearLineAndWriteError(
                        $"Invalid property. Available properties: {string.Join(",", properties)}");
                    return 5;
                }

                object castedValue = propertyInfo.Name switch
                {
                    nameof(Settings.Name) => value,
                    nameof(Settings.Email) => value,
                    nameof(Settings.Age) => int.Parse(value),
                    _ => throw new ArgumentOutOfRangeException(nameof(property), property,
                        $"Can't change property '{property}'")
                };

                if (verbose) ConsoleRendering.ClearLineAndWrite("Loading settings...");
                var settings = await store.ReadOrCreateAsync();

                if (verbose) ConsoleRendering.ClearLineAndWrite("Saving...");
                propertyInfo.SetValue(settings, castedValue);
                await store.SaveAsync(settings);

                if (verbose) ConsoleRendering.ClearLine();
                return 0;
            });

            command.Add(c);
        }

        private static IJsonStore<Settings> GetSettingsStore()
        {
            // creates a ServiceCollection to simulate the DI container
            var provider = new ServiceCollection().AddJsonStores().BuildServiceProvider();
            return provider.GetRequiredService<IJsonStore<Settings>>();
        }
    }
}