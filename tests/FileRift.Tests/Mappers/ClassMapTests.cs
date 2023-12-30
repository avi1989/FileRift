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
        Assert.Single(classMap.ColumnMappings);
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
}