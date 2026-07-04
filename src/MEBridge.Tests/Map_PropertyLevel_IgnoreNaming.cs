using MEBridge.Attributes;

namespace MEBridge.Tests;

public class PropIgn_Entity
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
}

[Map<PropIgn_Entity>(true)]
public class PropIgn_Model
{
    [MapProperty("FirstName")]
    public string FirstName { get; set; } = null!;

    [MapProperty("LastName")]
    public string LastName { get; set; } = null!;
}

public class ClassLevelIgnoreWithMapProperty_Tests
{
    [Fact]
    public async Task Map_ClassIgnoreTrue_MapPropertyStillWorks()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new PropIgn_Entity { FirstName = "John", LastName = "Doe" };

        var result = await mapper.BridgeTo<PropIgn_Model>(entity, null);

        Assert.Equal("John", result.FirstName);
        Assert.Equal("Doe", result.LastName);
    }
}

// ── Property-level ignore naming (NOT supported by the mapper yet) ──

public class PropNameIgn_Entity
{
    public string ShouldIgnore { get; set; } = null!;
}

[Map<PropNameIgn_Entity>]
public class PropNameIgn_Model
{
    [MapProperty("", ignoreDefaultNamingMap: true)]
    public string ShouldIgnore { get; set; } = null!;
}

public class PropertyLevelIgnoreNaming_Tests
{
    [Fact]
    public async Task Map_PropertyLevelIgnoreNamingTrue_ShouldNotMatchByName()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new PropNameIgn_Entity { ShouldIgnore = "not-mapped" };

        var result = await mapper.BridgeTo<PropNameIgn_Model>(entity, null);

        Assert.Null(result.ShouldIgnore);
    }
}
