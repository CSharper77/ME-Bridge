using MEBridge.Attributes;

namespace MEBridge.Tests;

public class CollEnum_InnerEntity
{
    public string Name { get; set; } = null!;
}

[Map<CollEnum_InnerEntity>]
public class CollEnum_InnerModel
{
    public string Name { get; set; } = null!;
}

public class CollEnum_OuterEntity
{
    public string Title { get; set; } = null!;
    public IEnumerable<CollEnum_InnerEntity> Items { get; set; } = null!;
}

[Map<CollEnum_OuterEntity>]
public class CollEnum_OuterModel
{
    public string Title { get; set; } = null!;
    [MapProperty("Items")]
    public IEnumerable<CollEnum_InnerModel> Items { get; set; } = null!;
}

public class CollectionIEnumerable_Tests
{
    [Fact]
    public async Task Map_IEnumerable_TypeMismatchThrows()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new CollEnum_OuterEntity
        {
            Title = "ienumerable-test",
            Items = new List<CollEnum_InnerEntity>
            {
                new() { Name = "1" },
                new() { Name = "2" }
            }
        };

        var ex = await Record.ExceptionAsync(() =>
            mapper.BridgeTo<CollEnum_OuterModel>(entity, null));

        Assert.NotNull(ex);
    }
}
