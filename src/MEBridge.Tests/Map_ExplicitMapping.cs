using MEBridge.Attributes;

namespace MEBridge.Tests;

public class ExplicitMap_Entity
{
    public string First { get; set; } = null!;
}

[Bridge<ExplicitMap_Entity>]
public class ExplicitMap_Model
{
    [BridgeProperty("First")]
    public string Second { get; set; } = null!;
}

public class ExplicitMapping_Tests
{
    [Fact]
    public async Task Map_MapPropertyRedirectsValue()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new ExplicitMap_Entity { First = "redirected" };

        var result = await mapper.BridgeTo<ExplicitMap_Model>(entity, null);

        Assert.Equal("redirected", result.Second);
    }
}
