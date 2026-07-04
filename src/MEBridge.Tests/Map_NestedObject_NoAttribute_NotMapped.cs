using MEBridge.Attributes;

namespace MEBridge.Tests;

public class NoAttrNest_InnerEntity
{
    public string Name { get; set; } = null!;
}

public class NoAttrNest_InnerModel { }

public class NoAttrNest_OuterEntity
{
    public string Title { get; set; } = null!;
    public NoAttrNest_InnerEntity Inner { get; set; } = null!;
}

[Map<NoAttrNest_OuterEntity>]
public class NoAttrNest_OuterModel
{
    public string Title { get; set; } = null!;
    [MapProperty("Inner")]
    public NoAttrNest_InnerModel Inner { get; set; } = null!;
}

public class NestedNoAttribute_Tests
{
    [Fact]
    public async Task Map_NestedObjectWithoutMapAttribute_Throws()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new NoAttrNest_OuterEntity
        {
            Title = "outer",
            Inner = new NoAttrNest_InnerEntity { Name = "inner" }
        };

        var ex = await Assert.ThrowsAsync<Exception>(() =>
            mapper.BridgeTo<NoAttrNest_OuterModel>(entity, null));

        Assert.Contains("not mapped", ex.Message);
    }
}
