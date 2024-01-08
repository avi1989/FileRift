using System.Reflection;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions;
using Xunit;

namespace FileRift.IntegrationTests;

public class DelimitedFileDataReaderTests
{
    private readonly string _basePath;

    public DelimitedFileDataReaderTests()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        _basePath = Path.GetDirectoryName(assembly.GetAssemblyLocation())!;
    }
    
    [Fact]
    public void Should_Return_CorrectNumberOfRows()
    {
        var pathToFile = Path.Join(_basePath, "Files", "SmallerCsv.csv");
        var fileReader = FileRiftBuilder.BuildDelimitedReader(pathToFile)
            .HasHeaders()
            .WithDelimiter(',')
            .WithDateFormats("MM/dd/yyyy", "yyyy-MM-dd")
            .WithEscapeCharacter('\"')
            .WithTrimmedData()
            .BuildDataReader();

        int rowCount = 0;
        while (fileReader.Read())
        {
            rowCount++;
        }
        
        Assert.Equal(2, rowCount);
    }
}