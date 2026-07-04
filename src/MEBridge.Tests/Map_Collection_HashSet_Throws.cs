using MEBridge.Attributes;

namespace MEBridge.Tests;

public class HashSetColl_InnerEntity
{
    public string Name { get; set; } = null!;
}

[Bridge<HashSetColl_InnerEntity>]
public class HashSetColl_InnerModel
{
    public string Name { get; set; } = null!;
}

public class HashSetColl_OuterEntity
{
    public string Title { get; set; } = null!;
    public HashSet<HashSetColl_InnerEntity> Items { get; set; } = null!;
}

[Bridge<HashSetColl_OuterEntity>]
public class HashSetColl_OuterModel
{
    public string Title { get; set; } = null!;
    [BridgeProperty("Items")]
    public HashSet<HashSetColl_InnerModel> Items { get; set; } = null!;
}

public class CollectionHashSet_Tests
{
    [Fact]
    public async Task Map_HashSetCollection_InvalidCastThrows()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new HashSetColl_OuterEntity
        {
            Title = "hashset",
            Items = new HashSet<HashSetColl_InnerEntity> { new() { Name = "x" } }
        };

        var ex = await Record.ExceptionAsync(() =>
            mapper.BridgeTo<HashSetColl_OuterModel>(entity, null));

        Assert.NotNull(ex);
    }
}
