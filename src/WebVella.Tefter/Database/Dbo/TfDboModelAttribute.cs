namespace WebVella.Tefter.Database.Dbo;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
internal class TfDboModelAttribute : Attribute
{
    public string TableName { get; set; } = string.Empty;

    public TfDboModelAttribute()
    {
    }

    public TfDboModelAttribute(string tableName)
    {
        TableName = tableName;
    }
}