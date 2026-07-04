using MEBridge.Attributes;

namespace MEBridge.Tests;

public class Nested_InnerEntity
{
    public string Name { get; set; } = null!;
    public int Value { get; set; }
}

[Map<Nested_InnerEntity>]
public class Nested_InnerModel
{
    public string Name { get; set; } = null!;
    public int Value { get; set; }
}

public class Nested_OuterEntity
{
    public string Title { get; set; } = null!;
    public Nested_InnerEntity Inner { get; set; } = null!;
}

[Map<Nested_OuterEntity>]
public class Nested_OuterModel
{
    public string Title { get; set; } = null!;
    [MapProperty("Inner")]
    public Nested_InnerModel Inner { get; set; } = null!;
}

public class NestedRecursive_Tests
{
    [Fact]
    public async Task Map_NestedObject_MapsRecursively()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new Nested_OuterEntity
        {
            Title = "outer",
            Inner = new Nested_InnerEntity { Name = "inner", Value = 7 }
        };

        var result = await mapper.BridgeTo<Nested_OuterModel>(entity, null);

        Assert.Equal("outer", result.Title);
        Assert.NotNull(result.Inner);
        Assert.Equal("inner", result.Inner.Name);
        Assert.Equal(7, result.Inner.Value);
    }
}
