using MEBridge.Attributes;

namespace MEBridge.Tests;

public class CollList_InnerEntity
{
    public string Name { get; set; } = null!;
}

[Map<CollList_InnerEntity>]
public class CollList_InnerModel
{
    public string Name { get; set; } = null!;
}

public class CollList_OuterEntity
{
    public string Title { get; set; } = null!;
    public List<CollList_InnerEntity> Items { get; set; } = null!;
}

[Map<CollList_OuterEntity>]
public class CollList_OuterModel
{
    public string Title { get; set; } = null!;
    [MapProperty("Items")]
    public List<CollList_InnerModel> Items { get; set; } = null!;
}

public class CollectionList_Tests
{
    [Fact]
    public async Task Map_ListOfComplexObjects_AllItemsMapped()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new CollList_OuterEntity
        {
            Title = "list-test",
            Items = new List<CollList_InnerEntity>
            {
                new() { Name = "A" },
                new() { Name = "B" },
                new() { Name = "C" }
            }
        };

        var result = await mapper.BridgeTo<CollList_OuterModel>(entity, null);

        Assert.Equal("list-test", result.Title);
        Assert.NotNull(result.Items);
        Assert.Equal(3, result.Items.Count);
        Assert.Equal("A", result.Items[0].Name);
        Assert.Equal("B", result.Items[1].Name);
        Assert.Equal("C", result.Items[2].Name);
    }
}
