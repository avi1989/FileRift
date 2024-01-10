using System.Data;
using System.Globalization;
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
    
    private CultureInfo _provider = CultureInfo.InvariantCulture;

    public IFileRiftDataReader DataReader => reader;

    public IReadOnlyCollection<ReadError> Errors => _errors;

    protected Dictionary<Type, Func<int, object?>> DataTypeReaders => new()
    {
        { typeof(string), ordinal => reader.GetString(ordinal) },
        { typeof(short), ordinal =>
            {
                var value = reader.GetString(ordinal);

                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }

                return short.Parse(value, CultureInfo.InvariantCulture);
            }
        },
        { typeof(int), ordinal =>
            {
                var value = reader.GetString(ordinal);

                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }

                return int.Parse(value, CultureInfo.InvariantCulture);
            }
        },
        { typeof(long), ordinal =>
            {
                var value = reader.GetString(ordinal);

                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }

                return long.Parse(value, CultureInfo.InvariantCulture);
            }
        },
        { typeof(bool), ordinal =>
            {
                var value = reader.GetString(ordinal);

                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }

                return bool.Parse(value);
            }
        },
        { typeof(float), ordinal =>
            {
                var value = reader.GetString(ordinal);

                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }

                return float.Parse(value);
            }
        },
        { typeof(double), ordinal =>
            {
                var value = reader.GetString(ordinal);

                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }

                return double.Parse(value); 
            }
        },
        { typeof(decimal), ordinal =>
            {
                var value = reader.GetString(ordinal);

                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }

                return decimal.Parse(value); 
            }
        },
        { typeof(Guid), ordinal =>
            {
                var value = reader.GetString(ordinal);

                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }

                return Guid.Parse(value); 
            }
        },
        { typeof(char), ordinal => reader.GetChar(ordinal) },
        { typeof(byte), ordinal => reader.GetByte(ordinal) },
        { typeof(DateTime), ordinal =>
            {
                var value = reader.GetString(ordinal);

                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }
                
                if (reader.AllowedDateFormats.Length != 0)
                {
                    CultureInfo provider = CultureInfo.InvariantCulture;
                    return DateTime.ParseExact(value, reader.AllowedDateFormats, provider);
                }

                return DateTime.Parse(value, CultureInfo.CurrentCulture);
            }
        },
    };


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
                    var type = item.DataType;
                    var underlyingType = Nullable.GetUnderlyingType(type);

                    if (underlyingType != null)
                    {
                        type = underlyingType;
                    }

                    if (DataTypeReaders.TryGetValue(type, out var readerFunc))
                    {
                        try
                        {
                            // If we have a nullable object, we should be able to separate 
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