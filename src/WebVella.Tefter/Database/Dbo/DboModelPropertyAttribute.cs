namespace WebVella.Tefter.Database.Dbo;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
internal class DboModelPropertyAttribute : Attribute
{
    public string ColumnName { get; set; } = String.Empty;
    private DboModelPropertyAttribute()
    {
    }

    public DboModelPropertyAttribute(string columnName)
    {
        ColumnName = columnName;
    }
}
