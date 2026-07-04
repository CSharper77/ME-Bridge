using MEBridge.Attributes;

namespace MEBridge.Tests;

public class IgnNameTrue_Entity
{
    public string First { get; set; } = null!;
}

[Bridge<IgnNameTrue_Entity>(true)]
public class IgnNameTrue_Model
{
    public string NotFirst { get; set; } = null!;

    [BridgeProperty("First")]
    public string First { get; set; } = null!;
}

public class ClassLevelIgnoreNamingTrue_Tests
{
    [Fact]
    public async Task Map_ClassLevelIgnoreNamingTrue_OnlyMappedProperties()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new IgnNameTrue_Entity { First = "value" };

        var result = await mapper.BridgeTo<IgnNameTrue_Model>(entity, null);

        Assert.Equal("value", result.First);
        Assert.Null(result.NotFirst);
    }
}
