using System.Data;
using FileRift.Mappers;
using FileRift.Services;

namespace FileRift.Contracts;

public class TypedFileReader<T>(IDataReader reader, ClassMap<T> map)
    where T : class, new()
{
    private readonly PropertySetter<T> _propertySetter = new();

    protected Dictionary<string, Func<int, object>> DataTypeReaders => new()
    {
        { typeof(string).FullName!, ordinal => reader.GetString(ordinal) },
        { typeof(short).FullName!, ordinal => reader.GetInt16(ordinal) },
        { typeof(int).FullName!, ordinal => reader.GetInt32(ordinal) },
        { typeof(long).FullName!, ordinal => reader.GetInt64(ordinal) },
        {typeof(bool).FullName!, ordinal => reader.GetBoolean(ordinal)},
        { typeof(Guid).FullName!, ordinal => reader.GetGuid(ordinal) },
        { typeof(char).FullName!, ordinal => reader.GetChar(ordinal) },
        { typeof(byte).FullName!, ordinal => reader.GetByte(ordinal) },
        { typeof(float).FullName!, ordinal => reader.GetFloat(ordinal) },
        { typeof(double).FullName!, ordinal => reader.GetDouble(ordinal) },
        { typeof(decimal).FullName!, ordinal => reader.GetDecimal(ordinal) },
        { typeof(DateTime).FullName!, ordinal => reader.GetDateTime(ordinal) },
    };

    public IEnumerable<T> Read()
    {
        while (reader.Read())
        {
            // TODO: Handle creation when class has parameters;
            var res = new T();
            foreach (var item in map.ColumnMappings)
            {
                var ordinal = reader.GetOrdinal(item.ColumnName);

                if (ordinal == -1)
                {
                    continue;
                }

                object? typedValue;

                if (DataTypeReaders.TryGetValue(item.DataType, out var readerFunc))
                {
                    typedValue = readerFunc(ordinal);
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Unable to set property {item.ColumnName} with type {item.DataType}");
                }

                // // var value = reader.GetString(ordinal);
                _propertySetter.SetValue(res, item.PropertyName, typedValue);
            }

            yield return res;
        }
    }
}