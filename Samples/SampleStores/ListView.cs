using System;
using System.Collections.Generic;
using System.CommandLine.Rendering.Views;
using System.Linq;
using System.Reflection;

namespace JsonStores.Samples.SampleStores
{
    public sealed class ListView<T> : StackLayoutView where T : class, new()
    {
        public ListView(IEnumerable<T> apps, IEnumerable<string> columns = default)
        {
            if (apps == null) throw new ArgumentNullException(nameof(apps));

            var items = apps.ToList();
            IsEmpty = !items.Any();

            var tableView = new TableView<T> {Items = items};

            var columnsToDisplay = columns ?? GetAllPropertyNames();
            foreach (var column in columnsToDisplay)
                tableView.AddColumn(app => GetValue(app, column), GetProperty(column)?.Name);

            Add(tableView);
        }

        public bool IsEmpty { get; }

        private static IEnumerable<string> GetAllPropertyNames()
        {
            var properties = typeof(T).GetProperties();
            return properties.Select(info => info.Name);
        }

        private static PropertyInfo GetProperty(string name)
        {
            return typeof(T).GetProperty(name,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        }

        private static object GetValue(T obj, string property)
        {
            return GetProperty(property)?.GetValue(obj) ?? "";
        }
    }
}