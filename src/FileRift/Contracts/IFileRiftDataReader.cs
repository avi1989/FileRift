using System.Collections.ObjectModel;
using System.Data;

namespace FileRift.Contracts;

public interface IFileRiftDataReader : IDataReader
{
    string[] AllowedDateFormats { get; }

    IReadOnlyCollection<string?>? Headers { get; }

    string?[] CurrentRow { get; }
    
    string CurrentRowRaw { get; }
    
    int CurrentRowNumber { get; }
}