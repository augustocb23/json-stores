using System;

namespace JsonStores.Exceptions
{
    public class FileChangedException : InvalidOperationException
    {
        public FileChangedException(string fileName) : base($"The file '{fileName}' was changed since the last reload.")
        {
        }
    }
}