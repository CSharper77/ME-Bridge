using MEBridge.Attributes;

namespace MEBridge.Tests;

public enum Map_NullableEnumStatus { Active, Inactive }

public class NullEnumVal_Entity
{
    public Map_NullableEnumStatus? Status { get; set; }
}

[Bridge<NullEnumVal_Entity>]
public class NullEnumVal_Model
{
    public Map_NullableEnumStatus? Status { get; set; }
}

public class NullableEnumWithValue_Tests
{
    [Fact]
    public async Task Map_NullableEnumWithValue()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new NullEnumVal_Entity { Status = Map_NullableEnumStatus.Active };

        var result = await mapper.BridgeTo<NullEnumVal_Model>(entity, null);

        Assert.Equal(Map_NullableEnumStatus.Active, result.Status);
    }
}
