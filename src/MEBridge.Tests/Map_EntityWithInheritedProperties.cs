using MEBridge.Attributes;

namespace MEBridge.Tests;

public class Inherit_BaseEntity
{
    public string BaseName { get; set; } = null!;
}

public class Inherit_DerivedEntity : Inherit_BaseEntity
{
    public string DerivedName { get; set; } = null!;
}

[Bridge<Inherit_DerivedEntity>]
public class Inherit_Model
{
    public string BaseName { get; set; } = null!;
    public string DerivedName { get; set; } = null!;
}

public class InheritedProperties_Tests
{
    [Fact]
    public async Task Map_EntityWithInheritedProperties_MapsAll()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new Inherit_DerivedEntity { BaseName = "base", DerivedName = "derived" };

        var result = await mapper.BridgeTo<Inherit_Model>(entity, null);

        Assert.Equal("base", result.BaseName);
        Assert.Equal("derived", result.DerivedName);
    }
}
