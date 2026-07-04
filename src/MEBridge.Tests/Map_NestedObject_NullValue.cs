using MEBridge.Attributes;

namespace MEBridge.Tests;

public class NestedNull_InnerEntity
{
    public string Name { get; set; } = null!;
}

[Map<NestedNull_InnerEntity>]
public class NestedNull_InnerModel
{
    public string Name { get; set; } = null!;
}

public class NestedNull_OuterEntity
{
    public string Title { get; set; } = null!;
    public NestedNull_InnerEntity Inner { get; set; } = null!;
}

[Map<NestedNull_OuterEntity>]
public class NestedNull_OuterModel
{
    public string Title { get; set; } = null!;
    [MapProperty("Inner")]
    public NestedNull_InnerModel Inner { get; set; } = null!;
}

public class NestedNullValue_Tests
{
    [Fact]
    public async Task Map_NestedObjectNull_Skips()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new NestedNull_OuterEntity
        {
            Title = "outer",
            Inner = null!
        };

        var result = await mapper.BridgeTo<NestedNull_OuterModel>(entity, null);

        Assert.Equal("outer", result.Title);
        Assert.Null(result.Inner);
    }
}
