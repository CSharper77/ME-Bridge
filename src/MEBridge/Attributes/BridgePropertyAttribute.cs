namespace MEBridge.Attributes;

/// <summary>Maps a model property to a differently named entity property.</summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple =  false, Inherited = true)]
public class BridgePropertyAttribute : Attribute
{
    /// <summary>The destination property name on the entity type.</summary>
    public string Destination { get; }
    
    /// <summary>When true, skips name-convention mapping for this property.</summary>
    public bool IgnoreDefaultNamingMap { get; set; } = false;

    /// <summary>Maps this property to the specified destination property name.</summary>
    public BridgePropertyAttribute(string destination, bool ignoreDefaultNamingMap = false)
    {
        Destination = destination;
        IgnoreDefaultNamingMap = ignoreDefaultNamingMap;
    }
}