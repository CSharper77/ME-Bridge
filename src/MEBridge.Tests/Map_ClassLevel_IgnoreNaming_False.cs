using MEBridge.Attributes;

namespace MEBridge.Tests;

public class IgnNameFalse_Entity
{
    public string Name { get; set; } = null!;
}

[Bridge<IgnNameFalse_Entity>(false)]
public class IgnNameFalse_Model
{
    public string Name { get; set; } = null!;
}

public class ClassLevelIgnoreNamingFalse_Tests
{
    [Fact]
    public async Task Map_ClassLevelIgnoreNamingFalse_NameConventionWorks()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new IgnNameFalse_Entity { Name = "value" };

        var result = await mapper.BridgeTo<IgnNameFalse_Model>(entity, null);

        Assert.Equal("value", result.Name);
    }
}
