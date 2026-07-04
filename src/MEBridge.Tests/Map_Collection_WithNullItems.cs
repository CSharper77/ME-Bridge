using MEBridge.Attributes;

namespace MEBridge.Tests;

public class CollNullItem_Entity
{
    public string Name { get; set; } = null!;
}

[Map<CollNullItem_Entity>]
public class CollNullItem_Model
{
    public string Name { get; set; } = null!;
}

public class CollNullItem_OuterEntity
{
    public string Title { get; set; } = null!;
    public List<CollNullItem_Entity?> Items { get; set; } = null!;
}

[Map<CollNullItem_OuterEntity>]
public class CollNullItem_OuterModel
{
    public string Title { get; set; } = null!;
    [MapProperty("Items")]
    public List<CollNullItem_Model?> Items { get; set; } = null!;
}

public class CollectionWithNullItems_Tests
{
    [Fact]
    public async Task Map_CollectionWithNullItems_SkipsNulls()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new CollNullItem_OuterEntity
        {
            Title = "with-nulls",
            Items = new List<CollNullItem_Entity?>
            {
                new() { Name = "valid" },
                null,
                new() { Name = "also-valid" }
            }
        };

        var result = await mapper.BridgeTo<CollNullItem_OuterModel>(entity, null);

        Assert.Equal("with-nulls", result.Title);
        Assert.NotNull(result.Items);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal("valid", result.Items[0]!.Name);
        Assert.Equal("also-valid", result.Items[1]!.Name);
    }
}
