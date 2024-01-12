using FileRift.Delimited;
using FileRift.FixedWidth;

namespace FileRift;

public static class FileRiftBuilder
{
    public static DelimitedFileBuilder Csv(string filePath)
    {
        return new DelimitedFileBuilder(filePath, ',', '"');
    }
    
    public static DelimitedFileBuilder Psv(string filePath)
    {
        return new DelimitedFileBuilder(filePath, '|', '"');
    }
    
    public static DelimitedFileBuilder Tsv(string filePath)
    {
        return new DelimitedFileBuilder(filePath, '\t', null);
    }

    public static DelimitedFileBuilder Delimited(string filePath)
    {
        return new DelimitedFileBuilder(filePath);
    }

    public static FixedWidthFileReaderBuilder FixedWidth(string filePath)
    {
        return new FixedWidthFileReaderBuilder(filePath);
    }
}