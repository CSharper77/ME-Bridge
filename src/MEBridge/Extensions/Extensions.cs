using Microsoft.Extensions.DependencyInjection;
using Reflectify;
using Reflectify.Extensions;

namespace MEBridge.Extensions;

/// <summary>Extension methods for registering MEBridge services with DI.</summary>
public static class Extensions
{
    /// <summary>Registers <see cref="IReflection"/>, <see cref="IBridgeConfiguration"/> (optional), and <see cref="Bridge"/> as singletons.</summary>
    public static void AddMapper(this IServiceCollection services, IBridgeConfiguration? mappingConfiguration = null)
    {
        services.AddReflectify(e =>
        {
            e.DetectionVisibility = Reflectify.Models.DetectionVisibility.OnlyPublic;
            e.LifeTime = Reflectify.Models.LifeTime.Singleton;
        });

        if (mappingConfiguration is not null)
            services.AddSingleton<IBridgeConfiguration>(mappingConfiguration);

        services.AddSingleton<Bridge>();
    }
}