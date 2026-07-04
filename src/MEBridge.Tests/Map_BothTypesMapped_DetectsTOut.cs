using MEBridge.Attributes;

namespace MEBridge.Tests;

[Bridge<BothMapped_Entity>]
public class BothMapped_Model
{
    public string Value { get; set; } = null!;
}

public class BothMapped_Entity
{
    public string Value { get; set; } = null!;
}

public class ModelHasMapAttribute_Tests
{
    [Fact]
    public async Task Map_ModelMapAttribute_MapsByName()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new BothMapped_Entity { Value = "from-entity" };

        var result = await mapper.BridgeTo<BothMapped_Model>(entity, null);

        Assert.Equal("from-entity", result.Value);
    }
}
