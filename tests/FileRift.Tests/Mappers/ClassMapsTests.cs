using FileRift.Mappers;
using FileRift.Tests.Models;

namespace FileRift.Tests.Mappers;

public class ClassMapsTests
{
    public ClassMapsTests()
    {
        ClassMaps.RegisteredMappers.Clear();
    }

    [Fact]
    public void RegisterClassMap_Should_RegisterClassMapStatically()
    {
        var classMaps = new ClassMaps();
        classMaps.RegisterClassMap(new ClassMap<Test>());

        Assert.NotNull(ClassMaps.RegisteredMappers[typeof(Test).FullName!]);
    }

    [Fact]
    public void GetClassMap_Should_ReturnClassMap()
    {
        var classMaps = new ClassMaps();
        var mapToAdd = new ClassMap<Test>();
        classMaps.RegisterClassMap(mapToAdd);

        var result = classMaps.GetClassMap<Test>();
        Assert.Equal(mapToAdd, result);

    }
}