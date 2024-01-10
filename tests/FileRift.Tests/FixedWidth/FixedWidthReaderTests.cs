using System.Data;
using FileRift.Contracts;
using FileRift.FixedWidth;
using FileRift.Mappers;
using FileRift.Tests.Models;
using NSubstitute;

namespace FileRift.Tests.FixedWidth;

public class FixedWidthReaderTests
{
    [Fact]
    public void Read_Should_ReturnMappedObject()
    {
        var classMap = new ClassMap<Test>();
        classMap.AddColumnMap("First Name", x => x.FirstName)
            .AddColumnMap("LName", x => x.LastName)
            .AddColumnMap("Age", x => x.Age);

        var dataReader = Substitute.For<IFileRiftDataReader>();
        dataReader.Read().Returns(true, true, false);
        dataReader.GetOrdinal("First Name").Returns(0);
        dataReader.GetOrdinal("LName").Returns(1);
        dataReader.GetOrdinal("Age").Returns(2);

        dataReader.GetString(0).Returns("John", "Jim");
        dataReader.GetString(1).Returns("Doe", "Seiger");
        dataReader.GetString(2).Returns("12", "14");

        dataReader.Headers.Returns(new List<string?>() { "First Name", "LName", "Age" });

        var delimitedFileReader = new FixedWidthFileReader<Test>(dataReader, classMap);

        var result = delimitedFileReader.Read().ToList();
        Assert.Equal(2, result.Count);

        Assert.Equal("John", result[0].FirstName);
        Assert.Equal("Doe", result[0].LastName);
        Assert.Equal(12, result[0].Age);

        Assert.Equal("Jim", result[1].FirstName);
        Assert.Equal("Seiger", result[1].LastName);
        Assert.Equal(14, result[1].Age);
    }
}