# MEBridge

A convention-based object-to-object mapper for .NET that bridges between **model** and **entity** (DTO / EF Core) classes using attributes and naming conventions. Supports simple types, nested objects, collections, enums, nullable types, and optional EF Core integration for persistence-aware mapping.

## Getting Started

Reference `MEBridge.csproj` in your project:

```xml
<ItemGroup>
  <ProjectReference Include="..\MEBridge\MEBridge.csproj" />
</ItemGroup>
```

Register with DI:

```csharp
using MEBridge.Extensions;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddMapper();
var provider = services.BuildServiceProvider();
var bridge = provider.GetRequiredService<Bridge>();
```

> `AddMapper()` registers `IReflectify` (from the `CSharper77.Reflectify` package), an optional `IBridgeConfiguration`, and `Bridge` as singletons.

## Defining a Mapping

Place `[Map<TEntity>]` on your model class:

```csharp
using MEBridge.Attributes;

// --- Entity ---
public class UserEntity
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

// --- Model ---
[Map<UserEntity>]
public class UserModel
{
    public int Id { get; set; }

    [MapProperty("FirstName")]
    public string Name { get; set; }

    public string LastName { get; set; }
}
```

Call `BridgeTo<TOutput>` to map in either direction:

```csharp
// Entity → Model
var entity = new UserEntity { Id = 1, FirstName = "Jane", LastName = "Doe" };
var model = await bridge.BridgeTo<UserModel>(entity, context: null);
// model.Id == 1, model.Name == "Jane", model.LastName == "Doe"

// Model → Entity (with EF Core lookup by PK)
var model2 = new UserModel { Id = 1, Name = "Jane", LastName = "Doe" };
var entity2 = await bridge.BridgeTo<UserEntity>(model2, context: dbContext);
```

## Attributes

| Attribute | Target | Description |
|-----------|--------|-------------|
| `[Map<TEntity>]` | Class | Marks the class as a model mapped to `TEntity` |
| `[MapProperty("dest")]` | Property | Maps from the named entity property; also required for nested/collection properties |

Both support `IgnoreDefaultNamingMap` (default `false`) to skip name-convention matching.

## Name Convention Mapping

Properties with identical names are mapped automatically — works for `string`, `int`, `bool`, `decimal`, `DateTime`, `long`, `double`, `float`, `byte`, `short`, `Guid`, enums, and nullable versions of all value types.

Custom class types and collections are **not** mapped by convention — use `[MapProperty]` explicitly.

```csharp
[Map<ConventionEntity>]
public class ConventionModel
{
    public string FirstName { get; set; }  // ✓ auto-mapped (same name)
    public int Age { get; set; }           // ✓ auto-mapped
    public MyColor Color { get; set; }     // ✓ auto-mapped (enum)
    public int? NullableInt { get; set; }  // ✓ auto-mapped (nullable)
}
```

## Nested Objects & Collections

Use `[MapProperty]` to trigger recursive mapping:

```csharp
[Map<AddressEntity>]
public class AddressModel
{
    public string Street { get; set; }
    public string City { get; set; }
}

[Map<PersonEntity>]
public class PersonModel
{
    public string Name { get; set; }

    [MapProperty(nameof(PersonEntity.Address))]
    public AddressModel Address { get; set; }

    [MapProperty(nameof(PersonEntity.Phones))]
    public List<PhoneModel> Phones { get; set; }
}
```

Supported collection types: `List<T>`, `T[]`, `IEnumerable<T>`.

When mapping model→entity with a `DbContext`, the mapper queries the database for existing entities by primary key and updates them in place, including nested collections (matched by PK).

## Configuration

Customize behavior via `IBridgeConfiguration` / `BridgeConfiguration`:

```csharp
services.AddMapper(new BridgeConfiguration
{
    IgnoreDefaultNamingMap = true,
    PrimaryKeyPropertyName = "ID"
});
```

| Member | Default | Description |
|--------|---------|-------------|
| `PrimaryKeyPropertyName` | `"Id"` | Entity PK name for EF lookups |
| `ModelPrimaryKeyPropertyName` | `"Id"` | Model PK name for matching |
| `IgnoreDefaultNamingMap` | `false` | Global disable of name convention |
| `CreateModelInstance<T>()` | `Activator` | Override for custom model instantiation |
| `AfterBridge(input, output, sp)` | no-op | Hook after each mapping |
| `GetOrCreateEntity<T>(sp, model)` | `null` | Custom entity retrieval (null → default EF lookup) |

## Known Limitations

- **Collections of simple types** (e.g. `List<string>`) are not supported — recursive mapping on `string` fails
- **Dictionary**, **HashSet**, **ICollection** are not supported as collection targets
- Nullable-to-non-nullable with a `null` value throws
- Models without parameterless constructors throw
- `DbContext` is only used at the top-level `BridgeTo` call; nested/collection calls pass `null`
