using MEBridge.Attributes;

namespace MEBridge.Tests;

public class CollNull_InnerEntity
{
    public string Name { get; set; } = null!;
}

[Map<CollNull_InnerEntity>]
public class CollNull_InnerModel
{
    public string Name { get; set; } = null!;
}

public class CollNull_OuterEntity
{
    public string Title { get; set; } = null!;
    public List<CollNull_InnerEntity> Items { get; set; } = null!;
}

[Map<CollNull_OuterEntity>]
public class CollNull_OuterModel
{
    public string Title { get; set; } = null!;
    [MapProperty("Items")]
    public List<CollNull_InnerModel> Items { get; set; } = null!;
}

public class CollectionNullValue_Tests
{
    [Fact]
    public async Task Map_NullCollection_Skips()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new CollNull_OuterEntity
        {
            Title = "null-collection",
            Items = null!
        };

        var result = await mapper.BridgeTo<CollNull_OuterModel>(entity, null);

        Assert.Equal("null-collection", result.Title);
        Assert.Null(result.Items);
    }
}
