using MEBridge.Attributes;

namespace MEBridge.Tests;

public class Unchanged_Entity
{
    public string Name { get; set; } = null!;
    public int Age { get; set; }
}

[Bridge<Unchanged_Entity>]
public class Unchanged_Model
{
    public string Name { get; set; } = null!;
    public int Age { get; set; }
}

public class EntityUnchanged_Tests
{
    [Fact]
    public async Task Map_EntityPropertiesRemainUnchanged()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new Unchanged_Entity { Name = "original", Age = 25 };

        await mapper.BridgeTo<Unchanged_Model>(entity, null);

        Assert.Equal("original", entity.Name);
        Assert.Equal(25, entity.Age);
    }
}
