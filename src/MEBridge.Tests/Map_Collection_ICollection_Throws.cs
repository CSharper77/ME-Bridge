using MEBridge.Attributes;

namespace MEBridge.Tests;

public class IColl_InnerEntity
{
    public string Name { get; set; } = null!;
}

[Map<IColl_InnerEntity>]
public class IColl_InnerModel
{
    public string Name { get; set; } = null!;
}

public class IColl_OuterEntity
{
    public string Title { get; set; } = null!;
    public ICollection<IColl_InnerEntity> Items { get; set; } = null!;
}

[Map<IColl_OuterEntity>]
public class IColl_OuterModel
{
    public string Title { get; set; } = null!;
    [MapProperty("Items")]
    public ICollection<IColl_InnerModel> Items { get; set; } = null!;
}

public class CollectionICollection_Tests
{
    [Fact]
    public async Task Map_ICollection_TypeMismatchThrows()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new IColl_OuterEntity
        {
            Title = "icollection",
            Items = new List<IColl_InnerEntity> { new() { Name = "x" } }
        };

        var ex = await Record.ExceptionAsync(() =>
            mapper.BridgeTo<IColl_OuterModel>(entity, null));

        Assert.NotNull(ex);
    }
}
