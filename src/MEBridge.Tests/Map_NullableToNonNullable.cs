using MEBridge.Attributes;

namespace MEBridge.Tests;

public class NtoNN_Entity
{
    public int? NullableInt { get; set; }
    public bool? NullableBool { get; set; }
    public DateTime? NullableDateTime { get; set; }
    public decimal? NullableDecimal { get; set; }
    public Guid? NullableGuid { get; set; }
}

[Map<NtoNN_Entity>]
public class NtoNN_Model
{
    public int NullableInt { get; set; }
    public bool NullableBool { get; set; }
    public DateTime NullableDateTime { get; set; }
    public decimal NullableDecimal { get; set; }
    public Guid NullableGuid { get; set; }
}

public class NullableToNonNullable_Tests
{
    [Fact]
    public async Task Map_NullableSourceToNonNullableTarget()
    {
        var mapper = TestHelpers.CreateMapper();
        var dt = new DateTime(2024, 6, 15);
        var guid = Guid.NewGuid();
        var entity = new NtoNN_Entity
        {
            NullableInt = 42,
            NullableBool = false,
            NullableDateTime = dt,
            NullableDecimal = 10.99m,
            NullableGuid = guid
        };

        var result = await mapper.BridgeTo<NtoNN_Model>(entity, null);

        Assert.Equal(42, result.NullableInt);
        Assert.False(result.NullableBool);
        Assert.Equal(dt, result.NullableDateTime);
        Assert.Equal(10.99m, result.NullableDecimal);
        Assert.Equal(guid, result.NullableGuid);
    }
}
