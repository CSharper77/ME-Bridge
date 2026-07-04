using Microsoft.Extensions.DependencyInjection;
using Reflectify;

namespace MEBridge.Tests;

public class CallbackMappingConfiguration : IBridgeConfiguration
{
    private readonly Func<object> _factory;

    public CallbackMappingConfiguration(Func<object> factory) => _factory = factory;

    public string PrimaryKeyPropertyName => "Id";
    public bool IgnoreDefaultNamingMap { get; set; }
    public string ModelPrimaryKeyPropertyName => "Id";
    public Task AfterBridge<TIn, TOut>(TIn input, TOut output, IServiceProvider serviceProvider) where TIn : class where TOut : class => Task.CompletedTask;
    public bool IsEntityFremworkClass<T>(T type) where T : class => false;

    public TOut CreateModelInstance<TOut>()
    {
        return (TOut)_factory();
    }

    public Task<TEntity> GetOrCreateEntity<TEntity>(IServiceProvider serviceProvider, object model)
        where TEntity : class => Task.FromResult<TEntity>(null!);

}

public static class TestHelpers
{
    public static ServiceProvider CreateServiceProvider(IBridgeConfiguration? config = null)
    {
        var services = new ServiceCollection();
        if (config is not null)
            services.AddSingleton<IBridgeConfiguration>(config);
        return services.BuildServiceProvider();
    }

    public static Bridge CreateMapper(IBridgeConfiguration? config = null)
    {
        return new Bridge(new Reflectify.Reflectify(new Reflectify.Models.ReflectifyConfiguration()), CreateServiceProvider(config));
    }
}
