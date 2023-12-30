using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FileRift.Tests")]

namespace FileRift.Contracts;

public interface IRowSplitter
{
    string[] SplitRow(string row);
}