using System.Data;
using System.Diagnostics;
using System.Globalization;

namespace FileRift.Contracts;

public abstract class FileRiftDataReader : IDataReader
{
    protected FileRiftDataReader(
        StreamReader streamReader,
        bool hasHeaders,
        IEnumerable<string>? allowedDateFormats,
        IRowSplitter rowSplitter)
    {
        RowSplitter = rowSplitter;
        StreamReader = streamReader;
        CurrentRow = new string[1];
        AllowedDateFormats = allowedDateFormats?.ToArray() ?? new string[] { };

        if (hasHeaders)
        {
            var firstLine = StreamReader.ReadLine();
            Debug.Assert(firstLine != null);

            Headers = RowSplitter.SplitRow(firstLine).ToList();
            FieldCount = Headers.Count;
        }
    }

    protected FileRiftDataReader(
        StreamReader streamReader,
        IEnumerable<string>? allowedDateFormats)
    {
        StreamReader = streamReader;
        CurrentRow = new string[1];
        AllowedDateFormats = allowedDateFormats?.ToArray() ?? new string[] { };
    }

    protected StreamReader StreamReader { get; }


    protected string[] AllowedDateFormats { get; }

    protected List<string>? Headers { get; set; }

    protected string[] CurrentRow { get; private set; }

    protected IRowSplitter RowSplitter { get; set; }

    protected bool HasHeader => this.Headers?.Any() ?? false;

    public int Depth { get; } = 1;

    public bool IsClosed { get; private set; }

    public int RecordsAffected { get; } = -1;

    public int FieldCount { get; private set; } = -1;

    public object this[int i] => GetValue(i);

    public object this[string name]
    {
        get
        {
            if (!HasHeader)
            {
                throw new InvalidOperationException("Cannot get data by name without headers");
            }

            var ordinal = this.GetOrdinal(name);
            if (ordinal == -1)
            {
                throw new ArgumentException($"Header {name} not found");
            }

            return this.GetValue(ordinal);
        }
    }

    public bool Read()
    {
        if (StreamReader.EndOfStream)
        {
            this.Close();
            return false;
        }

        var rawRow = StreamReader.ReadLine()!;

        CurrentRow = RowSplitter.SplitRow(rawRow);

        if (FieldCount == -1)
        {
            FieldCount = CurrentRow.Length;
        }

        return true;
    }

    public bool NextResult()
    {
        return false;
    }

    public bool GetBoolean(int i)
    {
        return bool.Parse(CurrentRow[i]);
    }

    public byte GetByte(int i)
    {
        return byte.Parse(CurrentRow[i]);
    }

    public long GetBytes(int i, long fieldOffset, byte[]? buffer, int bufferoffset, int length)
    {
        throw new NotImplementedException();
    }

    public char GetChar(int i)
    {
        var val = GetString(i);
        if (string.IsNullOrEmpty(val))
        {
            return '\0';
        }

        var charArray = val.ToCharArray();
        if (charArray.Length > 1)
        {
            throw new InvalidOperationException("A char cannot have more than 1 character");
        }

        return charArray[0];
    }

    public long GetChars(int columnIndex, long fieldoffset, char[]? buffer, int bufferoffset, int length)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        var value = this.GetString(columnIndex).ToCharArray();

        long charRead = 0;

        for (var i = 0; (i < length) && ((i + fieldoffset) < value.Length) && (i + bufferoffset) < buffer.Length; i++)
        {
            buffer[bufferoffset + i] = value[fieldoffset + i];
            charRead++;
        }

        return charRead;
    }

    public IDataReader GetData(int i)
    {
        return this;
    }

    public string GetDataTypeName(int i)
    {
        throw new NotImplementedException();
    }

    public DateTime GetDateTime(int i)
    {
        var dateAsString = this.GetString(i);

        if (AllowedDateFormats.Any())
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            return DateTime.ParseExact(dateAsString, AllowedDateFormats, provider);
        }
        return DateTime.Parse(dateAsString);
    }

    public decimal GetDecimal(int i)
    {
        return decimal.Parse(CurrentRow[i]);
    }

    public double GetDouble(int i)
    {
        return double.Parse(CurrentRow[i]);
        throw new NotImplementedException();
    }

    public Type GetFieldType(int i)
    {
        throw new NotImplementedException();
    }

    public float GetFloat(int i)
    {
        return float.Parse(CurrentRow[i]);
    }

    public Guid GetGuid(int i)
    {
        return Guid.Parse(CurrentRow[i]);
    }

    public short GetInt16(int i)
    {
        return short.Parse(CurrentRow[i]);
    }

    public int GetInt32(int i)
    {
        return int.Parse(CurrentRow[i]);
    }

    public long GetInt64(int i)
    {
        return long.Parse(CurrentRow[i]);
    }

    public string GetName(int i)
    {
        if (!HasHeader)
        {
            throw new InvalidOperationException("Cannot get column name without headers");
        }

        return Headers![i];
    }

    public int GetOrdinal(string name)
    {
        if (!HasHeader)
        {
            throw new InvalidOperationException("Cannot get column name without headers");
        }

        return Headers!.IndexOf(name);
    }

    public string GetString(int i)
    {
        return GetValue(i)?.ToString()!;
    }

    public object GetValue(int i)
    {
        return CurrentRow[i];
    }

    public int GetValues(object[] values)
    {
        int i;
        for (i = 0; i < CurrentRow.Length; i++)
        {
            values[i] = CurrentRow[i];
        }

        return i + 1;
    }

    public bool IsDBNull(int i)
    {
        var value = this.GetValue(i);

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        return value == null;
    }


    public void Dispose()
    {
        StreamReader.Dispose();
    }

    public void Close()
    {
        this.IsClosed = true;
    }

    public DataTable GetSchemaTable()
    {
        if (!HasHeader)
        {
            throw new InvalidOperationException("Cannot get schema table without headers");
        }

        var schemaTable = new DataTable();
        foreach (var header in Headers!)
        {
            schemaTable.Columns.Add(header);
        }

        return schemaTable;
    }
}