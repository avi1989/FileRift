using FileRift.Delimited;
using FileRift.FixedWidth;

namespace FileRift;

public static class FileReaderBuilder
{
    public static DelimitedFileReaderBuilder BuildDelimitedReader(string pathToFile)
    {
        return new DelimitedFileReaderBuilder(pathToFile);
    }

    public static FixedWidthFileReaderBuilder BuildFixedWidthReader(string pathToFile)
    {
        return new FixedWidthFileReaderBuilder(pathToFile);
    }
}