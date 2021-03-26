using System;
using JsonStores.Tests.Models;

namespace JsonStores.Tests
{
    public static class Constants
    {
        public static Person GetPerson() =>
            new Person {Id = 1, FullName = "John Smith", BirthDate = DateTime.UnixEpoch};
    }
}