using MEBridge.Attributes;

namespace MEBridge.Tests;

public class NullableValues_Entity
{
    public string StringProp { get; set; } = "default";
    public int? NullableInt { get; set; }
    public bool? NullableBool { get; set; }
    public DateTime? NullableDateTime { get; set; }
    public decimal? NullableDecimal { get; set; }
    public Guid? NullableGuid { get; set; }
}

[Bridge<NullableValues_Entity>]
public class NullableValues_Model
{
    public string StringProp { get; set; } = null!;
    public int? NullableInt { get; set; }
    public bool? NullableBool { get; set; }
    public DateTime? NullableDateTime { get; set; }
    public decimal? NullableDecimal { get; set; }
    public Guid? NullableGuid { get; set; }
}

public class NullableWithValues_Tests
{
    [Fact]
    public async Task Map_NullablePropertiesWithValues()
    {
        var mapper = TestHelpers.CreateMapper();
        var dt = new DateTime(2024, 12, 25);
        var guid = Guid.NewGuid();
        var entity = new NullableValues_Entity
        {
            StringProp = "non-null",
            NullableInt = 100,
            NullableBool = true,
            NullableDateTime = dt,
            NullableDecimal = 12.50m,
            NullableGuid = guid
        };

        var result = await mapper.BridgeTo<NullableValues_Model>(entity, null);

        Assert.Equal("non-null", result.StringProp);
        Assert.Equal(100, result.NullableInt);
        Assert.True(result.NullableBool);
        Assert.Equal(dt, result.NullableDateTime);
        Assert.Equal(12.50m, result.NullableDecimal);
        Assert.Equal(guid, result.NullableGuid);
    }
}
