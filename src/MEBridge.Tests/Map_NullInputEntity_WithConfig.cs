using MEBridge.Attributes;

namespace MEBridge.Tests;

public class NullWithCfg_Entity
{
    public string Name { get; set; } = null!;
}

[Map<NullWithCfg_Entity>]
public class NullWithCfg_Model
{
    public string Name { get; set; } = null!;
}

public class NullInputWithConfig_Tests
{
    [Fact]
    public async Task Map_NullInputWithCustomConfig_Throws()
    {
        var config = new CallbackMappingConfiguration(() => new NullWithCfg_Model());
        var mapper = TestHelpers.CreateMapper(config);

        var ex = await Record.ExceptionAsync(() =>
            mapper.BridgeTo<NullWithCfg_Model>(null!, null));

        Assert.NotNull(ex);
    }
}
