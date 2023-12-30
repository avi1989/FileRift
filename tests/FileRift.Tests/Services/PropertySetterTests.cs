using FileRift.Services;
using FileRift.Tests.Models;

namespace FileRift.Tests.Services;

public class PropertySetterTests
{
    [Fact]
    public void Should_SetProperty()
    {
        var test = new Test();
        var propertySetter = new PropertySetter<Test>();
        propertySetter.SetValue(test, "FirstName", "John");
        propertySetter.SetValue(test, "Age", 5);

        Assert.Equal("John", test.FirstName);
        Assert.Equal(5, test.Age);
    }
}