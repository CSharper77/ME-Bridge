using MEBridge.Attributes;

namespace MEBridge.Tests;

public class NNtoN_Entity
{
    public int IntProp { get; set; }
    public bool BoolProp { get; set; }
    public DateTime DateTimeProp { get; set; }
    public decimal DecimalProp { get; set; }
    public Guid GuidProp { get; set; }
}

[Map<NNtoN_Entity>]
public class NNtoN_Model
{
    public int? IntProp { get; set; }
    public bool? BoolProp { get; set; }
    public DateTime? DateTimeProp { get; set; }
    public decimal? DecimalProp { get; set; }
    public Guid? GuidProp { get; set; }
}

public class NonNullableToNullable_Tests
{
    [Fact]
    public async Task Map_NonNullableSourceToNullableTarget()
    {
        var mapper = TestHelpers.CreateMapper();
        var dt = new DateTime(2024, 1, 1);
        var guid = Guid.NewGuid();
        var entity = new NNtoN_Entity
        {
            IntProp = 99,
            BoolProp = true,
            DateTimeProp = dt,
            DecimalProp = 5.50m,
            GuidProp = guid
        };

        var result = await mapper.BridgeTo<NNtoN_Model>(entity, null);

        Assert.Equal(99, result.IntProp);
        Assert.True(result.BoolProp);
        Assert.Equal(dt, result.DateTimeProp);
        Assert.Equal(5.50m, result.DecimalProp);
        Assert.Equal(guid, result.GuidProp);
    }
}
