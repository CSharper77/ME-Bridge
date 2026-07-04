using Microsoft.EntityFrameworkCore;

namespace MEBridge;

/// <summary>Default implementation of <see cref="IBridgeConfiguration"/> with convention-based settings.</summary>
public class BridgeConfiguration : IBridgeConfiguration
{
    /// <summary>The entity primary key property name used for EF lookups.</summary>
    public string PrimaryKeyPropertyName { get; } = "Id";

    /// <summary>The model primary key property name used for matching with entities.</summary>
    public string ModelPrimaryKeyPropertyName { get; } = "Id";

    /// <summary>Globally disables name-convention property mapping.</summary>
    public bool IgnoreDefaultNamingMap { get; set; } = false;

    /// <summary>Hook invoked after each bridge operation completes.</summary>
    public Task AfterBridge<TIn, TOut>(TIn input, TOut output, IServiceProvider serviceProvider) where TIn : class where TOut : class
    {
        return Task.CompletedTask;
    }

    /// <summary>Returns false by default; override to identify EF entity classes.</summary>
    public bool IsEntityFremworkClass<T>(T type) where T : class
    {
        return false;
    }

    /// <summary>Creates a model instance using <see cref="Activator"/>.</summary>
    public TOut CreateModelInstance<TOut>()
    {
        return Activator.CreateInstance<TOut>();
    }

    /// <summary>Returns null by default; override to inject custom entity retrieval logic.</summary>
    public async Task<TEntity> GetOrCreateEntity<TEntity>(IServiceProvider serviceProvider, object model) where TEntity : class
    {
        return null;
    }

    /// <summary>Returns the default primary key property name "Id".</summary>
    public string GetPrimaryKeyPropertyName<TOut>()
    {
        return "Id";
    }

}

/// <summary>Configuration interface for customizing mapping behavior.</summary>
public interface IBridgeConfiguration
{
    /// <summary>Entity primary key property name used for EF Core lookups.</summary>
    public string PrimaryKeyPropertyName { get; }
    /// <summary>Model primary key property name used for matching with entities.</summary>
    public string ModelPrimaryKeyPropertyName { get; }

    /// <summary>Globally disables name-convention property matching.</summary>
    public bool IgnoreDefaultNamingMap { get; set; } 
    
    /// <summary>Hook invoked after each bridge operation completes.</summary>
    Task AfterBridge<TIn,TOut>(TIn input, TOut output, IServiceProvider serviceProvider) where TIn : class where TOut : class;
    
    /// <summary>Returns true if the type is an EF entity class.</summary>
    bool IsEntityFremworkClass<T>(T type) where T: class;
    /// <summary>Creates a model instance.</summary>
    TOut CreateModelInstance<TOut>();

    /// <summary>Custom entity retrieval. Return null to fall back to default EF lookup.</summary>
    Task<TEntity> GetOrCreateEntity<TEntity>(IServiceProvider serviceProvider, object model)
        where TEntity : class;
}