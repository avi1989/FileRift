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
        dataReader.GetInt32(2).Returns(12, 14);

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
        dataReader.GetInt32(2).Returns(12, 14);
        dataReader.CurrentRowNumber.Returns(2);

        var delimitedFileReader = new DelimitedFileReader<Test>(dataReader, classMap, shouldIgnoreErrors: true);

        delimitedFileReader.Read().ToList();
        Assert.Equal(1, delimitedFileReader.Errors.Count);
        Assert.Equal(2, delimitedFileReader.Errors.First().RowNumber);
        
    }
}