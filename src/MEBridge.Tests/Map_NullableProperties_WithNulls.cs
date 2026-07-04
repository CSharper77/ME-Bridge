using MEBridge.Attributes;

namespace MEBridge.Tests;

public class NullableNulls_Entity
{
    public int? NullableInt { get; set; }
    public bool? NullableBool { get; set; }
    public DateTime? NullableDateTime { get; set; }
    public decimal? NullableDecimal { get; set; }
    public Guid? NullableGuid { get; set; }
}

[Map<NullableNulls_Entity>]
public class NullableNulls_Model
{
    public int? NullableInt { get; set; }
    public bool? NullableBool { get; set; }
    public DateTime? NullableDateTime { get; set; }
    public decimal? NullableDecimal { get; set; }
    public Guid? NullableGuid { get; set; }
}

public class NullableWithNulls_Tests
{
    [Fact]
    public async Task Map_NullablePropertiesAllNull()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new NullableNulls_Entity();

        var result = await mapper.BridgeTo<NullableNulls_Model>(entity, null);

        Assert.Null(result.NullableInt);
        Assert.Null(result.NullableBool);
        Assert.Null(result.NullableDateTime);
        Assert.Null(result.NullableDecimal);
        Assert.Null(result.NullableGuid);
    }
}
