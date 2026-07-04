# MEBridge — Complete Usage Reference

## Table of Contents

1. [Getting Started](#1-getting-started)
2. [Defining Mappings](#2-defining-mappings)
3. [Name Convention Mapping](#3-name-convention-mapping)
4. [Explicit Mapping with \[MapProperty\]](#4-explicit-mapping-with-mapproperty)
5. [Nested Complex Objects](#5-nested-complex-objects)
6. [Collection Mapping](#6-collection-mapping)
7. [Enum Mapping](#7-enum-mapping)
8. [Nullable Types](#8-nullable-types)
9. [Type Resolution](#9-type-resolution)
10. [Error Scenarios](#10-error-scenarios)
11. [DI Registration](#11-di-registration)
12. [Known Limitations](#12-known-limitations)
13. [API Reference](#13-api-reference)

---

## 1. Getting Started

### Install Dependencies

```bash
dotnet add package Microsoft.EntityFrameworkCore
```

Reference the `MEBridge` and `HyperReflect` projects in your `.csproj`:

```xml
<ItemGroup>
  <ProjectReference Include="..\MEBridge\MEBridge.csproj" />
</ItemGroup>
```

### Register with DI

```csharp
using MEBridge.Extensions;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddMapper();
var provider = services.BuildServiceProvider();

var mapper = provider.GetRequiredService<Mapper>();
```

The `AddMapper()` extension registers:
- `IReflectionHelper` → `ReflectionHelper` (singleton)
- `Mapper` (singleton)

### Basic Mapping Call

```csharp
var entity = new MyEntity { ... };
var model = await mapper.Map<MyEntity, MyModel>(entity, dbContext: null);
```

The `DbContext` parameter is accepted but **not currently used** — pass `null`.

---

## 2. Defining Mappings

### `[Map<TEntity>]` (Class-Level)

Place on the **model class** to indicate which entity type it maps from.

```csharp
using MEBridge.Attributes;

[Map<UserEntity>]
public class UserModel
{
    // properties ...
}
```

The generic version `[Map<T>]` derives from `MapAttribute` and automatically sets `TargetType = typeof(T)`.

### `[MapProperty("destination")]` (Property-Level)

Place on model properties to map from a differently-named entity property.

```csharp
[Map<UserEntity>]
public class UserModel
{
    [MapProperty("FirstName")]
    public string Name { get; set; }
}
```

**Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `destination` | `string` | Name of the source property in the entity class |

**Notes:**
- `AllowMultiple = false` — only one per property
- `Inherited = true` — inherited by derived classes

---

## 3. Name Convention Mapping

Properties with **identical names** in both entity and model are automatically mapped.

### Rules

A property is mapped by convention only when **all** conditions are met:

1. An entity property with the **same name** exists
2. The model property does **not** have a `[MapProperty]` attribute
3. The property type is **not** a collection (`IEnumerable` excluding `string`)
4. The property type is **not** a custom class (reference type excluding `string`)

### Supported Types

```csharp
public class ConventionEntity
{
    public string FirstName { get; set; }    // ✓
    public int Age { get; set; }             // ✓
    public bool IsActive { get; set; }       // ✓
    public decimal Salary { get; set; }      // ✓
    public DateTime CreatedAt { get; set; }  // ✓
    public MyColor Color { get; set; }       // ✓ enum
    public int? NullableInt { get; set; }    // ✓ nullable
}

[Map<ConventionEntity>]
public class ConventionModel
{
    public string FirstName { get; set; }
    public int Age { get; set; }
    public bool IsActive { get; set; }
    public decimal Salary { get; set; }
    public DateTime CreatedAt { get; set; }
    public MyColor Color { get; set; }
    public int? NullableInt { get; set; }
}
```

### What is NOT mapped by convention

```csharp
// Entity has matching property name, but type is a custom class → skipped
public SubEntity SubEntity { get; set; }       // custom class → NOT mapped
public List<SubEntity> SubEntities { get; set; } // collection → NOT mapped
```

For these, you must use `[MapProperty]`.

---

## 4. Explicit Mapping with `[MapProperty]`

Use when the entity property has a **different name** or the property is a **complex type / collection**.

### Simple Types

```csharp
public class ExplicitEntity
{
    public string First { get; set; }
    public int Value { get; set; }
}

[Map<ExplicitEntity>]
public class ExplicitModel
{
    [MapProperty(nameof(ExplicitEntity.First))]
    public string FirstName { get; set; }

    [MapProperty(nameof(ExplicitEntity.Value))]
    public int MyValue { get; set; }
}
```

### Int to Enum Mapping

```csharp
public class EnumEntity
{
    public int ColorCode { get; set; }
}

public enum MyColor { Red = 1, Green = 2, Blue = 3 }

[Map<EnumEntity>]
public class EnumModel
{
    [MapProperty(nameof(EnumEntity.ColorCode))]
    public MyColor Color { get; set; }
}
```

The CLR handles int-to-enum conversion automatically via `PropertyInfo.SetValue`.

---

## 5. Nested Complex Objects

Complex properties (custom classes) require `[MapProperty]` to trigger recursive mapping.

### Single Nested Object

```csharp
// --- Entity classes ---
public class AddressEntity
{
    public string Street { get; set; }
    public string City { get; set; }
}

public class PersonEntity
{
    public string Name { get; set; }
    public AddressEntity Address { get; set; }
}

// --- Model classes ---
[Map<AddressEntity>]
public class AddressModel
{
    public string Street { get; set; }
    [MapProperty(nameof(AddressEntity.City))]
    public string CityName { get; set; }
}

[Map<PersonEntity>]
public class PersonModel
{
    public string Name { get; set; }

    [MapProperty(nameof(PersonEntity.Address))]
    public AddressModel Address { get; set; }
}
```

When mapping `PersonEntity → PersonModel`, the mapper:
1. Maps `Name` by convention
2. Detects `Address` has `[MapProperty]` and the type is a custom class
3. Recursively calls `Map<AddressEntity, AddressModel>(entity.Address, null)`

### Null Entity Values

If the entity's complex property is `null`, the mapping is **skipped** and the model property stays at its default (`null`).

```csharp
var entity = new PersonEntity { Name = "John", Address = null };
var model = await mapper.Map<PersonEntity, PersonModel>(entity, null);
// model.Address == null
```

---

## 6. Collection Mapping

Collections require `[MapProperty]` on the model property. The mapper iterates each item and maps it recursively.

### Supported Target Collection Types

| Model Property Type | Mapped? | Notes |
|-------------------|---------|-------|
| `List<T>` | ✓ | Creates `List<T>` via `Enumerable.ToList()` |
| `T[]` | ✗ | **Not supported** — throws `IndexOutOfRangeException` (uses `GenericTypeArguments[0]` which is empty for arrays) |
| `IEnumerable<T>` | ✓ | Assigns the casted `IEnumerable<T>` directly |

### Example: List to List

```csharp
public class ItemEntity
{
    public int Id { get; set; }
    public string Title { get; set; }
}

[Map<ItemEntity>]
public class ItemModel
{
    public int Id { get; set; }
    [MapProperty(nameof(ItemEntity.Title))]
    public string Name { get; set; }
}

public class OrderEntity
{
    public List<ItemEntity> Items { get; set; }
}

[Map<OrderEntity>]
public class OrderModel
{
    [MapProperty(nameof(OrderEntity.Items))]
    public List<ItemModel> Items { get; set; }
}
```

### Example: IEnumerable Source to List Target

```csharp
public class OrderEntity
{
    public IEnumerable<ItemEntity> Items { get; set; }
}

[Map<OrderEntity>]
public class OrderModel
{
    [MapProperty(nameof(OrderEntity.Items))]
    public List<ItemModel> Items { get; set; }
}
```

### Collection Edge Cases

| Scenario | Behavior |
|----------|----------|
| Empty source collection | Model property gets empty collection |
| Null source collection | Model property stays `null` (skipped) |
| Null items in collection | Null items are skipped, non-null items are mapped |
| Collection of strings (`List<string>`) | **Throws** — cannot `Map<string, string>()` because `string` has no `[Map]` attribute |

---

## 7. Enum Mapping

### By Name Convention

```csharp
public enum Status { Active, Inactive }

public class StatusEntity
{
    public Status CurrentStatus { get; set; }
}

[Map<StatusEntity>]
public class StatusModel
{
    public Status CurrentStatus { get; set; } // auto-mapped
}
```

### Nullable Enum

```csharp
public class StatusEntity
{
    public Status? CurrentStatus { get; set; }
}

[Map<StatusEntity>]
public class StatusModel
{
    public Status? CurrentStatus { get; set; } // supports null
}
```

### Non-Nullable Model from Nullable Entity Source

```csharp
public class StatusEntity
{
    public Status? CurrentStatus { get; set; }
}

[Map<StatusEntity>]
public class StatusModel
{
    public Status CurrentStatus { get; set; } // ok when entity has value
}
```

---

## 8. Nullable Types

Nullable value types (`int?`, `bool?`, `DateTime?`, `Guid?`, `decimal?`, `MyEnum?`) are treated as simple types and mapped by name convention.

### Nullable ↔ Non-Nullable

| Entity Type | Model Type | Entity Value | Result |
|-------------|-----------|-------------|--------|
| `int` | `int?` | `42` | `42` |
| `int?` | `int?` | `null` | `null` |
| `int?` | `int?` | `42` | `42` |
| `int?` | `int` | `42` | `42` (value present) |
| `int?` | `int` | `null` | `0` (default via CLR) |

```csharp
public class NullableEntity
{
    public int NonNullableInt { get; set; }
    public int? NullableInt { get; set; }
    public bool? NullableBool { get; set; }
    public DateTime? NullableDateTime { get; set; }
    public Guid? NullableGuid { get; set; }
    public decimal? NullableDecimal { get; set; }
    public MyColor? NullableColor { get; set; }
}

[Map<NullableEntity>]
public class NullableModel
{
    public int? NonNullableInt { get; set; }
    public int? NullableInt { get; set; }
    public bool? NullableBool { get; set; }
    public DateTime? NullableDateTime { get; set; }
    public Guid? NullableGuid { get; set; }
    public decimal? NullableDecimal { get; set; }
    public MyColor? NullableColor { get; set; }
}
```

---

## 9. Type Resolution

`Map<TIn, TOut>()` determines which type is the **model** (has `[Map]`) and which is the **entity** (no `[Map]`).

### Resolution Logic

1. If `TIn` has `[Map]` → model = `TIn`, entity = `TOut`
2. If `TOut` has `[Map]` → model = `TOut`, entity = `TIn`
3. If both have `[Map]` → `TOut` wins (overwrites)
4. If neither has `[Map]` → throws `Exception`

### Recommended Usage

```csharp
// Entity as TIn, Model as TOut → correct direction
var result = await mapper.Map<MyEntity, MyModel>(entity, null);

// Model as TIn, Entity as TOut → reverse direction (also works)
var result = await mapper.Map<MyModel, MyEntity>(model, null);
```

---

## 10. Error Scenarios

| Scenario | Exception | Message |
|----------|-----------|---------|
| Neither type has `[Map]` | `Exception` | `"The types X/Y are not mapped, no class declare [MapAttribute]"` |
| `[MapProperty]` destination not found in entity | `Exception` | `"Mapper Error, Failed to find mapped property X in type Y"` |
| Both `[Map]` attributes present | ✅ (TOut wins) | — |
| Array model property for collection mapping | `IndexOutOfRangeException` | — |
| `List<string>` collection | `Exception` | `"The types System.String/System.String are not mapped..."` (from recursive `Map`) |

---

## 11. DI Registration

### Extension Method

```csharp
using MEBridge.Extensions;

services.AddMapper();
```

This registers:
- `IReflectionHelper` → `ReflectionHelper` (singleton)
- `Mapper` (singleton)

### Manual Instantiation

```csharp
using HyperReflect;

var mapper = new Mapper(new ReflectionHelper());
```

---

## 12. Known Limitations

1. **Collection of simple types** — `List<string>`, `List<int>`, etc. are not supported (recursive `Map` on `string` fails)
2. **Array target properties** — Model properties typed as `T[]` for collection mapping throw `IndexOutOfRangeException`
3. **`DbContext` parameter** — Accepted but unused, currently always passes `null` in recursive calls
4. **`MappingConfiguration`** — Defined but not wired into the `Mapper` constructor or used during mapping
5. **`IgnoreDefaultNamingMap`** — Defined on `[Map]` attribute but not checked by the mapper
6. **Order of operations** — Name convention runs first, then `[MapProperty]` overrides; this means both strategies execute for each property

---

## 13. API Reference

### `Mapper`

| Member | Description |
|--------|-------------|
| `Map<TIn, TOut>(TIn, DbContext)` | Main mapping method. Maps entity to model. |
| `ReflectionHelper` | Gets the `IReflectionHelper` instance |
| `CreateModelInstance<TOut>()` | Protected virtual factory method (override for custom instantiation) |

### Attributes

| Attribute | Target | Description |
|-----------|--------|-------------|
| `[Map<TEntity>]` | Class | Marks the class as a model mapped from `TEntity` |
| `[MapProperty("dest")]` | Property | Maps from the named entity property |

### `IReflectionHelper`

| Method | Description |
|--------|-------------|
| `GetProperties(Type)` | Gets all properties with their attributes |
| `GetProperty(Type, string)` | Gets a specific property by name |
| `GetTypeInfo(Type)` | Gets extended type info with attributes |
| `IsCustomClassType(Type)` | Returns true for `class` types except `string` |
| `IsCollectionType(Type)` | Returns true for `IEnumerable` types except `string` |
| `GetUnderlyingNonNullableType(Type)` | Unwraps `Nullable<T>` to `T` |
