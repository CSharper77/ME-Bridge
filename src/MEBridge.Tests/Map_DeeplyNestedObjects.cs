using MEBridge.Attributes;

namespace MEBridge.Tests;

public class Deep_GrandchildEntity
{
    public string Value { get; set; } = null!;
}

public class Deep_ChildEntity
{
    public string Name { get; set; } = null!;
    public Deep_GrandchildEntity Grandchild { get; set; } = null!;
}

public class Deep_RootEntity
{
    public string Title { get; set; } = null!;
    public Deep_ChildEntity Child { get; set; } = null!;
}

[Bridge<Deep_GrandchildEntity>]
public class Deep_GrandchildModel
{
    public string Value { get; set; } = null!;
}

[Bridge<Deep_ChildEntity>]
public class Deep_ChildModel
{
    public string Name { get; set; } = null!;
    [BridgeProperty("Grandchild")]
    public Deep_GrandchildModel Grandchild { get; set; } = null!;
}

[Bridge<Deep_RootEntity>]
public class Deep_RootModel
{
    public string Title { get; set; } = null!;
    [BridgeProperty("Child")]
    public Deep_ChildModel Child { get; set; } = null!;
}

public class DeeplyNested_Tests
{
    [Fact]
    public async Task Map_ThreeLevelsDeep_MapsAll()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new Deep_RootEntity
        {
            Title = "root",
            Child = new Deep_ChildEntity
            {
                Name = "child",
                Grandchild = new Deep_GrandchildEntity { Value = "grand" }
            }
        };

        var result = await mapper.BridgeTo<Deep_RootModel>(entity, null);

        Assert.Equal("root", result.Title);
        Assert.NotNull(result.Child);
        Assert.Equal("child", result.Child.Name);
        Assert.NotNull(result.Child.Grandchild);
        Assert.Equal("grand", result.Child.Grandchild.Value);
    }
}
