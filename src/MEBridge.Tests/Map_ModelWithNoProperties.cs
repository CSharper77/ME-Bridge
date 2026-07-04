using MEBridge.Attributes;

namespace MEBridge.Tests;

public class EmptyModel_Entity
{
    public string Name { get; set; } = null!;
}

[Bridge<EmptyModel_Entity>]
public class EmptyModel_Model { }

public class ModelWithNoProperties_Tests
{
    [Fact]
    public async Task Map_ModelWithNoProperties_ReturnsInstance()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new EmptyModel_Entity { Name = "test" };

        var result = await mapper.BridgeTo<EmptyModel_Model>(entity, null);

        Assert.NotNull(result);
    }
}
