using MEBridge.Attributes;

namespace MEBridge.Tests;

public class DefFact_Entity
{
    public string Name { get; set; } = "from-entity";
}

[Map<DefFact_Entity>]
public class DefFact_Model
{
    public string Name { get; set; } = null!;
}

public class DefaultFactory_Tests
{
    [Fact]
    public async Task Map_NoConfig_FallsBackToActivator()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new DefFact_Entity { Name = "works" };

        var result = await mapper.BridgeTo<DefFact_Model>(entity, null);

        Assert.Equal("works", result.Name);
    }
}
