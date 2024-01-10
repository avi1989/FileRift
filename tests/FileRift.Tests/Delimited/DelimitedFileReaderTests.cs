using System.Data;
using FileRift.Contracts;
using FileRift.Delimited;
using FileRift.Mappers;
using FileRift.Tests.Models;
using NSubstitute;

namespace FileRift.Tests.Delimited;

public class DelimitedFileReaderTests
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


        var delimitedFileReader = new DelimitedFileReader<Test>(dataReader, classMap);

        var result = delimitedFileReader.Read().ToList();
        Assert.Equal(2, result.Count);

        Assert.Equal("John", result[0].FirstName);
        Assert.Equal("Doe", result[0].LastName);
        Assert.Equal(12, result[0].Age);

        Assert.Equal("Jim", result[1].FirstName);
        Assert.Equal("Seiger", result[1].LastName);
        Assert.Equal(14, result[1].Age);
    }

    [Fact]
    public void Read_Should_ThrowAnExceptionWithLineNumber_IfReadingOrdinalFailed()
    {
        var classMap = new ClassMap<Test>();
        classMap.AddColumnMap("First Name", x => x.FirstName)
            .AddColumnMap("LName", x => x.LastName)
            .AddColumnMap("Age", x => x.Age);

        var dataReader = Substitute.For<IFileRiftDataReader>();
        dataReader.Read().Returns(true, true, false);
        dataReader.GetOrdinal("First Name").Returns(0);
        dataReader.GetOrdinal("LName").Returns(x => 1, x => throw new Exception());
        dataReader.GetOrdinal("Age").Returns(2);

        dataReader.GetString(0).Returns("John", "Jim");
        dataReader.GetString(1).Returns("Doe", "Dow");
        dataReader.GetInt32(2).Returns(12, 14);
        dataReader.CurrentRowNumber.Returns(2);
        
        dataReader.Headers.Returns(new List<string?>() { "First Name", "LName", "Age" });


        var delimitedFileReader = new DelimitedFileReader<Test>(dataReader, classMap);

        RowReadException exception = Assert.Throws<RowReadException>(() =>
        {
            delimitedFileReader.Read().ToList();
        });

        Assert.Equal(2, exception.RowNumber);
    }

    [Fact]
    public void Read_Should_ThrowAnExceptionWithLineNumber_IfReadingDataFailed_ByDefault()
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

        dataReader.GetString(0).Returns(x => "John", x => throw new Exception());
        dataReader.GetString(1).Returns("Doe", "Dow");
        dataReader.GetInt32(2).Returns(12, 14);
        dataReader.CurrentRowNumber.Returns(2);
        
        dataReader.Headers.Returns(new List<string?>() { "First Name", "LName", "Age" });


        var delimitedFileReader = new DelimitedFileReader<Test>(dataReader, classMap);

        RowReadException exception = Assert.Throws<RowReadException>(() =>
        {
            delimitedFileReader.Read().ToList();
        });

        Assert.Equal(2, exception.RowNumber);
    }

    [Fact]
    public void Read_Should_NotThrowAnExceptionWithLineNumber_IfReadingDataFailed_IfRequested()
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

        dataReader.GetString(0).Returns(x => "John", x => throw new Exception());
        dataReader.GetString(1).Returns("Doe", "Dow");
        dataReader.GetString(2).Returns("12", "14");
        dataReader.CurrentRowNumber.Returns(2);
        
        dataReader.Headers.Returns(new List<string?>() { "First Name", "LName", "Age" });

        var delimitedFileReader =
            new DelimitedFileReader<Test>(dataReader, classMap, shouldIgnoreErrors: true);

        delimitedFileReader.Read().ToList();
        Assert.Equal(1, delimitedFileReader.Errors.Count);
        Assert.Equal(2, delimitedFileReader.Errors.First().RowNumber);
    }

    [Fact]
    public void Read_Should_BeHandleNullableValues()
    {
        var classMap = new ClassMap<TestWithNullableValues>();
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
        dataReader.GetString(2).Returns("12", "");
        
        dataReader.Headers.Returns(new List<string?>() { "First Name", "LName", "Age" });


        var delimitedFileReader =
            new DelimitedFileReader<TestWithNullableValues>(dataReader, classMap);

        var result = delimitedFileReader.Read().ToList();
        Assert.Equal(2, result.Count);

        Assert.Equal("John", result[0].FirstName);
        Assert.Equal("Doe", result[0].LastName);
        Assert.Equal(12, result[0].Age);

        Assert.Equal("Jim", result[1].FirstName);
        Assert.Equal("Seiger", result[1].LastName);
        Assert.Null(result[1].Age);
    }
    
    [Fact]
    public void Read_Should_BeHandleNullableDates()
    {
        var classMap = new ClassMap<TestWithNullableValues>();
        classMap.AddColumnMap("First Name", x => x.FirstName)
            .AddColumnMap("LName", x => x.LastName)
            .AddColumnMap("RegDate", x => x.RegistrationDate)
            .AddColumnMap("Age", x => x.Age);

        var dataReader = Substitute.For<IFileRiftDataReader>();
        dataReader.Read().Returns(true, true, false);
        dataReader.GetOrdinal("First Name").Returns(0);
        dataReader.GetOrdinal("LName").Returns(1);
        dataReader.GetOrdinal("Age").Returns(2);
        dataReader.GetOrdinal("RegDate").Returns(3);

        dataReader.Headers.Returns(new List<string?>() { "First Name", "LName", "Age", "RegDate" });

        dataReader.GetString(0).Returns("John", "Jim");
        dataReader.GetString(1).Returns("Doe", "Seiger");
        dataReader.GetString(2).Returns("12", "");
        dataReader.GetString(3).Returns("2020-01-01", "");

        var delimitedFileReader =
            new DelimitedFileReader<TestWithNullableValues>(dataReader, classMap);

        var result = delimitedFileReader.Read().ToList();
        Assert.Equal(2, result.Count);

        Assert.Equal(new DateTime(2020, 1, 1), result[0].RegistrationDate);
        Assert.Null(result[1].RegistrationDate);
    }

    [Fact]
    public void Read_Should_SetDefaultRatherThanNullIfValueIsNullForNonNullableData()
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
        dataReader.GetString(2).Returns("12", "");
        
        dataReader.Headers.Returns(new List<string?>() { "First Name", "LName", "Age" });


        var delimitedFileReader = new DelimitedFileReader<Test>(dataReader, classMap);

        var result = delimitedFileReader.Read().ToList();
        Assert.Equal(2, result.Count);

        Assert.Equal("John", result[0].FirstName);
        Assert.Equal("Doe", result[0].LastName);
        Assert.Equal(12, result[0].Age);

        Assert.Equal("Jim", result[1].FirstName);
        Assert.Equal("Seiger", result[1].LastName);
        Assert.Equal(0, result[1].Age);
    }
    
    [Fact]
    public void Read_Should_BeHandleNullableStrings()
    {
        var classMap = new ClassMap<TestWithNullableValues>();
        classMap.AddColumnMap("First Name", x => x.FirstName)
            .AddColumnMap("LName", x => x.LastName)
            .AddColumnMap("RegDate", x => x.RegistrationDate)
            .AddColumnMap("Age", x => x.Age);

        var dataReader = Substitute.For<IFileRiftDataReader>();
        dataReader.Read().Returns(true, false);
        dataReader.GetOrdinal("First Name").Returns(0);
        dataReader.GetOrdinal("LName").Returns(1);
        dataReader.GetOrdinal("Age").Returns(2);
        dataReader.GetOrdinal("RegDate").Returns(3);

        dataReader.GetString(0).Returns("John");
        dataReader.GetString(1).Returns(null as string);
        dataReader.GetString(2).Returns("12");
        dataReader.GetString(3).Returns("2020-01-01");

        var delimitedFileReader =
            new DelimitedFileReader<TestWithNullableValues>(dataReader, classMap);

        var result = delimitedFileReader.Read().ToList();
        Assert.Single(result);
        Assert.Null(result[0].LastName);

    }
}