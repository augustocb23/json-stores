using JsonStores.Annotations;

namespace JsonStores.Sample.Models
{
    public class Country
    {
        [JsonRepositoryId] public int CountryCode { get; set; }
        public string Name { get; set; }
    }
}