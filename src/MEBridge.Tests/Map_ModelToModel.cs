using MEBridge.Attributes;

namespace MEBridge.Tests;

[Map<ModelToModel_B>]
public class ModelToModel_A
{
    public string Name { get; set; } = null!;
    public int Age { get; set; }
    public string Email { get; set; } = null!;
}

[Map<ModelToModel_A>]
public class ModelToModel_B
{
    public string Name { get; set; } = null!;
    public int Age { get; set; }
    public string Email { get; set; } = null!;
}

public class ModelToModel_Tests
{
    [Fact]
    public async Task Map_ModelAToModelB_BothHaveMapAttribute()
    {
        var mapper = TestHelpers.CreateMapper();
        var modelA = new ModelToModel_A
        {
            Name = "Alice",
            Age = 30,
            Email = "alice@example.com"
        };

        var result = await mapper.BridgeTo<ModelToModel_B>(modelA, null);

        Assert.Equal("Alice", result.Name);
        Assert.Equal(30, result.Age);
        Assert.Equal("alice@example.com", result.Email);
    }

    [Fact]
    public async Task Map_ModelBToModelA_ReverseDirection()
    {
        var mapper = TestHelpers.CreateMapper();
        var modelB = new ModelToModel_B
        {
            Name = "Bob",
            Age = 25,
            Email = "bob@example.com"
        };

        var result = await mapper.BridgeTo<ModelToModel_A>(modelB, null);

        Assert.Equal("Bob", result.Name);
        Assert.Equal(25, result.Age);
        Assert.Equal("bob@example.com", result.Email);
    }

    [Fact]
    public async Task Map_ModelToModel_RoundTrip()
    {
        var mapper = TestHelpers.CreateMapper();
        var modelA = new ModelToModel_A { Name = "Charlie", Age = 35, Email = "c@example.com" };

        var modelB = await mapper.BridgeTo<ModelToModel_B>(modelA, null);
        var result = await mapper.BridgeTo<ModelToModel_A>(modelB, null);

        Assert.Equal(modelA.Name, result.Name);
        Assert.Equal(modelA.Age, result.Age);
        Assert.Equal(modelA.Email, result.Email);
    }
}
