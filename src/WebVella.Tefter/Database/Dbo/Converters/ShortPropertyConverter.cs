namespace WebVella.Tefter.Database.Dbo;

internal class ShortPropertyConverter : IDboPropertyValueConverter
{
	public bool CanConvert(Type type)
	{
		return type == typeof(decimal) || type == typeof(Int16) || type == typeof(short) || type == typeof(decimal?) || type == typeof(Int16?) || type == typeof(short?);
	}

	public object ConvertFromDatabaseType(object obj)
	{
		if (obj == null)
			return null;

		if (obj is decimal)
			return Convert.ToInt16(obj);

		if (obj is decimal?)
			return (int?)Convert.ToInt16(obj);

		throw new Exception("DboConverter trying to convert to int16 object different from decimal");
	}

	public object ConvertToDatabaseType(object obj)
	{
		if (obj == null)
			return null;

		if (obj is Int16)
			return Convert.ToDecimal(obj);

		if (obj is Int16?)
			return (decimal?)Convert.ToDecimal(obj);

		throw new Exception("DboConverter trying to convert to decimal object different from Int16");
	}
}

