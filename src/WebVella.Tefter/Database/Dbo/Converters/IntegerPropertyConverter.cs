namespace WebVella.Tefter.Database.Dbo;

internal class IntegerPropertyConverter : IDboPropertyValueConverter
{
    public bool CanConvert(Type type)
    {
        return type == typeof(decimal) || type == typeof(int) || type == typeof(decimal?) || type == typeof(int?);
    }

    public object ConvertFromDatabaseType(object obj)
    {
        if (obj == null)
            return null;

        if (obj is decimal)
            return Convert.ToInt32(obj);

        if (obj is decimal?)
            return (int?)Convert.ToInt32(obj);

        throw new Exception("DboConverter trying to convert to int object different from decimal");
    }

    public object ConvertToDatabaseType(object obj)
    {
        if (obj == null)
            return null;

        if (obj is int)
            return Convert.ToDecimal(obj);
        
        if (obj is int?)
            return (decimal?)Convert.ToDecimal(obj);

        throw new Exception("DboConverter trying to convert to decimal object different from integer");
    }
}
