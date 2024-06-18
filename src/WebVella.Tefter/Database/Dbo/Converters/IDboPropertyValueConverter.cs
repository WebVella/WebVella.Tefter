namespace WebVella.Tefter.Database.Dbo;

internal interface IDboPropertyValueConverter
{ 
    bool CanConvert(Type type);
    object ConvertFromDatabaseType(object obj);
    object ConvertToDatabaseType(object obj);
}