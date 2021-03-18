using System;

namespace JsonStores.Exceptions
{
    public class InvalidJsonRepositoryIdException : InvalidOperationException
    {
        public InvalidJsonRepositoryIdException(string message) : base(message)
        {
        }
    }
}