using MEBridge.Attributes;

namespace MEBridge.Tests;

public class ExtraModelProp_Entity
{
    public string Name { get; set; } = null!;
}

[Map<ExtraModelProp_Entity>]
public class ExtraModelProp_Model
{
    public string Name { get; set; } = null!;
    public string ExtraField { get; set; } = null!;
}

public class ExtraModelProperty_Tests
{
    [Fact]
    public async Task Map_ModelHasExtraProperty_DefaultsToDefault()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new ExtraModelProp_Entity { Name = "test" };

        var result = await mapper.BridgeTo<ExtraModelProp_Model>(entity, null);

        Assert.Equal("test", result.Name);
        Assert.Null(result.ExtraField);
    }
}
