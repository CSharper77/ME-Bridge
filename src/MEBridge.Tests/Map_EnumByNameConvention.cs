using MEBridge.Attributes;

namespace MEBridge.Tests;

public enum Map_EnumStatus { Active, Inactive, Pending }

public class EnumConv_Entity
{
    public Map_EnumStatus Status { get; set; }
}

[Bridge<EnumConv_Entity>]
public class EnumConv_Model
{
    public Map_EnumStatus Status { get; set; }
}

public class EnumByNameConvention_Tests
{
    [Fact]
    public async Task Map_EnumByNameConvention()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new EnumConv_Entity { Status = Map_EnumStatus.Pending };

        var result = await mapper.BridgeTo<EnumConv_Model>(entity, null);

        Assert.Equal(Map_EnumStatus.Pending, result.Status);
    }
}
