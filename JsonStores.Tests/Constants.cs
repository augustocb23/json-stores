using System;
using JsonStores.Tests.Models;

namespace JsonStores.Tests
{
    public static class Constants
    {
        public static Person GetPerson() =>
            new() {Id = 1, FullName = "John Smith", BirthDate = DateTime.UnixEpoch};

        public static Person GetPerson2() =>
            new() {Id = 2, FullName = "Jonathan Smith", BirthDate = DateTime.MinValue};
    }
}