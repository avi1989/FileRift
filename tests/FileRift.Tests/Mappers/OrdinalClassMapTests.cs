using FileRift.Mappers;
using FileRift.Tests.Models;

namespace FileRift.Tests.Mappers;

public class OrdinalClassMapTests
{
    [Fact]
    public void Properties_Should_Be_Populated()
    {
        var map = new OrdinalClassMap<Test>();

        Assert.Equal(5, map.Properties.Count);
    }

    [Fact]
    public void Type_Should_BeSetCorrectly()
    {
        var map = new OrdinalClassMap<Test>();

        Assert.Equal(typeof(Test), map.Type);
    }

    [Fact]
    public void AddColumnMap_ShouldAddColumnMapping()
    {
        var map = new OrdinalClassMap<Test>();
        map.AddColumnMap(0, x => x.FirstName);
        
        Assert.Single(map.ColumnMappings);
        Assert.Equal(0, map.ColumnMappings[0].ColumnIndex);
        Assert.Equal("FirstName", map.ColumnMappings[0].PropertyName);
    }
}