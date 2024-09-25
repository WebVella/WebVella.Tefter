namespace WebVella.Tefter.Database.Dbo;
internal class DateTimePropertyConverter : IDboPropertyValueConverter
{
    public bool CanConvert(Type type)
    {
        return type == typeof(DateTime) || type == typeof(DateTime?);
    }

    public object ConvertFromDatabaseType(object obj)
    {
        if (obj == null)
            return null;

        if (obj is DateTime)
        {
            var value = Convert.ToDateTime(obj);
			return DateTime.SpecifyKind(value, DateTimeKind.Unspecified);//.ToLocalTime();
        }

        if (obj is DateTime?)
        {
            var value = (DateTime?)Convert.ToDateTime(obj);
			return DateTime.SpecifyKind(value.Value, DateTimeKind.Unspecified);//.ToLocalTime();
        }
        
        throw new Exception("DateTimePropertyConverter: invalid object to convert");
    }

    public object ConvertToDatabaseType(object obj)
    {
        if (obj == null)
            return null;

        if (obj is DateTime)
            return Convert.ToDateTime(obj);

        if (obj is DateTime?)
            return (DateTime?)Convert.ToDateTime(obj);

        throw new Exception("DateTimePropertyConverter: invalid object to convert");
    }
}
