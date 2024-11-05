namespace WebVella.Tefter.Database.Dbo;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
internal class TfDboSqlModelAttribute : Attribute
{
    public string QueryName { get; set; } = string.Empty;

    public TfDboSqlModelAttribute()
    {
    }

    public TfDboSqlModelAttribute(string queryName)
    {
        QueryName = queryName;
    }
}