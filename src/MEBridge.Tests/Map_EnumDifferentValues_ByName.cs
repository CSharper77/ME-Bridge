using MEBridge.Attributes;

namespace MEBridge.Tests;

public enum Map_SourceEnum { ValueA, ValueB, ValueC }
public enum Map_TargetEnum { ValueA, ValueB, ValueC }

public class DiffEnum_Entity
{
    public Map_SourceEnum Status { get; set; }
}

[Map<DiffEnum_Entity>]
public class DiffEnum_Model
{
    public Map_TargetEnum Status { get; set; }
}

public class EnumDifferentValues_Tests
{
    [Fact]
    public async Task Map_DifferentEnumTypesByNameConvention()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new DiffEnum_Entity { Status = Map_SourceEnum.ValueB };

        var result = await mapper.BridgeTo<DiffEnum_Model>(entity, null);

        Assert.Equal(Map_TargetEnum.ValueB, result.Status);
    }
}
