using System;

namespace JsonStores.Samples.SampleStores.Models
{
    public class Note
    {
        public Guid Id { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? LastEdit { get; set; }
        public string Text { get; set; }
    }
}