namespace WebVella.Tefter.Database.Dbo;
internal class LongPropertyConverter : IDboPropertyValueConverter
{
    public bool CanConvert(Type type)
    {
        return type == typeof(decimal) || type == typeof(Int64) || type == typeof(long) || type == typeof(decimal?) || type == typeof(Int64?) || type == typeof(long?);
    }

    public object ConvertFromDatabaseType(object obj)
    {
        if (obj == null)
            return null;

        if (obj is decimal)
            return Convert.ToInt64(obj);

        if (obj is decimal?)
            return (int?)Convert.ToInt64(obj);

        throw new Exception("DboConverter trying to convert to int64 object different from decimal");
    }

    public object ConvertToDatabaseType(object obj)
    {
        if (obj == null)
            return null;

        if (obj is Int64)
            return Convert.ToDecimal(obj);
        
        if (obj is Int64?)
            return (decimal?)Convert.ToDecimal(obj);

        throw new Exception("DboConverter trying to convert to decimal object different from Int64");
    }
}
