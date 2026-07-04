using MEBridge.Attributes;

namespace MEBridge.Tests;

public enum Map_ExplicitEnum { Zero, One, Two }

public class IntEnum_Entity
{
    public int StatusCode { get; set; }
}

[Bridge<IntEnum_Entity>]
public class IntEnum_Model
{
    [BridgeProperty("StatusCode")]
    public Map_ExplicitEnum StatusCode { get; set; }
}

public class EnumExplicitInt_Tests
{
    [Fact]
    public async Task Map_IntSourceToEnumTargetExplicit()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new IntEnum_Entity { StatusCode = 1 };

        var result = await mapper.BridgeTo<IntEnum_Model>(entity, null);

        Assert.Equal(Map_ExplicitEnum.One, result.StatusCode);
    }
}
