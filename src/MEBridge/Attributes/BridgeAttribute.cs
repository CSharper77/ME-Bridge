namespace MEBridge.Attributes;

/// <summary>Marks a class as a model and declares its corresponding entity type for mapping.</summary>
[AttributeUsage(AttributeTargets.Class)]
public class BridgeAttribute : Attribute
{
    /// <summary>The target entity type this model maps to.</summary>
    public Type TargetType { get; set; }

    /// <summary>When true, disables property name convention matching for all properties.</summary>
    public bool IgnoreDefaultNamingMap { get; set; } = false;

    /// <summary>Maps the model to the specified entity type.</summary>
    public BridgeAttribute(Type targetType, bool ignoreDefaultNamingMap = false)
    {
        TargetType = targetType;
        IgnoreDefaultNamingMap = ignoreDefaultNamingMap;
    }
}

/// <summary>Generic version of <see cref="BridgeAttribute"/> that infers the entity type from the type parameter.</summary>
[AttributeUsage(AttributeTargets.Class)]
public class BridgeAttribute<T> : BridgeAttribute
{
    /// <summary>Maps the model to entity type <typeparamref name="T"/>.</summary>
    public BridgeAttribute(bool ignoreDefaultNamingMap = false)  : base(typeof(T), ignoreDefaultNamingMap)
    {
        
    }
}