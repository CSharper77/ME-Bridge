using MEBridge.Attributes;

namespace MEBridge.Tests;

public class MultiModel_Entity
{
    public string Shared { get; set; } = null!;
}

[Bridge<MultiModel_Entity>]
public class MultiModel_A
{
    public string Shared { get; set; } = null!;
}

[Bridge<MultiModel_Entity>]
public class MultiModel_B
{
    public string Shared { get; set; } = null!;
}

public class MultipleModelsOneEntity_Tests
{
    [Fact]
    public async Task Map_TwoModelsFromSameEntity_BothMap()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new MultiModel_Entity { Shared = "value" };

        var resultA = await mapper.BridgeTo<MultiModel_A>(entity, null);
        var resultB = await mapper.BridgeTo<MultiModel_B>(entity, null);

        Assert.Equal("value", resultA.Shared);
        Assert.Equal("value", resultB.Shared);
    }
}
