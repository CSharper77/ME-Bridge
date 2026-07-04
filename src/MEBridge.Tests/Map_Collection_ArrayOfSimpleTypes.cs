using MEBridge.Attributes;

namespace MEBridge.Tests;

public class SimpleArr_Entity
{
    public string Title { get; set; } = null!;
    public string[] Names { get; set; } = null!;
}

[Map<SimpleArr_Entity>]
public class SimpleArr_Model
{
    public string Title { get; set; } = null!;
    [MapProperty("Names")]
    public string[] Names { get; set; } = null!;
}

public class ArraySimpleTypes_Tests
{
    [Fact]
    public async Task Map_ArrayOfSimpleTypes_ElementMappingThrows()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new SimpleArr_Entity
        {
            Title = "arr",
            Names = new[] { "x", "y", "z" }
        };

        var ex = await Record.ExceptionAsync(() =>
            mapper.BridgeTo<SimpleArr_Model>(entity, null));

        Assert.NotNull(ex);
        Assert.Contains("not mapped", ex!.Message);
    }
}
