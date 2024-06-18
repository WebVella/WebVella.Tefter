namespace WebVella.Tefter.Database.Dbo;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
internal class DboModelAttribute : Attribute
{
    public string TableName { get; set; } = string.Empty;

    public DboModelAttribute()
    {
    }

    public DboModelAttribute(string tableName)
    {
        TableName = tableName;
    }
}