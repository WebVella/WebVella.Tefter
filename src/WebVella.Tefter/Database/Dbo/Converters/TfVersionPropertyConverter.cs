namespace WebVella.Tefter.Database.Dbo;
internal class TfVersionPropertyConverter : ITfDboPropertyValueConverter
{
    public bool CanConvert(Type type)
    {
        return type == typeof(Version);
    }

	public object ConvertFromDatabaseType(object obj)
	{
		if (obj == null)
			return null;

		if (obj is string)
		{
			if (string.IsNullOrWhiteSpace((string)obj))
				return null;

			return new Version((string)obj);
		}

		throw new Exception("VersionPropertyConverter: argument is not string");
	}

	public object ConvertToDatabaseType(object obj)
	{
		if (obj == null)
			return null;

		if (obj is Version)
			return ((Version)obj).ToString();

		throw new Exception($"VersionPropertyConverter: argument is not from type {typeof(Version).FullName}");
	}
}
