namespace WebVella.Tefter.Database.Dbo;

internal class EnumPropertyConverter<T> : IDboPropertyValueConverter where T : struct, Enum
{
    public bool CanConvert(Type type)
    {
        bool t1 = type == typeof(T);
        bool t2 = type == typeof(T?);
        bool t3 = type == typeof(decimal);
        bool t4 = type == typeof(decimal?);

        return type == typeof(T) || type == typeof(T?) || type == typeof(decimal) || type == typeof(decimal?);
    }

    public object ConvertFromDatabaseType(object obj)
    {
        if (obj == null)
            return null;

        if (obj is decimal)
        {
            int intValue = Convert.ToInt32(obj);
            return Enum.ToObject(typeof(T), intValue);
        }

        if (obj is decimal?)
        {
            int intValue = Convert.ToInt32(((decimal?)obj).Value);
            return Enum.ToObject(typeof(T?), intValue);
        }

        throw new Exception("EnumPropertyConverter: argument is not decimal");
    }

    public object ConvertToDatabaseType(object obj)
    {
        if (obj == null)
            return null;

        if (obj is T)
        {
            int intValue = (int)obj;
            return Convert.ToDecimal(intValue);
        }

        throw new Exception($"EnumPropertyConverte: argument is not from defined enum type {typeof(T)}");
    }
}
