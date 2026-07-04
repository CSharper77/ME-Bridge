using MEBridge.Attributes;

namespace MEBridge.Tests;

public class SimpleTypes_Entity
{
    public string StringProp { get; set; } = null!;
    public int IntProp { get; set; }
    public bool BoolProp { get; set; }
    public decimal DecimalProp { get; set; }
    public DateTime DateTimeProp { get; set; }
    public long LongProp { get; set; }
    public double DoubleProp { get; set; }
    public float FloatProp { get; set; }
    public byte ByteProp { get; set; }
    public short ShortProp { get; set; }
    public Guid GuidProp { get; set; }
}

[Map<SimpleTypes_Entity>]
public class SimpleTypes_Model
{
    public string StringProp { get; set; } = null!;
    public int IntProp { get; set; }
    public bool BoolProp { get; set; }
    public decimal DecimalProp { get; set; }
    public DateTime DateTimeProp { get; set; }
    public long LongProp { get; set; }
    public double DoubleProp { get; set; }
    public float FloatProp { get; set; }
    public byte ByteProp { get; set; }
    public short ShortProp { get; set; }
    public Guid GuidProp { get; set; }
}

public class AllSimpleTypes_Tests
{
    [Fact]
    public async Task Map_AllSimpleValueTypes()
    {
        var mapper = TestHelpers.CreateMapper();
        var guid = Guid.NewGuid();
        var dt = new DateTime(2025, 1, 1, 10, 30, 0);
        var entity = new SimpleTypes_Entity
        {
            StringProp = "hello",
            IntProp = 42,
            BoolProp = true,
            DecimalProp = 99.99m,
            DateTimeProp = dt,
            LongProp = 9876543210L,
            DoubleProp = 3.14159,
            FloatProp = 2.718f,
            ByteProp = 255,
            ShortProp = 32767,
            GuidProp = guid
        };

        var result = await mapper.BridgeTo<SimpleTypes_Model>(entity, null);

        Assert.Equal("hello", result.StringProp);
        Assert.Equal(42, result.IntProp);
        Assert.True(result.BoolProp);
        Assert.Equal(99.99m, result.DecimalProp);
        Assert.Equal(dt, result.DateTimeProp);
        Assert.Equal(9876543210L, result.LongProp);
        Assert.Equal(3.14159, result.DoubleProp);
        Assert.Equal(2.718f, result.FloatProp);
        Assert.Equal(255, result.ByteProp);
        Assert.Equal(32767, result.ShortProp);
        Assert.Equal(guid, result.GuidProp);
    }
}
