﻿using JsonStores.Annotations;

namespace JsonStores.Samples.SampleStores.Models
{
    public class Country
    {
        [JsonRepositoryKey]
        public int CountryCode { get; set; }

        public string Name { get; set; }
    }
}