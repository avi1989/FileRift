﻿using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;

namespace FileRift.Contracts;

public abstract class FileRiftDataReader : IFileRiftDataReader
{
    private List<string>? _headers;
    private readonly char? _enclosingCharacter;

    protected FileRiftDataReader(
        StreamReader streamReader,
        bool hasHeaders,
        IEnumerable<string>? allowedDateFormats,
        IRowSplitter rowSplitter,
        char? enclosingCharacter)
    {
        RowSplitter = rowSplitter;
        StreamReader = streamReader;
        CurrentRow = new string[1];
        AllowedDateFormats = allowedDateFormats?.ToArray() ?? [];
        _enclosingCharacter = enclosingCharacter;

        if (hasHeaders)
        {
            var firstLine = StreamReader.ReadLine();
            Debug.Assert(firstLine != null);

            _headers = RowSplitter.SplitRow(firstLine).ToList();
            FieldCount = _headers.Count;
        }
    }

    // protected FileRiftDataReader(
    //     StreamReader streamReader,
    //     IEnumerable<string>? allowedDateFormats)
    // {
    //     StreamReader = streamReader;
    //     CurrentRow = new string[1];
    //     AllowedDateFormats = allowedDateFormats?.ToArray() ?? [];
    // }

    public int CurrentRowNumber { get; private set; }

    protected StreamReader StreamReader { get; }

    public string[] AllowedDateFormats { get; }

    public IReadOnlyCollection<string>? Headers
    {
        get { return _headers; }
        protected set { _headers = value?.ToList(); }
    }

    public string?[] CurrentRow { get; private set; }

    protected IRowSplitter RowSplitter { get; init; }

    protected bool HasHeader => _headers?.Count > 0;

    public int Depth { get; } = 1;

    public string CurrentRowRaw { get; private set; }

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

            var ordinal = GetOrdinal(name);
            if (ordinal == -1)
            {
                throw new ArgumentException($"Header {name} not found");
            }

            return GetValue(ordinal);
        }
    }

    public bool Read()
    {
        CurrentRowNumber++;
        if (StreamReader.EndOfStream)
        {
            Close();
            return false;
        }

        if (_enclosingCharacter == null)
        {
            CurrentRowRaw = StreamReader.ReadLine()!;
            CurrentRow = RowSplitter.SplitRow(CurrentRowRaw);
            if (FieldCount == -1)
            {
                FieldCount = CurrentRow.Length;
            }

            return true;
        }
        else
        {
            int? charInt = null;
            bool isEscaped = false;
            string row = "";
            while ((charInt = StreamReader.Read()) != null)
            {
                if (StreamReader.EndOfStream)
                {
                    Close();

                    if (!string.IsNullOrEmpty(row))
                    {
                        CurrentRowRaw = row;
                        CurrentRow = RowSplitter.SplitRow(CurrentRowRaw);
                        if (FieldCount == -1)
                        {
                            FieldCount = CurrentRow.Length;
                        }

                        return true;
                    }
                    return false;
                }
                
                var @char = (char)charInt!;
                if (@char == _enclosingCharacter)
                {
                    isEscaped = !isEscaped;
                }

                if (@char is '\r' or '\n')
                {
                    if (isEscaped)
                    {
                        // continue
                    }
                    else if (@char is '\r')
                    {
                        continue;
                    }
                    else
                    {
                        CurrentRowRaw = row;
                        CurrentRow = RowSplitter.SplitRow(CurrentRowRaw);
                        if (FieldCount == -1)
                        {
                            FieldCount = CurrentRow.Length;
                        }

                        return true;
                    }
                }

                row += @char;
            }
        }

        return false;
    }

    public bool NextResult()
    {
        return false;
    }

    public bool GetBoolean(int i)
    {
        var value = CurrentRow[i];
        return value == null ? default : bool.Parse(value);
    }

    public byte GetByte(int i)
    {
        var value = CurrentRow[i];
        return value == null ? default : byte.Parse(value, CultureInfo.InvariantCulture);
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

    public long GetChars(int i, long fieldoffset, char[]? buffer, int bufferoffset, int length)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        var value = GetString(i).ToCharArray();

        long charRead = 0;

        for (var idx = 0;
             (idx < length) && ((idx + fieldoffset) < value.Length) &&
             (idx + bufferoffset) < buffer.Length;
             idx++)
        {
            buffer[bufferoffset + idx] = value[fieldoffset + idx];
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
        var dateAsString = GetString(i);

        if (AllowedDateFormats.Length != 0)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            return DateTime.ParseExact(dateAsString, AllowedDateFormats, provider);
        }

        return DateTime.Parse(dateAsString, CultureInfo.CurrentCulture);
    }

    public decimal GetDecimal(int i)
    {
        var value = CurrentRow[i];

        return value == null ? default : decimal.Parse(value, CultureInfo.CurrentCulture);
    }

    public double GetDouble(int i)
    {
        var value = CurrentRow[i];

        return value == null ? default : double.Parse(value, CultureInfo.CurrentCulture);
    }

    public Type GetFieldType(int i)
    {
        throw new NotImplementedException();
    }

    public float GetFloat(int i)
    {
        var value = CurrentRow[i];

        return value == null ? default : float.Parse(value, CultureInfo.CurrentCulture);
    }

    public Guid GetGuid(int i)
    {
        var value = CurrentRow[i];

        return value == null ? default : Guid.Parse(value);
    }

    public short GetInt16(int i)
    {
        var value = CurrentRow[i];

        return value == null ? default : short.Parse(value, CultureInfo.CurrentCulture);
    }

    public int GetInt32(int i)
    {
        var value = CurrentRow[i];

        if (value == null)
        {
            return default;
        }

        return int.Parse(value, CultureInfo.CurrentCulture);
    }

    public long GetInt64(int i)
    {
        var value = CurrentRow[i];

        if (value == null)
        {
            return default;
        }

        return long.Parse(value, CultureInfo.CurrentCulture);
    }

    public string GetName(int i)
    {
        if (!HasHeader)
        {
            throw new InvalidOperationException("Cannot get column name without headers");
        }

        var header = _headers![i];
        if (string.IsNullOrEmpty(header))
        {
            throw new InvalidOperationException($"Column for index {i} is null");
        }

        return header;
    }

    public int GetOrdinal(string name)
    {
        if (!HasHeader)
        {
            throw new InvalidOperationException("Cannot get column name without headers");
        }

        return _headers!.IndexOf(name);
    }

    public string GetString(int i)
    {
        return GetValue(i)?.ToString()!;
    }

    public object GetValue(int i)
    {
        var value = CurrentRow[i];

        if (value == null)
        {
            return default;
        }

        return value;
    }

    public int GetValues(object?[] values)
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
        var value = GetValue(i);

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        return value == null;
    }


    public void Dispose()
    {
        StreamReader.Dispose();
        GC.SuppressFinalize(this);
    }

    public void Close()
    {
        IsClosed = true;
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