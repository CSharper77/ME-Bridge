using MEBridge.Attributes;

namespace MEBridge.Tests;

public class DictColl_Entity
{
    public string Title { get; set; } = null!;
    public Dictionary<string, string> Items { get; set; } = null!;
}

[Map<DictColl_Entity>]
public class DictColl_Model
{
    public string Title { get; set; } = null!;
    [MapProperty("Items")]
    public Dictionary<string, string> Items { get; set; } = null!;
}

public class CollectionDictionary_Tests
{
    [Fact]
    public async Task Map_DictionaryCollection_InvalidCastThrows()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new DictColl_Entity
        {
            Title = "dict",
            Items = new Dictionary<string, string> { { "k", "v" } }
        };

        var ex = await Record.ExceptionAsync(() =>
            mapper.BridgeTo<DictColl_Model>(entity, null));

        Assert.NotNull(ex);
    }
}
