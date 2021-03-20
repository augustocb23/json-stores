using System;

namespace JsonStores.Sample.Models
{
    public class Note
    {
        public Guid Id { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? LastEdit { get; set; }
        public string Text { get; set; }
    }
}