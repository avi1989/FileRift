using FileRift.Mappers;
using FileRift.Tests.Models;

namespace FileRift.Tests.Mappers;

public class ClassMapTests
{
    [Fact]
    public void AddColumn_Should_AddColumnMap()
    {
        var classMap = new ClassMap<Test>();
        classMap.AddColumnMap("Test", c => c.FirstName);
        Assert.Single(classMap.SavedColumnMappings);
    }

    [Fact]
    public void AddColumn_ShouldNotAllowNestedColumnMap()
    {
        var classMap = new ClassMap<Test>();
        Assert.Throws<ArgumentException>(() =>
        {
            classMap.AddColumnMap("Test", c => c.Test2.Age);
        });
    }

    [Fact]
    public void Type_ShouldContain_TypeName()
    {
        var classMap = new ClassMap<Test>();
        Assert.Equal(typeof(Test).FullName, classMap.Type.FullName);
    }

    [Fact]
    public void GetColumnMapping_Should_ReturnAddedColumnMap()
    {
        var classMap = new ClassMap<Test>();
        classMap.AddColumnMap("Test", c => c.FirstName);
        var addedClassMap = classMap.GetColumnMapping("Test");
        Assert.Equal("Test", addedClassMap!.ColumnName);
        Assert.Equal("FirstName", addedClassMap.PropertyName);
    }

    [Fact]
    public void Constructor_Should_AddProperties()
    {
        var classMap = new ClassMap<Test>(); 
        Assert.Equal(5, classMap.Properties.Count);
    }
    
    [Fact]
    public void Constructor_Should_AddMappingsThatHaveAttributes()
    {
        var classMap = new ClassMap<TestWithAttribute>();
        Assert.Single(classMap.SavedColumnMappings);
    }
    
    [Fact]
    public void AddColumnMap_Should_OverwritePreviousMappings_ByPropertyName()
    {
        var classMap = new ClassMap<Test>();
        classMap.AddColumnMap("FName", x => x.FirstName);
        classMap.AddColumnMap("FirstName", x => x.FirstName);
        var columnMap = classMap.GetColumnMapping("FirstName");
        Assert.Equal("FirstName", columnMap!.ColumnName);
        Assert.Single(classMap.SavedColumnMappings);
    }
    
    [Fact]
    public void AddColumnMap_Should_OverwritePreviousMappings_ByColumnName()
    {
        var classMap = new ClassMap<Test>();
        classMap.AddColumnMap("Name", x => x.FirstName);
        classMap.AddColumnMap("Name", x => x.LastName);
        var columnMap = classMap.GetColumnMapping("Name");
        Assert.Equal("Name", columnMap!.ColumnName);
        Assert.Equal("LastName", columnMap!.PropertyName);
        Assert.Single(classMap.SavedColumnMappings);
    }
}