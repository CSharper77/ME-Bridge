using MEBridge.Attributes;

namespace MEBridge.Tests;

public class NoMapProp_InnerEntity
{
    public string Name { get; set; } = null!;
}

[Map<NoMapProp_InnerEntity>]
public class NoMapProp_InnerModel
{
    public string Name { get; set; } = null!;
}

public class NoMapProp_OuterEntity
{
    public string Title { get; set; } = null!;
    public NoMapProp_InnerEntity Inner { get; set; } = null!;
}

[Map<NoMapProp_OuterEntity>]
public class NoMapProp_OuterModel
{
    public string Title { get; set; } = null!;
    public NoMapProp_InnerModel Inner { get; set; } = null!;
}

public class NestedWithoutMapProperty_Tests
{
    [Fact]
    public async Task Map_NestedObjectViaNameConvention()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new NoMapProp_OuterEntity
        {
            Title = "outer",
            Inner = new NoMapProp_InnerEntity { Name = "inner" }
        };

        var result = await mapper.BridgeTo<NoMapProp_OuterModel>(entity, null);

        Assert.Equal("outer", result.Title);
        Assert.NotNull(result.Inner);
        Assert.Equal("inner", result.Inner.Name);
    }
}
