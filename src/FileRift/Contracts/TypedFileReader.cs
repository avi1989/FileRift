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
        {
            typeof(short), ordinal =>
            {
                var value = reader.GetString(ordinal);

                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }

                return short.Parse(value, CultureInfo.InvariantCulture);
            }
        },
        {
            typeof(int), ordinal =>
            {
                var value = reader.GetString(ordinal);

                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }

                return int.Parse(value, CultureInfo.InvariantCulture);
            }
        },
        {
            typeof(long), ordinal =>
            {
                var value = reader.GetString(ordinal);

                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }

                return long.Parse(value, CultureInfo.InvariantCulture);
            }
        },
        {
            typeof(bool), ordinal =>
            {
                var value = reader.GetString(ordinal);

                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }

                return bool.Parse(value);
            }
        },
        {
            typeof(float), ordinal =>
            {
                var value = reader.GetString(ordinal);

                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }

                return float.Parse(value);
            }
        },
        {
            typeof(double), ordinal =>
            {
                var value = reader.GetString(ordinal);

                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }

                return double.Parse(value);
            }
        },
        {
            typeof(decimal), ordinal =>
            {
                var value = reader.GetString(ordinal);

                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }

                return decimal.Parse(value);
            }
        },
        {
            typeof(Guid), ordinal =>
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
        {
            typeof(DateTime), ordinal =>
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
            var headers = reader.Headers!.ToList();
            foreach (var header in headers)
            {
                var column = map.GetColumnMapping(header!);
                if (column == null)
                {
                    continue;
                }

                try
                {
                    int ordinal = reader.GetOrdinal(header!);
                    object? value = this.GetValue(ordinal, column);
                    _propertySetter.SetValue(res, column.PropertyName, value);
                }
                catch (Exception e)
                {
                    if (ignoreErrors)
                    {
                        _errors.Add(new(reader.CurrentRowNumber, reader.CurrentRowRaw, e));
                        continue;
                    }

                    throw new RowReadException(reader.CurrentRowNumber, reader.CurrentRowRaw, e);
                }
            }

            yield return res;
        }
    }

    private object? GetValue(int ordinal, ColumnMapping item)
    {
        object? typedValue;
        var type = item.DataType;
        var underlyingType = Nullable.GetUnderlyingType(type);

        if (underlyingType != null)
        {
            type = underlyingType;
        }

        if (DataTypeReaders.TryGetValue(type, out var readerFunc))
        {
            typedValue = readerFunc(ordinal);
        }
        else
        {
            throw new InvalidOperationException($"Mapper not found for type {type.FullName}");
        }

        return typedValue;
    }
}