using MEBridge.Attributes;

namespace MEBridge.Tests;

public class SimpleList_Entity
{
    public string Title { get; set; } = null!;
    public List<string> Names { get; set; } = null!;
}

[Bridge<SimpleList_Entity>]
public class SimpleList_Model
{
    public string Title { get; set; } = null!;
    [BridgeProperty("Names")]
    public List<string> Names { get; set; } = null!;
}

public class CollectionListOfSimpleTypes_Tests
{
    [Fact]
    public async Task Map_ListOfSimpleTypes_ElementMappingThrows()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new SimpleList_Entity
        {
            Title = "list",
            Names = new List<string> { "a", "b", "c" }
        };

        var ex = await Record.ExceptionAsync(() =>
            mapper.BridgeTo<SimpleList_Model>(entity, null));

        Assert.NotNull(ex);
        Assert.Contains("not mapped", ex!.Message);
    }
}
