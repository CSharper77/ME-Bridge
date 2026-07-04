using MEBridge.Attributes;

namespace MEBridge.Tests;

public class BadDest_Entity
{
    public string Name { get; set; } = null!;
}

[Bridge<BadDest_Entity>]
public class BadDest_Model
{
    [BridgeProperty("NonExistent")]
    public string Value { get; set; } = null!;
}

public class BadDestination_Tests
{
    [Fact]
    public async Task Map_MapPropertyDestinationNotFound_Throws()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new BadDest_Entity { Name = "test" };

        var ex = await Assert.ThrowsAsync<Exception>(() =>
            mapper.BridgeTo<BadDest_Model>(entity, null));

        Assert.Contains("NonExistent", ex.Message);
    }
}
