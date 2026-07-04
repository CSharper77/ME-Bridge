using MEBridge.Attributes;

namespace MEBridge.Tests;

public class CfgIgn_Entity
{
    public string First { get; set; } = null!;
}

[Bridge<CfgIgn_Entity>]
public class CfgIgn_Model
{
    public string First { get; set; } = null!;
    public string Second { get; set; } = null!;
}

public class ConfigLevelIgnoreNaming_Tests
{
    [Fact]
    public async Task Map_ConfigLevelIgnoreNamingFalse_NameConventionWorks()
    {
        var config = new CallbackMappingConfiguration(() => new CfgIgn_Model())
        {
            IgnoreDefaultNamingMap = false
        };
        var mapper = TestHelpers.CreateMapper(config);
        var entity = new CfgIgn_Entity { First = "value" };

        var result = await mapper.BridgeTo<CfgIgn_Model>(entity, null);

        Assert.Equal("value", result.First);
        Assert.Null(result.Second);
    }

    [Fact]
    public async Task Map_ConfigLevelIgnoreNamingTrue_NameConventionNotlUsed()
    {
        var config = new CallbackMappingConfiguration(() => new CfgIgn_Model())
        {
            IgnoreDefaultNamingMap = true
        };
        var mapper = TestHelpers.CreateMapper(config);
        var entity = new CfgIgn_Entity { First = "value" };

        var result = await mapper.BridgeTo<CfgIgn_Model>(entity, null);

        Assert.Null(result.First);
    }
}
