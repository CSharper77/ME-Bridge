using MEBridge.Attributes;

namespace MEBridge.Tests;

public class NtoNNNull_Entity
{
    public int? NullableInt { get; set; }
}

[Map<NtoNNNull_Entity>]
public class NtoNNNull_Model
{
    public int NullableInt { get; set; }
}

public class NullableToNonNullableNull_Tests
{
    [Fact]
    public async Task Map_NullableNullToNonNullable_DefaultValue()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new NtoNNNull_Entity { NullableInt = null };

        var result = await mapper.BridgeTo<NtoNNNull_Model>(entity, null);

        Assert.Equal(0, result.NullableInt);
    }
}
