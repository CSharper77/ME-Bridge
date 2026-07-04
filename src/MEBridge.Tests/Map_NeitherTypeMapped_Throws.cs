namespace MEBridge.Tests;

public class NeitherMapped_A
{
    public string Name { get; set; } = null!;
}

public class NeitherMapped_B
{
    public string Name { get; set; } = null!;
}

public class NeitherTypeMapped_Tests
{
    [Fact]
    public async Task Map_NeitherTypeHasMapAttribute_ThrowsException()
    {
        var mapper = TestHelpers.CreateMapper();
        var a = new NeitherMapped_A { Name = "test" };

        var ex = await Assert.ThrowsAsync<Exception>(() =>
            mapper.BridgeTo<NeitherMapped_B>(a, null));

        Assert.Contains("not mapped", ex.Message);
        Assert.Contains("BridgeAttribute", ex.Message);
    }
}
