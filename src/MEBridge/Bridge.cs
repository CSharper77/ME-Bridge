using System.Collections;
using System.Reflection;
using MEBridge.Attributes;

namespace MEBridge;
using Microsoft.EntityFrameworkCore;
using Reflectify;
using Reflectify.Models;
using System.Collections.Concurrent;

/// <summary>Main mapper class that maps between model and entity objects using conventions and attributes.</summary>
public class Bridge 
{
    /// <summary>Optional mapping configuration for custom behavior.</summary>
    public IBridgeConfiguration MappingConfiguration { get; }
    /// <summary>Reflection service for type and attribute inspection.</summary>
    public IReflectify Reflectify { get; }
    /// <summary>Service provider for resolving dependencies.</summary>
    public IServiceProvider ServiceProvider { get; }

    /// <summary>Cache of already-initialized (model → entity) type pairs.</summary>
    public ConcurrentDictionary<Type, Type> InitializedTypes = new();
    /// <summary>Bidirectional mapping dictionary linking model and entity properties.</summary>
    public ConcurrentDictionary<PropertyInfo, PropertyInfo> PropertiesMappingDictionary = new();
    
    /// <summary>Initializes the mapper with reflection and DI services.</summary>
    public Bridge(IReflectify reflectify, IServiceProvider serviceProvider)
    {
        Reflectify = reflectify;
        ServiceProvider = serviceProvider;
        MappingConfiguration = (IBridgeConfiguration?)serviceProvider.GetService(typeof(IBridgeConfiguration));
    }
    
    /// <summary>Creates a model instance using the configured factory or Activator.</summary>
    private TOut CreateModelInstance<TOut>()
    {
        if (MappingConfiguration is not null)
            return MappingConfiguration.CreateModelInstance<TOut>();
        
        return Activator.CreateInstance<TOut>();
    }
    
    /// <summary>Public entry point: maps the input object to <typeparamref name="TOutput"/> using a DbContext for persistence lookups.</summary>
    public async Task<TOutput> BridgeTo<TOutput>(object input, DbContext? context) where TOutput : class
    {
        return await To<TOutput>(input, context, null);
    }

    /// <summary>Internal mapping logic: resolves model/entity types, initializes the dictionary, creates or retrieves the entity, and maps properties.</summary>
    private async Task<TOutput> To<TOutput>(object input, DbContext? context, TOutput? output = null) where  TOutput : class
    {
        var inputType = input.GetType();
        var types = GetModelAndEntityTypes(inputType, typeof(TOutput));
        var entityType = types.EntityType;
        var modelType = types.ModelType;
        var modelMappingAttribute = Reflectify.GetTypeInfo(modelType).GetAttribute<MapAttribute>();
        
        if (modelMappingAttribute is null)
            throw new Exception($"The model of type {modelType} is not mapped, use the '{nameof(MapAttribute)}'.");

        InitializeDictionary(modelType, entityType, modelMappingAttribute);

        if (inputType == entityType)
        {
            output = CreateModelInstance<TOutput>();
        }
        else
        {
            if(output is null)
                output = await GetOrCreateEntity<TOutput>(input, context);
        }

        await MapTypes(input, output, modelType, context);

        if(MappingConfiguration is not null)
            await MappingConfiguration.AfterBridge(input, output, ServiceProvider);

        return output;
    }

    /// <summary>Builds the bidirectional property mapping dictionary between model and entity types using name convention and attributes.</summary>
    private void InitializeDictionary(Type modelType, Type entityType, MapAttribute modelMappingAttribute)
    {
        if(InitializedTypes.TryGetValue(modelType, out var value) && value == entityType)
            return;

        var modelProperties = Reflectify.GetProperties(modelType);
        var entityProperties = Reflectify.GetProperties(entityType);

        if(modelProperties is null || modelProperties.Count == 0)
        {
            InitializedTypes.TryAdd(modelType, entityType);
        }

        if (modelProperties is null)
            return;

        foreach(var extendedModelProp in modelProperties)
        {
            PropertyInfo? entityProperty = null;

            if(!ShouldIgnorePropertyNamingConventionMapping(modelMappingAttribute, extendedModelProp))
                entityProperty = entityProperties?.FirstOrDefault(w => w.PropertyInfo.Name == extendedModelProp.PropertyInfo.Name)?.PropertyInfo;

            // when name convension 
            if (entityProperty is not null)
            {
                // add the two way mapping : model > entity and entity > model
                PropertiesMappingDictionary.TryAdd(extendedModelProp.PropertyInfo, entityProperty);
                PropertiesMappingDictionary.TryAdd(entityProperty, extendedModelProp.PropertyInfo);
            }

            var modelPropertyAttribute = extendedModelProp.GetAttribute<MapPropertyAttribute>();

            // using attribute
            if(modelPropertyAttribute is not null && !string.IsNullOrWhiteSpace(modelPropertyAttribute.Destination))
            {
                entityProperty = Reflectify.GetProperty(entityType, modelPropertyAttribute.Destination)?.PropertyInfo;
                if (entityProperty is null)
                    throw new Exception($"Unable to map the property name '{extendedModelProp.PropertyInfo.Name}'  in model of type {modelType}, the destination map property with name '{modelPropertyAttribute.Destination}' is not found in entity of type {entityType}.");
                
                // add the two way mapping : model > entity and entity > model
                if (!PropertiesMappingDictionary.ContainsKey(extendedModelProp.PropertyInfo))
                    PropertiesMappingDictionary.TryAdd(extendedModelProp.PropertyInfo, entityProperty);
                else
                    PropertiesMappingDictionary[extendedModelProp.PropertyInfo] = entityProperty;

                if (!PropertiesMappingDictionary.ContainsKey(entityProperty))
                    PropertiesMappingDictionary.TryAdd(entityProperty, extendedModelProp.PropertyInfo);
                else
                    PropertiesMappingDictionary[entityProperty] = extendedModelProp.PropertyInfo;
            }
        }

        InitializedTypes.TryAdd(modelType, entityType);
    }

    /// <summary>Returns true if the property should skip name-convention mapping based on attribute or configuration settings.</summary>
    private bool ShouldIgnorePropertyNamingConventionMapping(MapAttribute modelMappingAttribute,  ExtendedPropertyInfo extendedModelProp)
    {
        if (extendedModelProp.GetAttribute<MapPropertyAttribute>() is MapPropertyAttribute propertyMapAttr && propertyMapAttr.IgnoreDefaultNamingMap)
            return true;

        if (modelMappingAttribute.IgnoreDefaultNamingMap)
            return true;

        if (MappingConfiguration is not null && MappingConfiguration.IgnoreDefaultNamingMap)
            return true;

        return false;

    }
    
    /// <summary>Determines which of the two types is the model (has [Map]) and which is the entity.</summary>
    private (Type ModelType, Type EntityType) GetModelAndEntityTypes(Type typeIn, Type typeOut) 
    {
        Type? modelType = null;
        Type? entityType = null;
        
        var classInInfo = Reflectify.GetTypeInfo(typeIn);
        if (classInInfo.GetAttribute<MapAttribute>() is not null)
        {
            modelType = typeIn;
            entityType = typeOut;
        }
        
        var classOutInfo = Reflectify.GetTypeInfo(typeOut);
        if (classOutInfo.GetAttribute<MapAttribute>() is not null)
        {
            modelType = typeOut;
            entityType = typeIn;
        }
        if(modelType is null || entityType is null)
            throw new Exception($"The types {typeIn}/{typeOut} are not mapped, no class declare [{nameof(MapAttribute)}]");
        
        return (modelType, entityType);
    }
    
    /// <summary>Iterates output properties and copies values from input, recursively handling nested and collection types.</summary>
    private async Task MapTypes<TOutput>(object input, TOutput output, Type modelType, DbContext? context) where TOutput : class
    {
        var typeMapAttribute = Reflectify.GetTypeInfo(modelType).GetAttribute<MapAttribute>();
        var outputProperties = Reflectify.GetProperties(typeof(TOutput));
        if (outputProperties is null || outputProperties.Count == 0)
            return;
        
        foreach (var prop in outputProperties)
        {
            var outPropertyInfo = prop.PropertyInfo;
            if(PropertiesMappingDictionary.TryGetValue(outPropertyInfo, out var inputPropertyInfo))
            {
                var value = inputPropertyInfo.GetValue(input);
                if (!Reflectify.IsCustomClassType(outPropertyInfo.PropertyType))
                {
                    
                    outPropertyInfo.SetValue(output, value);
                }
                else
                {
                    var mapMethod = typeof(Bridge).GetMethod(nameof(To), BindingFlags.NonPublic | BindingFlags.Instance);
                    if (mapMethod is null)
                        throw new Exception($"Unable to dynamically call {nameof(To)} method.");

                    if (value is null)
                        continue;

                    if (!Reflectify.IsCollectionType(prop.PropertyInfo.PropertyType))
                    {
                        object? entityInstance = null;
                        if (typeof(TOutput) != modelType && Reflectify.IsCustomClassType(outPropertyInfo.PropertyType))
                            entityInstance = outPropertyInfo.GetValue(output);

                        var genericMap = mapMethod.MakeGenericMethod(/*value.GetType(),*/ outPropertyInfo.PropertyType);
                        var task = (Task)genericMap.Invoke(this, [value, null, entityInstance])!;
                        await task;
                        var mappedValue = task.GetType().GetProperty("Result")!.GetValue(task);
                        outPropertyInfo.SetValue(output, mappedValue);
                    }
                    else
                    {
                        var list = (IList)value;
                        var result = new List<object>();
                        foreach (var item in list)
                        {
                            if (item is null)
                                continue;

                            object? entityInstance = null;
                            var elementType = GetCollectionElementType(outPropertyInfo.PropertyType);
                            if (typeof(TOutput) != modelType && MappingConfiguration is not null)
                            {
                                var entitiesList = (IList?)outPropertyInfo.GetValue(output);
                                var modelPrimaryKeyProperty = Reflectify.GetProperty(item.GetType(), MappingConfiguration.ModelPrimaryKeyPropertyName);
                                var modelPrimaryKey = modelPrimaryKeyProperty?.PropertyInfo.GetValue(item);
                                var entityPrimaryKeyProperty = Reflectify.GetProperty(elementType, MappingConfiguration.PrimaryKeyPropertyName);
                                entityInstance = entitiesList?.Cast<object>().FirstOrDefault(w=> entityPrimaryKeyProperty?.PropertyInfo.GetValue(w) is object obj && obj.Equals(modelPrimaryKey));
                            }

                            var genericMap = mapMethod.MakeGenericMethod(/*item.GetType(),*/ elementType);
                            var task = (Task)genericMap.Invoke(this, [item, null, entityInstance])!;
                            await task;
                            var mappedObject = task.GetType().GetProperty("Result")!.GetValue(task);
                            if (mappedObject is null)
                                continue;

                            result.Add(mappedObject);
                        }

                        SetCollectionProperty(outPropertyInfo, output, result);
                    }
                }
            }
        }
    }

    /// <summary>Assigns the mapped collection to the target property, converting to the correct collection type (array, List, IEnumerable).</summary>
    private static void SetCollectionProperty(PropertyInfo prop, object targetInstance, List<object> sourceList)
    {
        if (prop == null || targetInstance == null || sourceList == null) return;

        Type targetType = prop.PropertyType;

        // 1. Determine the item type inside the collection (e.g., MyClass)
        Type? elementType = null;
        if (targetType.IsArray)
        {
            elementType = targetType.GetElementType();
        }
        else if (targetType.IsGenericType)
        {
            elementType = targetType.GetGenericArguments()[0];
        }

        if (elementType == null)
        {
            throw new ArgumentException($"Property {prop.Name} is not a valid generic collection or array.");
        }

        // 2. Cast the elements dynamically to the correct type
        // This is equivalent to sourceList.Cast<MyClass>()
        var castMethod = typeof(Enumerable).GetMethod("Cast")!.MakeGenericMethod(elementType);
        var castedResult = castMethod.Invoke(null, new object[] { sourceList });

        // 3. Convert the casted IEnumerable to the exact target type
        object? finalCollection = null;

        if (targetType.IsArray)
        {
            // Convert to MyClass[]
            var toArrayMethod = typeof(Enumerable).GetMethod("ToArray")!.MakeGenericMethod(elementType);
            finalCollection = toArrayMethod.Invoke(null, new object[] { castedResult! });
        }
        else if (targetType.GetGenericTypeDefinition() == typeof(List<>))
        {
            // Convert to List<MyClass>
            var toListMethod = typeof(Enumerable).GetMethod("ToList")!.MakeGenericMethod(elementType);
            finalCollection = toListMethod.Invoke(null, new object[] { castedResult! });
        }
        else if (targetType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            // IEnumerable<MyClass> can just accept the casted result directly
            finalCollection = castedResult;
        }
        else
        {
            throw new NotSupportedException($"Collection type {targetType.Name} is not supported.");
        }

        // 4. Assign it to the property
        prop.SetValue(targetInstance, finalCollection);
    }
    
    /// <summary>Returns the element type of a collection (array or generic).</summary>
    private static Type GetCollectionElementType(Type collectionType)
    {
        if (collectionType.IsArray)
            return collectionType.GetElementType()!;
        return collectionType.GetGenericArguments()[0];
    }

    /// <summary>Returns the primary key property name for the entity type from the EF model metadata.</summary>
    private static string GetEntityPrimaryKeyName(Type type, DbContext context) 
    {
        var entityType = context.Model.FindEntityType(type);
        var primaryKey = entityType?.FindPrimaryKey();

        return primaryKey?.Properties.First().Name ?? throw new Exception($"Failed to find primary key for model of type '{type}'");
    }

    /// <summary>Retrieves an existing entity by primary key from the database, or creates a new instance if not found.</summary>
    private async Task<TEntity> GetOrCreateEntity<TEntity>(object model, DbContext? context)
        where TEntity : class 
    {
        var modelType = model.GetType();
        if (MappingConfiguration is not null)
        {
            var configured = await MappingConfiguration.GetOrCreateEntity<TEntity>(ServiceProvider, model);
            if (configured is not null)
                return configured;
        }

        if (context is null)
            return Activator.CreateInstance<TEntity>();

        var pkName = GetEntityPrimaryKeyName(typeof(TEntity), context);
        var modelPkName = MappingConfiguration?.PrimaryKeyPropertyName ?? pkName;
        var pkProp = Reflectify.GetProperty(modelType, modelPkName)?.PropertyInfo;
        var pkValue = pkProp?.GetValue(model);

        IQueryable<TEntity> query = context.Set<TEntity>();
        var includes = BuildIncludePaths(modelType, typeof(TEntity), new HashSet<Type>());
        foreach (var path in includes)
            query = query.Include(path);

        var entity = pkValue is not null
            ? await query.FirstOrDefaultAsync(e => EF.Property<object>(e, pkName) == pkValue)
            : null;

        return entity ?? Activator.CreateInstance<TEntity>();
    }

    /// <summary>Recursively builds EF Core .Include() paths for all navigable properties (nested and collection).</summary>
    private List<string> BuildIncludePaths(Type modelType, Type entityType, HashSet<Type> visited)
    {
        var paths = new List<string>();
        if (!visited.Add(modelType))
            return paths;

        var modelProps = Reflectify.GetProperties(modelType);
        if (modelProps is null)
            return paths;

        foreach (var prop in modelProps)
        {
            var attr = prop.GetAttribute<MapPropertyAttribute>();
            var entityPropName = attr?.Destination ?? prop.PropertyInfo.Name;

            var entityProp = Reflectify.GetProperty(entityType, entityPropName)?.PropertyInfo;
            if (entityProp is null)
                continue;

            var entityPropType = entityProp.PropertyType;
            var modelPropType = prop.PropertyInfo.PropertyType;

            if (!Reflectify.IsCustomClassType(entityPropType) && !Reflectify.IsCollectionType(entityPropType))
                continue;

            Type subEntityType;
            Type subModelType;

            if (Reflectify.IsCollectionType(entityPropType) && entityPropType.IsGenericType)
            {
                subEntityType = entityPropType.GetGenericArguments()[0];
                subModelType = modelPropType.IsGenericType
                    ? modelPropType.GetGenericArguments()[0]
                    : modelPropType;
            }
            else
            {
                subEntityType = entityPropType;
                subModelType = modelPropType;
            }

            if (!Reflectify.IsCustomClassType(subEntityType) || !Reflectify.IsCustomClassType(subModelType))
                continue;

            paths.Add(entityPropName);

            var subPaths = BuildIncludePaths(subModelType, subEntityType, visited);
            foreach (var sub in subPaths)
                paths.Add($"{entityPropName}.{sub}");
        }

        return paths;
    }
}