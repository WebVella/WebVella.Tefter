namespace WebVella.Tefter.Database.Dbo;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
internal class DboSqlModelAttribute : Attribute
{
    public string QueryName { get; set; } = string.Empty;

    public DboSqlModelAttribute()
    {
    }

    public DboSqlModelAttribute(string queryName)
    {
        QueryName = queryName;
    }
}