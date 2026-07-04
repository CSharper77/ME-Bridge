using MEBridge.Attributes;

namespace MEBridge.Tests;

public enum Map_NullEnum { A, B }

public class NullEnumNull_Entity
{
    public Map_NullEnum? Status { get; set; }
}

[Map<NullEnumNull_Entity>]
public class NullEnumNull_Model
{
    public Map_NullEnum? Status { get; set; }
}

public class NullableEnumWithNull_Tests
{
    [Fact]
    public async Task Map_NullableEnumWithNull_StaysNull()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new NullEnumNull_Entity { Status = null };

        var result = await mapper.BridgeTo<NullEnumNull_Model>(entity, null);

        Assert.Null(result.Status);
    }
}
