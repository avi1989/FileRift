using System.Reflection;
using FileRift.FixedWidth;
using FileRift.IntegrationTests.Models;
using FileRift.Mappers;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions;
using Xunit;

namespace FileRift.IntegrationTests;

public class FixedWidthFileReaderTests
{
    private readonly string _basePath;

    public FixedWidthFileReaderTests()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        _basePath = Path.GetDirectoryName(assembly.GetAssemblyLocation())!;
    }

    [Fact]
    public void Read_Should_ReturnTypedData()
    {
        var pathToFile = Path.Join(_basePath, "Files", "FixedWidthFile.txt");
        var classMap = new ClassMap<Person>();
        classMap
            .AddColumnMap("Id", x => x.Id)
            .AddColumnMap("FirstName", x => x.FirstName)
            .AddColumnMap("LastName", x => x.LastName)
            .AddColumnMap("Age", x => x.Age)
            .AddColumnMap("IsStudent", x => x.IsStudent);

        // var fileReader = new DelimitedFileReader<Person>(pathToFile, true, ',', '\"', classMap, true);
        var columns = new List<FixedWidthColumnInfo>()
        {
            new(1, 36, "Id"),
            new(2, 20, "FirstName"),
            new(3, 20, "LastName"),
            new(4, 2, "Age"),
            new(5, 5, "IsStudent"),
        };

        // var fileReader = new FixedWidthFileReader<Person>(pathToFile, columns, classMap);
        var fileReader = FileReaderBuilder.BuildFixedWidthReader(pathToFile)
            .WithColumns(columns)
            .Build(classMap);

        var results = fileReader.Read().ToList();
        Assert.Equal(2, results.Count);
        Assert.Equal(Guid.Parse("cd0cf662-9983-4152-8230-2a6f225ad985"), results[0].Id);
        Assert.Equal("John", results[0].FirstName);
        Assert.Equal("Doe", results[0].LastName);
        Assert.Equal(25, results[0].Age);
        Assert.True(results[0].IsStudent);

        Assert.Equal(Guid.Parse("b3436c0e-7eb4-4620-b9eb-7890c3462fbe"), results[1].Id);
        Assert.Equal("Jane", results[1].FirstName);
        Assert.Equal("Mary Doe", results[1].LastName);
        Assert.Equal(22, results[1].Age);
        Assert.False(results[1].IsStudent);
    }

    [Fact]
    public void Read_Should_ReturnTypedData_WithRegisteredClassMap()
    {
        var pathToFile = Path.Join(_basePath, "Files", "FixedWidthFile.txt");
        var columns = new List<FixedWidthColumnInfo>()
        {
            new(1, 36, "Id"),
            new(2, 20, "FirstName"),
            new(3, 20, "LastName"),
            new(4, 2, "Age"),
            new(5, 5, "IsStudent"),
        };

        var classMap = new ClassMap<Person>();
        classMap.AddColumnMap("FirstName", x => x.FirstName)
            .AddColumnMap("LastName", x => x.LastName)
            .AddColumnMap("Age", x => x.Age)
            .AddColumnMap("IsStudent", x => x.IsStudent)
            .AddColumnMap("Id", x => x.Id);

        new ClassMaps().RegisterClassMap(classMap);

        // var fileReader = new FixedWidthFileReader<Person>(pathToFile, columns, classMap);
        var fileReader = FileReaderBuilder.BuildFixedWidthReader(pathToFile)
            .WithColumns(columns)
            .Build<Person>();

        var results = fileReader.Read().ToList();
        Assert.Equal(2, results.Count);
        Assert.Equal(Guid.Parse("cd0cf662-9983-4152-8230-2a6f225ad985"), results[0].Id);
        Assert.Equal("John", results[0].FirstName);
        Assert.Equal("Doe", results[0].LastName);
        Assert.Equal(25, results[0].Age);
        Assert.True(results[0].IsStudent);

        Assert.Equal(Guid.Parse("b3436c0e-7eb4-4620-b9eb-7890c3462fbe"), results[1].Id);
        Assert.Equal("Jane", results[1].FirstName);
        Assert.Equal("Mary Doe", results[1].LastName);
        Assert.Equal(22, results[1].Age);
        Assert.False(results[1].IsStudent);
    }
}