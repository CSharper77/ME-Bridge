namespace MEBridge.Tests;

public class NoAttr_Entity
{
    public string Name { get; set; } = null!;
}

public class NoAttr_Model
{
    public string Name { get; set; } = null!;
}

public class ModelWithoutAttribute_Tests
{
    [Fact]
    public async Task Map_OutputTypeWithoutMapAttribute_ThrowsException()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new NoAttr_Entity { Name = "test" };

        var ex = await Assert.ThrowsAsync<Exception>(() =>
            mapper.BridgeTo<NoAttr_Model>(entity, null));

        Assert.Contains("not mapped", ex.Message);
    }
}
