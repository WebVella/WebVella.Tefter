namespace WebVella.Tefter.Database.Dbo;
internal class LegacyDateTimePropertyConverter : IDboPropertyValueConverter
{
    public bool CanConvert(Type type)
    {
        return type == typeof(DateTime) || type == typeof(DateTime?);
    }

    public object ConvertFromDatabaseType(object obj)
    {
        if (obj == null)
            return null;

        //Boz & Rumen: as the kind of the value is returned as Unspecified by the driver
        //legacy mode of Driver is used
        //this makes all query to return DateTime values in LOCAL value with Unspecified KIND
        //if this setting is removed the values will be in UTC with Unspecified KIND

        if (obj is DateTime)
        {
            var value = Convert.ToDateTime(obj);
            return DateTime.SpecifyKind(value, DateTimeKind.Local);
        }

        if (obj is DateTime?)
        {
            var value = (DateTime?)Convert.ToDateTime(obj);
            return DateTime.SpecifyKind(value.Value, DateTimeKind.Local);
        }
        
        throw new Exception("LegacyDateTimePropertyConverter: invalid object to convert");
    }

    public object ConvertToDatabaseType(object obj)
    {
        if (obj == null)
            return null;

        if (obj is DateTime)
            return Convert.ToDateTime(obj);

        if (obj is DateTime?)
            return (DateTime?)Convert.ToDateTime(obj);

        throw new Exception("LegacyDateTimePropertyConverter: invalid object to convert");
    }
}
