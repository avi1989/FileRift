using System.Data;
using FileRift.Mappers;
using FileRift.Services;

namespace FileRift.Contracts;

public class TypedFileReader<T>(
    IFileRiftDataReader reader,
    ClassMap<T> map,
    bool ignoreErrors = false)
    where T : class, new()
{
    private readonly PropertySetter<T> _propertySetter = new();

    private readonly List<ReadError> _errors = new();

    public IFileRiftDataReader DataReader => reader;

    public IReadOnlyCollection<ReadError> Errors => _errors;

    protected Dictionary<string, Func<int, object>> DataTypeReaders => new()
    {
        { typeof(string).FullName!, ordinal => reader.GetString(ordinal) },
        { typeof(short).FullName!, ordinal => reader.GetInt16(ordinal) },
        { typeof(int).FullName!, ordinal => reader.GetInt32(ordinal) },
        { typeof(long).FullName!, ordinal => reader.GetInt64(ordinal) },
        { typeof(bool).FullName!, ordinal => reader.GetBoolean(ordinal) },
        { typeof(Guid).FullName!, ordinal => reader.GetGuid(ordinal) },
        { typeof(char).FullName!, ordinal => reader.GetChar(ordinal) },
        { typeof(byte).FullName!, ordinal => reader.GetByte(ordinal) },
        { typeof(float).FullName!, ordinal => reader.GetFloat(ordinal) },
        { typeof(double).FullName!, ordinal => reader.GetDouble(ordinal) },
        { typeof(decimal).FullName!, ordinal => reader.GetDecimal(ordinal) },
        { typeof(DateTime).FullName!, ordinal => reader.GetDateTime(ordinal) },

        { typeof(short?).FullName!, ordinal => reader.GetInt16(ordinal) },
        { typeof(int?).FullName!, ordinal => reader.GetInt32(ordinal) },
        { typeof(long?).FullName!, ordinal => reader.GetInt64(ordinal) },
        { typeof(bool?).FullName!, ordinal => reader.GetBoolean(ordinal) },
        { typeof(Guid?).FullName!, ordinal => reader.GetGuid(ordinal) },
        { typeof(char?).FullName!, ordinal => reader.GetChar(ordinal) },
        { typeof(byte?).FullName!, ordinal => reader.GetByte(ordinal) },
        { typeof(float?).FullName!, ordinal => reader.GetFloat(ordinal) },
        { typeof(double?).FullName!, ordinal => reader.GetDouble(ordinal) },
        { typeof(decimal?).FullName!, ordinal => reader.GetDecimal(ordinal) },
        { typeof(DateTime?).FullName!, ordinal => reader.GetDateTime(ordinal) },
    };

    public IEnumerable<T> Read1()
    {
        while (reader.Read())
        {
            // TODO: Handle creation when class has parameters;
            var res = new T();
            foreach (var item in map.ColumnMappings)
            {
                int ordinal;
                try
                {
                    ordinal = reader.GetOrdinal(item.ColumnName);

                    if (ordinal == -1)
                    {
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    throw new RowReadException(reader.CurrentRowNumber, reader.CurrentRowRaw, ex);
                }

                object? typedValue;

                if (DataTypeReaders.TryGetValue(item.DataType, out var readerFunc))
                {
                    try
                    {
                        typedValue = readerFunc(ordinal);
                    }
                    catch (Exception e)
                    {
                        throw new RowReadException(
                            reader.CurrentRowNumber,
                            reader.CurrentRowRaw,
                            $"Unable to get property {item.ColumnName} with type {item.DataType}",
                            e);
                    }
                }
                else
                {
                    throw new RowReadException(
                        reader.CurrentRowNumber,
                        reader.CurrentRowRaw,
                        $"Unable to set property {item.ColumnName} with type {item.DataType}",
                        null);
                }

                // // var value = reader.GetString(ordinal);
                _propertySetter.SetValue(res, item.PropertyName, typedValue);
            }

            yield return res;
        }
    }

    public IEnumerable<T> Read()
    {
        while (reader.Read())
        {
            T res = new T();

            try
            {
                foreach (var item in map.ColumnMappings)
                {
                    int ordinal;
                    try
                    {
                        ordinal = reader.GetOrdinal(item.ColumnName);

                        if (ordinal == -1)
                        {
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new RowReadException(reader.CurrentRowNumber,
                                                   reader.CurrentRowRaw,
                                                   ex);
                    }

                    object? typedValue;

                    if (DataTypeReaders.TryGetValue(item.DataType, out var readerFunc))
                    {
                        try
                        {
                            typedValue = readerFunc(ordinal);
                        }
                        catch (Exception e)
                        {
                            throw new RowReadException(
                                reader.CurrentRowNumber,
                                reader.CurrentRowRaw,
                                $"Unable to get property {item.ColumnName} with type {item.DataType}",
                                e);
                        }
                    }
                    else
                    {
                        throw new RowReadException(
                            reader.CurrentRowNumber,
                            reader.CurrentRowRaw,
                            $"Unable to set property {item.ColumnName} with type {item.DataType}",
                            null);
                    }

                    // // var value = reader.GetString(ordinal);
                    _propertySetter.SetValue(res, item.PropertyName, typedValue);
                }
            }
            catch (Exception e)
            {
                if (ignoreErrors)
                {
                    _errors.Add(new (reader.CurrentRowNumber, reader.CurrentRowRaw, e));
                    continue;
                }
                else
                {
                    throw;
                }
            }

            yield return res;
        }
    }
}