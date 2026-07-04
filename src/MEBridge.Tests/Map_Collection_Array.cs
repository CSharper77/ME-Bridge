using MEBridge.Attributes;

namespace MEBridge.Tests;

public class CollArr_InnerEntity
{
    public string Name { get; set; } = null!;
}

[Map<CollArr_InnerEntity>]
public class CollArr_InnerModel
{
    public string Name { get; set; } = null!;
}

public class CollArr_OuterEntity
{
    public string Title { get; set; } = null!;
    public CollArr_InnerEntity[] Items { get; set; } = null!;
}

[Map<CollArr_OuterEntity>]
public class CollArr_OuterModel
{
    public string Title { get; set; } = null!;
    [MapProperty("Items")]
    public CollArr_InnerModel[] Items { get; set; } = null!;
}

public class CollectionArray_Tests
{
    [Fact]
    public async Task Map_ArrayOfComplexObjects_MapsCorrectly()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new CollArr_OuterEntity
        {
            Title = "array-test",
            Items = new[]
            {
                new CollArr_InnerEntity { Name = "X" },
                new CollArr_InnerEntity { Name = "Y" }
            }
        };

        var result = await mapper.BridgeTo<CollArr_OuterModel>(entity, null);

        Assert.Equal("array-test", result.Title);
        Assert.NotNull(result.Items);
        Assert.Equal(2, result.Items.Length);
        Assert.Equal("X", result.Items[0].Name);
        Assert.Equal("Y", result.Items[1].Name);
    }
}
