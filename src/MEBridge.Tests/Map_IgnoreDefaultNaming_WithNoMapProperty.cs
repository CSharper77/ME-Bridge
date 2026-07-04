using MEBridge.Attributes;

namespace MEBridge.Tests;

public class IgnNoMap_Entity
{
    public string First { get; set; } = null!;
    public string Second { get; set; } = null!;
}

[Map<IgnNoMap_Entity>(true)]
public class IgnNoMap_Model
{
    public string First { get; set; } = null!;
    public string Second { get; set; } = null!;
}

public class IgnoreNamingNoMapProperty_Tests
{
    [Fact]
    public async Task Map_IgnoreNamingTrueNoMapProperty_AllDefaultValues()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new IgnNoMap_Entity { First = "a", Second = "b" };

        var result = await mapper.BridgeTo<IgnNoMap_Model>(entity, null);

        Assert.Null(result.First);
        Assert.Null(result.Second);
    }
}
