namespace WebVella.Tefter.Database.Dbo;

internal class TfEnumPropertyConverter<T> : ITfDboPropertyValueConverter where T : struct, Enum
{
    public bool CanConvert(Type type)
    {
        return type == typeof(T) || type == typeof(T?) || type == typeof(short) || type == typeof(short?);
    }

    public object ConvertFromDatabaseType(object obj)
    {
        if (obj == null)
            return null;

        if (obj is short)
        {
            int intValue = Convert.ToInt32(obj);
            return Enum.ToObject(typeof(T), intValue);
        }

        if (obj is short?)
        {
            int intValue = Convert.ToInt32(((short?)obj).Value);
            return Enum.ToObject(typeof(T?), intValue);
        }

        throw new Exception("EnumPropertyConverter: argument is not short");
    }

    public object ConvertToDatabaseType(object obj)
    {
        if (obj == null)
            return null;

        if (obj is T)
        {
            short intValue = (short)(int)obj;
            return Convert.ToInt16(intValue);
        }

        throw new Exception($"EnumPropertyConverter: argument is not from defined enum type {typeof(T)}");
    }
}
