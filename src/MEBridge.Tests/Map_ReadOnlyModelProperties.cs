using MEBridge.Attributes;

namespace MEBridge.Tests;

public class ReadOnly_Entity
{
    public string Name { get; set; } = null!;
    public int Age { get; set; }
}

[Map<ReadOnly_Entity>]
public class ReadOnly_Model
{
    public string Name { get; set; } = null!;
    public int Age { get; }
}

public class ReadOnlyModelProperties_Tests
{
    [Fact]
    public async Task Map_ModelHasReadOnlyProperty_Throws()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new ReadOnly_Entity { Name = "test", Age = 99 };

        var ex = await Record.ExceptionAsync(() =>
            mapper.BridgeTo<ReadOnly_Model>(entity, null));

        Assert.NotNull(ex);
        Assert.Contains("set method", ex!.Message, StringComparison.OrdinalIgnoreCase);
    }
}
