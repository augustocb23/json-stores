using System;

namespace JsonStores.Tests.Models
{
    public record Person
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
    }
}