using JsonStores.Annotations;

namespace JsonStores.Tests.Repositories
{
    public static class RepositoryTestModels
    {
        public record PersonWithoutId
        {
            public int Number { get; set; }
            public string Name { get; set; }
        }

        [IgnoreIdProperty]
        public record PersonIdWithAttribute
        {
            [JsonRepositoryKey] public int Number { get; set; }
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}