using MEBridge.Attributes;

namespace MEBridge.Tests;

public class CollEmpty_InnerEntity
{
    public string Name { get; set; } = null!;
}

[Map<CollEmpty_InnerEntity>]
public class CollEmpty_InnerModel
{
    public string Name { get; set; } = null!;
}

public class CollEmpty_OuterEntity
{
    public string Title { get; set; } = null!;
    public List<CollEmpty_InnerEntity> Items { get; set; } = null!;
}

[Map<CollEmpty_OuterEntity>]
public class CollEmpty_OuterModel
{
    public string Title { get; set; } = null!;
    [MapProperty("Items")]
    public List<CollEmpty_InnerModel> Items { get; set; } = null!;
}

public class CollectionEmpty_Tests
{
    [Fact]
    public async Task Map_EmptyCollection_ReturnsEmpty()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new CollEmpty_OuterEntity
        {
            Title = "empty",
            Items = new List<CollEmpty_InnerEntity>()
        };

        var result = await mapper.BridgeTo<CollEmpty_OuterModel>(entity, null);

        Assert.Equal("empty", result.Title);
        Assert.NotNull(result.Items);
        Assert.Empty(result.Items);
    }
}
