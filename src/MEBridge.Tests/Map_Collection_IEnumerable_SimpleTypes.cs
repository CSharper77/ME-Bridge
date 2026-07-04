using MEBridge.Attributes;

namespace MEBridge.Tests;

public class SimpleEnumColl_Entity
{
    public string Title { get; set; } = null!;
    public IEnumerable<string> Names { get; set; } = null!;
}

[Bridge<SimpleEnumColl_Entity>]
public class SimpleEnumColl_Model
{
    public string Title { get; set; } = null!;
    [BridgeProperty("Names")]
    public IEnumerable<string> Names { get; set; } = null!;
}

public class IEnumerableSimpleTypes_Tests
{
    [Fact]
    public async Task Map_IEnumerableOfSimpleTypes_CopiesValues()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new SimpleEnumColl_Entity
        {
            Title = "ienum",
            Names = new List<string> { "a", "b" }
        };

        var result = await mapper.BridgeTo<SimpleEnumColl_Model>(entity, null);

        Assert.Equal("ienum", result.Title);
        Assert.NotNull(result.Names);
        var list = result.Names.ToList();
        Assert.Equal(2, list.Count);
        Assert.Equal("a", list[0]);
        Assert.Equal("b", list[1]);
    }
}
