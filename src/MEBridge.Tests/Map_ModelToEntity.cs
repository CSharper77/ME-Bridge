using MEBridge.Attributes;

namespace MEBridge.Tests;

public class Person
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public int Age { get; set; }
    public bool IsActive { get; set; }
    public decimal Salary { get; set; }
    public DateTime CreatedAt { get; set; }
}

[Bridge<Person>]
public class PersonModel
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public int Age { get; set; }
    public bool IsActive { get; set; }
    public decimal Salary { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ModelToEntity_Tests
{
    [Fact]
    public async Task Map_ModelToEntity_AllSimpleProperties()
    {
        var mapper = TestHelpers.CreateMapper();
        using var ctx = EfTestHelpers.CreateInMemoryContext();
        var model = new PersonModel
        {
            Id = 1,
            FirstName = "Jane",
            LastName = "Smith",
            Age = 25,
            IsActive = false,
            Salary = 60000m,
            CreatedAt = new DateTime(2023, 6, 15)
        };

        var result = await mapper.BridgeTo<Person>(model, ctx);

        Assert.Equal(1, result.Id);
        Assert.Equal("Jane", result.FirstName);
        Assert.Equal("Smith", result.LastName);
        Assert.Equal(25, result.Age);
        Assert.False(result.IsActive);
        Assert.Equal(60000m, result.Salary);
        Assert.Equal(new DateTime(2023, 6, 15), result.CreatedAt);
    }

    [Fact]
    public async Task Map_ModelToEntity_ExistingEntityByPk_ReturnsExisting()
    {
        var mapper = TestHelpers.CreateMapper();
        var existing = new Person
        {
            Id = 10,
            FirstName = "Existing",
            LastName = "User",
            Age = 99,
            IsActive = true,
            Salary = 99999m,
            CreatedAt = new DateTime(2000, 1, 1)
        };
        using var ctx = EfTestHelpers.CreateSeedContext(existing);

        var model = new PersonModel
        {
            Id = 10,
            FirstName = "Updated",
            LastName = "User",
            Age = 30,
            IsActive = false,
            Salary = 50000m,
            CreatedAt = new DateTime(2024, 1, 1)
        };

        var result = await mapper.BridgeTo<Person>(model, ctx);

        Assert.Equal(10, result.Id);
        Assert.Equal("Updated", result.FirstName);
        Assert.Equal("User", result.LastName);
        Assert.Equal(30, result.Age);
        Assert.False(result.IsActive);
        Assert.Equal(50000m, result.Salary);
        Assert.Equal(new DateTime(2024, 1, 1), result.CreatedAt);
    }
}
