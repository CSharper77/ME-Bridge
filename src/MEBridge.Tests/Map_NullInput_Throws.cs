using MEBridge.Attributes;

namespace MEBridge.Tests;

public class NullInput_Entity
{
    public string Name { get; set; } = null!;
}

[Bridge<NullInput_Entity>]
public class NullInput_Model
{
    public string Name { get; set; } = null!;
}

public class NullInput_Tests
{
    [Fact]
    public async Task Map_NullInput_ThrowsNullRef()
    {
        var mapper = TestHelpers.CreateMapper();

        var ex = await Record.ExceptionAsync(() =>
            mapper.BridgeTo<NullInput_Model>(null!, null));

        Assert.NotNull(ex);
    }
}
