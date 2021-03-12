using System;
using System.Diagnostics.CodeAnalysis;

namespace JsonStores.Exceptions
{
    public class ItemNotFoundException : InvalidOperationException
    {
        public ItemNotFoundException([NotNull] object key) : base(
            $"There is no item with the key '{key}'.")
        {
        }
    }
}