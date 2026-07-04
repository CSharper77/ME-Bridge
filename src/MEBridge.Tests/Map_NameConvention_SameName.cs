using MEBridge.Attributes;

namespace MEBridge.Tests;

public class NameConv_Entity
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
}

[Map<NameConv_Entity>]
public class NameConv_Model
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
}

public class NameConvention_SameName_Tests
{
    [Fact]
    public async Task Map_SamePropertyNameMapsByName()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new NameConv_Entity
        {
            FirstName = "Alice",
            LastName = "Johnson"
        };

        var result = await mapper.BridgeTo<NameConv_Model>(entity, null);

        Assert.Equal("Alice", result.FirstName);
        Assert.Equal("Johnson", result.LastName);
    }
}
