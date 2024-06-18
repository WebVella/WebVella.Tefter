namespace WebVella.Tefter.Database.Dbo;

internal class JsonPropertyConverter<T> : IDboPropertyValueConverter
{
#pragma warning disable CS8632
    public bool CanConvert(Type type)
    {
        return type == typeof(T) || type == typeof(T?) || type == typeof(string);
    }

#pragma warning restore CS8632

    public object ConvertFromDatabaseType(object obj)
    {
        if (obj == null)
            return null;

        if (obj is string)
        {
            if (string.IsNullOrWhiteSpace((string)obj))
                return null;

            return JsonSerializer.Deserialize<T>((string)obj);
        }

        throw new Exception("JsonPropertyConverter: argument is not string");
    }

    public object ConvertToDatabaseType(object obj)
    {
        if (obj == null)
            return null;

        if (obj is T)
            return JsonSerializer.Serialize(obj);

        throw new Exception($"JsonPropertyConverter: argument is not from generic type {typeof(T).FullName}");
    }
}
