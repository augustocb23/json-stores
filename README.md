# JsonStores

[![GitHub](https://img.shields.io/github/license/augustocb23/json-stores)](LICENSE) [![Nuget](https://img.shields.io/nuget/v/JsonStores)](https://www.nuget.org/packages/JsonStores) [![Nuget](https://img.shields.io/nuget/dt/JsonStores)](https://www.nuget.org/packages/JsonStores)

Persist your data on JSON files in an easy and flexible way. You can create an simple store, or a repository that encapsulates a collection of items.

Developed on top of [System.Text.Json](https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-overview) and compatible with `netstandard2.1`.

## Getting started

Install the package from [Nuget](https://www.nuget.org/packages/JsonStores)

```text
    dotnet add package JsonStores
```

Just add it to your [DI container](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection) using `AddJsonStores()` extension method

```csharp
    public void ConfigureServices(IServiceCollection services)
    {
        // add your dependencies

        services.AddJsonStores();
    }
```

Inject this with `IJsonStore<T>` or `IJsonRepository<T, TKey>` interfaces

```csharp
    [ApiController]
    [Route("[controller]")]
    public class MyController : ControllerBase
    {
        private readonly IJsonRepository<MyClass, MyClassKey> _repository;
        private readonly IJsonStore<MyConfig> _config;

        public MyController(IJsonRepository<MyClass, MyClassKey> repository, IJsonStore<MyConfig> config)
        {
            _repository = repository;
            _config = config;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var items = await _repository.GetAllAsync();
            return Ok(items);
        }

        [HttpPost("ChangeData")]
        public async Task<ActionResult> Post(Dto dto)
        {
            var item = await _config.ReadOrCreateAsync();

            item.Data1 = dto.Data1;
            item.Data2 = dto.Data2;

            await _config.SaveAsync(item);
            return Ok();
        }
    }
```

## Change options

Use the class `JsonStoreOptions` to customize

```csharp
    public void ConfigureServices(IServiceCollection services)
    {
        var options = new JsonStoreOptions
        {
            // file location - default is the current directory
            Location = @"C:\my files",
            // how generate the file name - default is the name of the generic class
            NamingStrategy = new StaticNamingStrategy("myFile"),
            // file extension - default is 'json'
            FileExtension = "arq"
            // looks file writing time to avoid data loss - default is 'true'
            ThrowOnSavingChangedFile = false;
        };
        
        services.AddJsonRepository<MyClass, MyClassId>(options);
        services.AddJsonStore<Settings>(options => options.NamingStrategy = new StaticNamingStrategy("configs"));

        // you can add generic classes with other options
        services.AddJsonStores(options => options.Location = @"D:\stores", ServiceLifetime.Transient);
    }
```

## Create your JSON repository

In order to instantiate the repository, you must specify a property as an unique key. The second generic parameter must match the type of this property.

- If your class has an Id property, it will be used. You can change this behavior by adding the `IgnoreIdProperty` annotation to your class.
- To use another property, add the `JsonRepositoryKey` annotation on any property of your class.

If your are manually instantiating your class, you can pass an expression in the constructor:

```csharp
    var options = new JsonStoreOptions();
    var repository = new JsonRepository<MyClass, int>(options, myClass => myClass.Key);
```

An `InvalidJsonRepositoryKeyException` will be thrown if:

- Your class don't have an Id or any property with the `JsonRepositoryKey` annotation.
- There is more than one property with the `JsonRepositoryKey` annotation.
- The property type don't match the second generic parameter.

## Personalize your store using inheritance

You can also customize the behavior of your stores just extending `JsonStore` and `JsonRepository` classes.

```csharp
    public class CustomStore : JsonRepository<MyClass, Guid>
    {
        protected override string FileName => "my-items";
    
        public CustomStore(options) : base(options, obj => obj.ObjId)
        {
        }

        // ...
    }
```

See the `NoteStore` class on sample app for details.

## Sample app

You can see the usage of both `JsonStore` and `JsonRepository` with the CLI app on [JsonStores.Sample](https://github.com/augustocb23/json-stores/tree/master/JsonStores.Sample) project.
