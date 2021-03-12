using System;
using System.Diagnostics.CodeAnalysis;

namespace JsonStores.Exceptions
{
    public class UniquenessConstraintViolationException : InvalidOperationException
    {
        public UniquenessConstraintViolationException([NotNull] object obj, object key) : base(
            $"There is already an '{obj.GetType().Name}' with the key '{key}'.")
        {
        }
    }
}