using MEBridge.Attributes;

namespace MEBridge.Tests;

public class PrivSet_Entity
{
    public PrivSet_Entity() { }
    public PrivSet_Entity(string name, int computed) { Name = name; Computed = computed; }
    public string Name { get; set; } = null!;
    public int Computed { get; private set; }
}

[Bridge<PrivSet_Entity>]
public class PrivSet_Model
{
    public string Name { get; set; } = null!;
    public int Computed { get; set; }
}

public class PrivateSetter_Tests
{
    [Fact]
    public async Task Map_EntityPrivateSetter_MapperReadsViaReflection()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new PrivSet_Entity("test", 42);

        var result = await mapper.BridgeTo<PrivSet_Model>(entity, null);

        Assert.Equal("test", result.Name);
        Assert.Equal(42, result.Computed);
    }
}
