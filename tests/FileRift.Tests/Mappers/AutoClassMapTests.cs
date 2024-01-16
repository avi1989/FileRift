using FileRift.Mappers;
using FileRift.Tests.Models;

namespace FileRift.Tests.Mappers;

public class AutoClassMapTests
{
    [Fact]
    public void GetColumnMapping_Should_ReturnDataFromRegisteredMapping_IfExists()
    {
        var classMap = new AutoClassMap<Test>(false, false);
        classMap.AddColumnMap("FN", x => x.FirstName);

        var propName = classMap.GetColumnMapping("FN");
        Assert.Equal("FN", propName.ColumnName);;
        Assert.Equal("FirstName", propName.PropertyName);;
    }
    
    [Fact]
    public void GetColumnMapping_Should_ReturnDataFromRegisteredMapping_IfExistsIgnoringCasing()
    {
        var classMap = new AutoClassMap<Test>(true, false);
        classMap.AddColumnMap("FN", x => x.FirstName);

        var propName = classMap.GetColumnMapping("fn");
        Assert.Equal("FN", propName.ColumnName);;
        Assert.Equal("FirstName", propName.PropertyName);;
    }
    
    [Fact]
    public void GetColumnMapping_Should_ReturnAutoMapping_IfExactlyEqual()
    {
        var classMap = new AutoClassMap<Test>(false, false);

        var propName = classMap.GetColumnMapping("FirstName");
        Assert.Equal("FirstName", propName.ColumnName);;
        Assert.Equal("FirstName", propName.PropertyName);;
    }
    
    [Fact]
    public void GetColumnMapping_Should_ReturnAutoMapping_IgnoringCase()
    {
        var classMap = new AutoClassMap<Test>(true, false);

        var propName = classMap.GetColumnMapping("firstname");
        Assert.Equal("firstname", propName.ColumnName);;
        Assert.Equal("FirstName", propName.PropertyName);;
    }
    
    [Fact]
    public void GetColumnMapping_Should_ReturnAutoMapping_IgnoringUnderscore()
    {
        var classMap = new AutoClassMap<Test>(true, true);

        var propName = classMap.GetColumnMapping("First_Name");
        Assert.Equal("First_Name", propName.ColumnName);;
        Assert.Equal("FirstName", propName.PropertyName);;
    }
    
    [Fact]
    public void GetColumnMapping_Should_ReturnAutoMapping_IgnoringCaseAndUnderscore()
    {
        var classMap = new AutoClassMap<Test>(true, true);

        var propName = classMap.GetColumnMapping("FIRST_NAME");
        Assert.Equal("FIRST_NAME", propName.ColumnName);;
        Assert.Equal("FirstName", propName.PropertyName);;
    }
    
    [Fact]
    public void GetColumnMapping_Should_ReturnAutoMapping_IgnoringUnderscoreOnPropertyName()
    {
        var classMap = new AutoClassMap<Test>(true, true);

        var propName = classMap.GetColumnMapping("TestUnderscore");
        Assert.Equal("TestUnderscore", propName.ColumnName);;
        Assert.Equal("Test_Underscore", propName.PropertyName);;
    }
    
    [Fact]
    public void GetColumnMapping_Should_ReturnAutoMapping_IgnoringHyphen()
    {
        var classMap = new AutoClassMap<Test>(true, true);

        var propName = classMap.GetColumnMapping("First-Name");
        Assert.Equal("First-Name", propName.ColumnName);;
        Assert.Equal("FirstName", propName.PropertyName);;
    }
    
    [Fact]
    public void GetColumnMapping_Should_ReturnAttributeMapping_OverAutomaticMapping()
    {
        var classMap = new AutoClassMap<TestWithAttribute>(true, true);

        var propName = classMap.GetColumnMapping("F_Name");
        Assert.Equal("F_Name", propName.ColumnName);;
        Assert.Equal("FirstName", propName.PropertyName);;
    }
    
    [Theory]
    [InlineData("First Name")]
    [InlineData("First   Name")]
    public void GetColumnMapping_Should_ReturnAutoMapping_IgnoringSpaces(string columnName)
    {
        var classMap = new AutoClassMap<Test>(true, true);

        var propName = classMap.GetColumnMapping(columnName);
        Assert.Equal(columnName, propName.ColumnName);;
        Assert.Equal("FirstName", propName.PropertyName);;
    }
}