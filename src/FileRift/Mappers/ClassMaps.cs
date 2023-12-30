using FileRift.Contracts;

namespace FileRift.Mappers;

public class ClassMaps
{
    static ClassMaps()
    {
        RegisteredMappers = new();
    }
    public void RegisterClassMap(IClassMap map)
    {
        RegisteredMappers.TryAdd(map.Type.FullName!, map);
    }

    public IClassMap? GetClassMap(string fullName)
    {
        RegisteredMappers.TryGetValue(fullName, out var classMap);
        return classMap;
    }

    public ClassMap<T>? GetClassMap<T>()
    {
        var fullName = typeof(T).FullName!;
        RegisteredMappers.TryGetValue(fullName, out var classMap);
        return classMap as ClassMap<T>;
    }

    internal static readonly Dictionary<string, IClassMap> RegisteredMappers;
}