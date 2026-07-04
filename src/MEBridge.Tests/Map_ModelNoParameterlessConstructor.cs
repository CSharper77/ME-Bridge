using MEBridge.Attributes;

namespace MEBridge.Tests;

public class NoCtor_Entity
{
    public string Name { get; set; } = null!;
}

[Bridge<NoCtor_Entity>]
public class NoCtor_Model
{
    public NoCtor_Model(string name) { Name = name; }
    public string Name { get; set; }
}

public class NoParameterlessCtor_Tests
{
    [Fact]
    public async Task Map_ModelWithoutParameterlessCtor_Throws()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new NoCtor_Entity { Name = "test" };

        var ex = await Record.ExceptionAsync(() =>
            mapper.BridgeTo<NoCtor_Model>(entity, null));

        Assert.NotNull(ex);
    }
}
