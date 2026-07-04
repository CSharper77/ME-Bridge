using MEBridge.Attributes;

namespace MEBridge.Tests;

public class ExtraEntityProp_Entity
{
    public string Name { get; set; } = null!;
    public string Hidden { get; set; } = null!;
}

[Bridge<ExtraEntityProp_Entity>]
public class ExtraEntityProp_Model
{
    public string Name { get; set; } = null!;
}

public class ExtraEntityProperty_Tests
{
    [Fact]
    public async Task Map_EntityHasExtraProperty_Ignores()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new ExtraEntityProp_Entity { Name = "visible", Hidden = "invisible" };

        var result = await mapper.BridgeTo<ExtraEntityProp_Model>(entity, null);

        Assert.Equal("visible", result.Name);
    }
}
