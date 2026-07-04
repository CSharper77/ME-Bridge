using MEBridge.Attributes;

namespace MEBridge.Tests;

public class CustFact_Entity
{
    public string Name { get; set; } = null!;
}

[Map<CustFact_Entity>]
public class CustFact_Model
{
    public string Name { get; set; } = null!;
    public bool WasCreatedByFactory { get; set; }
}

public class CustomFactory_Tests
{
    [Fact]
    public async Task Map_CustomCreateModelInstanceFactory_Called()
    {
        var config = new CallbackMappingConfiguration(() => new CustFact_Model
        {
            WasCreatedByFactory = true
        });
        var mapper = TestHelpers.CreateMapper(config);
        var entity = new CustFact_Entity { Name = "test" };

        var result = await mapper.BridgeTo<CustFact_Model>(entity, null);

        Assert.True(result.WasCreatedByFactory);
        Assert.Equal("test", result.Name);
    }
}
