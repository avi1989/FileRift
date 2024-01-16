using System.Reflection;
using FileRift.IntegrationTests.Models;
using FileRift.Mappers;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions;
using Xunit;

namespace FileRift.IntegrationTests;

public class DelimitedFileReaderTests
{
    private readonly string _basePath;

    public DelimitedFileReaderTests()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        _basePath = Path.GetDirectoryName(assembly.GetAssemblyLocation())!;
    }

    [Fact]
    public void Read_Should_ReturnTypedData()
    {
        var pathToFile = Path.Join(_basePath, "Files", "CsvWithHeader.csv");
        var classMap = new ClassMap<Person>();
        classMap.AddColumnMap("FirstName", x => x.FirstName)
            .AddColumnMap("LastName", x => x.LastName)
            .AddColumnMap("Age", x => x.Age)
            .AddColumnMap("IsStudent", x => x.IsStudent)
            .AddColumnMap("Id", x => x.Id);

        var fileReader = FileRiftBuilder.Delimited(pathToFile)
            .HasHeaders()
            .WithDelimiter(',')
            .WithDateFormats("MM/dd/yyyy", "yyyy-MM-dd")
            .WithQuote('\"')
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
    public void Read_Should_ReturnTypedData_WithOrdinals()
    {
        var pathToFile = Path.Join(_basePath, "Files", "CsvWithoutHeader.csv");
        var classMap = new OrdinalClassMap<Person>();
        classMap.AddColumnMap(1, x => x.FirstName)
            .AddColumnMap(2, x => x.LastName)
            .AddColumnMap(3, x => x.Age)
            .AddColumnMap(4, x => x.IsStudent)
            .AddColumnMap(0, x => x.Id);

        var fileReader = FileRiftBuilder.Delimited(pathToFile)
            .WithDelimiter(',')
            .WithDateFormats("MM/dd/yyyy", "yyyy-MM-dd")
            .WithQuote('\"')
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
    public void Read_ShouldAutoConfigureAndHandleClassMap()
    {
        var pathToFile = Path.Join(_basePath, "Files", "CsvWithHeader.csv");
        var fileReader = FileRiftBuilder
            .Delimited(pathToFile)
            .AutoConfigure()
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

    [Fact]
    public void Read_Should_ReturnTypedDataWithRegisteredClassMap_AndNulls()
    {
        var classMap = new ClassMap<Person>();
        classMap
            .Add("FirstName", x => x.FirstName)
            .Add("LastName", x => x.LastName)
            .Add("Age", x => x.Age)
            .Add("IsStudent", x => x.IsStudent)
            .Add("Nulls", x => x.Nulls)
            .Add("Id", x => x.Id);

        var classMaps = new ClassMaps();
        classMaps.RegisterClassMap(classMap);

        var pathToFile = Path.Join(_basePath, "Files", "CsvWithHeader.csv");
        var fileReader = FileRiftBuilder.Delimited(pathToFile)
            .HasHeaders()
            .WithDelimiter(',')
            .WithQuote('\"')
            .Build<Person>();

        var results = fileReader.Read().ToList();
        Assert.Equal(2, results.Count);

        Assert.Null(results[0].Nulls);
    }
    
    [Fact]
    public void AutoConfiguredRead_Should_ReturnTypedData()
    {
        var pathToFile = Path.Join(_basePath, "Files", "CsvWithHeader.csv");
        var classMap = new ClassMap<Person>();
        classMap.AddColumnMap("FirstName", x => x.FirstName)
            .AddColumnMap("LastName", x => x.LastName)
            .AddColumnMap("Age", x => x.Age)
            .AddColumnMap("IsStudent", x => x.IsStudent)
            .AddColumnMap("Id", x => x.Id);

        var fileReader = FileRiftBuilder.Delimited(pathToFile)
            .HasHeaders()
            .WithDateFormats("MM/dd/yyyy", "yyyy-MM-dd")
            .AutoConfigure()
            .Build(classMap);
        // var fileReader = new DelimitedFileReader<Person>(pathToFile, true, ',', '\"', classMap, true);

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
    public void Read_Should_HandleNewLinesIfEnclosed_UnixFormat()
    {
        string fileName = "CsvWithNewlinesInRow.csv";
        var pathToFile = Path.Join(_basePath, "Files", fileName);
        var classMap = new ClassMap<Person>();
        classMap.AddColumnMap("FirstName", x => x.FirstName)
            .AddColumnMap("LastName", x => x.LastName)
            .AddColumnMap("Age", x => x.Age)
            .AddColumnMap("IsStudent", x => x.IsStudent)
            .AddColumnMap("Id", x => x.Id);

        var fileReader = FileRiftBuilder.Delimited(pathToFile)
            .HasHeaders()
            .WithDelimiter(',')
            .WithDateFormats("MM/dd/yyyy", "yyyy-MM-dd")
            .WithQuote('\"')
            .Build(classMap);
        // var fileReader = new DelimitedFileReader<Person>(pathToFile, true, ',', '\"', classMap, true);

        var results = fileReader.Read().ToList();
        Assert.Equal(2, results.Count);

        Assert.Equal(Guid.Parse("cd0cf662-9983-4152-8230-2a6f225ad985"), results[0].Id);
        Assert.Equal("John", results[0].FirstName);
        Assert.Equal("Doe", results[0].LastName);
        Assert.Equal(25, results[0].Age);
        Assert.True(results[0].IsStudent);
        
        Assert.Equal("Mary\nDoe", results[1].LastName);
    }
    
    [Fact]
    public void Read_Should_HandleNewLinesIfEnclosed_DosFormat()
    {
        string fileName = "CsvWithNewlinesInRowWindows.csv";
        var pathToFile = Path.Join(_basePath, "Files", fileName);
        var classMap = new ClassMap<Person>();
        classMap.AddColumnMap("FirstName", x => x.FirstName)
            .AddColumnMap("LastName", x => x.LastName)
            .AddColumnMap("Age", x => x.Age)
            .AddColumnMap("IsStudent", x => x.IsStudent)
            .AddColumnMap("Id", x => x.Id);

        var fileReader = FileRiftBuilder.Delimited(pathToFile)
            .HasHeaders()
            .WithDelimiter(',')
            .WithDateFormats("MM/dd/yyyy", "yyyy-MM-dd")
            .WithQuote('\"')
            .Build(classMap);
        // var fileReader = new DelimitedFileReader<Person>(pathToFile, true, ',', '\"', classMap, true);

        var results = fileReader.Read().ToList();
        Assert.Equal(2, results.Count);

        Assert.Equal(Guid.Parse("cd0cf662-9983-4152-8230-2a6f225ad985"), results[0].Id);
        Assert.Equal("John", results[0].FirstName);
        Assert.Equal("Doe", results[0].LastName);
        Assert.Equal(25, results[0].Age);
        Assert.True(results[0].IsStudent);
        
        Assert.Equal("Mary\r\nDoe", results[1].LastName);
    }
}