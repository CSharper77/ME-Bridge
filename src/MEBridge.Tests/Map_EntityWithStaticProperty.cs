using MEBridge.Attributes;

namespace MEBridge.Tests;

public class StaticProp_Entity
{
    public string Name { get; set; } = null!;
    public static string StaticValue { get; set; } = "static-entity";
}

[Bridge<StaticProp_Entity>]
public class StaticProp_Model
{
    public string Name { get; set; } = null!;
    public static string StaticValue { get; set; } = "static-model";
}

public class StaticProperty_Tests
{
    [Fact]
    public async Task Map_StaticPropertiesAreCopiedByMapper()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new StaticProp_Entity { Name = "instance" };

        var result = await mapper.BridgeTo<StaticProp_Model>(entity, null);

        Assert.Equal("instance", result.Name);
        Assert.Equal("static-entity", StaticProp_Model.StaticValue);
    }
}
