using System;

namespace JsonStores.Exceptions
{
    public class InvalidJsonRepositoryKeyException : InvalidOperationException
    {
        public InvalidJsonRepositoryKeyException(string message) : base(message)
        {
        }
    }
}