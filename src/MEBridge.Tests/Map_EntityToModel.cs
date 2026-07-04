using MEBridge.Attributes;

namespace MEBridge.Tests;

public class EntityToModel_Source
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public int Age { get; set; }
    public bool IsActive { get; set; }
    public decimal Salary { get; set; }
    public DateTime CreatedAt { get; set; }
}

[Map<EntityToModel_Source>]
public class EntityToModel_Target
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public int Age { get; set; }
    public bool IsActive { get; set; }
    public decimal Salary { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class EntityToModel_Tests
{
    [Fact]
    public async Task Map_EntityToModel_AllSimpleProperties()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new EntityToModel_Source
        {
            FirstName = "John",
            LastName = "Doe",
            Age = 30,
            IsActive = true,
            Salary = 50000m,
            CreatedAt = new DateTime(2024, 1, 1)
        };

        var result = await mapper.BridgeTo<EntityToModel_Target>(entity, null);

        Assert.Equal("John", result.FirstName);
        Assert.Equal("Doe", result.LastName);
        Assert.Equal(30, result.Age);
        Assert.True(result.IsActive);
        Assert.Equal(50000m, result.Salary);
        Assert.Equal(new DateTime(2024, 1, 1), result.CreatedAt);
    }
}
