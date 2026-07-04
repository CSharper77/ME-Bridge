using MEBridge.Attributes;

namespace MEBridge.Tests;

public class BothWays_Entity
{
    public string Source { get; set; } = null!;
}

[Bridge<BothWays_Entity>]
public class BothWays_Model
{
    [BridgeProperty("Source")]
    public string Source { get; set; } = null!;
}

public class NameAndAttributeWins_Tests
{
    [Fact]
    public async Task Map_MapPropertyTakesPrecedenceOverName()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new BothWays_Entity { Source = "value" };

        var result = await mapper.BridgeTo<BothWays_Model>(entity, null);

        Assert.Equal("value", result.Source);
    }
}
