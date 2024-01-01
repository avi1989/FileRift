﻿using System.Reflection;
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

        var fileReader = FileReaderBuilder.BuildDelimitedReader(pathToFile)
            .HasHeaders()
            .WithDelimiter(',')
            .WithDateFormats("MM/dd/yyyy", "yyyy-MM-dd")
            .WithEscapeCharacter('\"')
            .WithTrimmedData()
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
    public void Read_Should_ReturnTypedDataWithRegisteredClassMap()
    {
        var classMap = new ClassMap<Person>();
        classMap
            .Add("FirstName", x => x.FirstName)
            .Add("LastName", x => x.LastName)
            .Add("Age", x => x.Age)
            .Add("IsStudent", x => x.IsStudent)
            .Add("Id", x => x.Id);

        var classMaps = new ClassMaps();
        classMaps.RegisterClassMap(classMap);

        var pathToFile = Path.Join(_basePath, "Files", "CsvWithHeader.csv");
        var fileReader = FileReaderBuilder.BuildDelimitedReader(pathToFile)
            .HasHeaders()
            .WithDelimiter(',')
            .WithEscapeCharacter('\"')
            .WithTrimmedData()
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