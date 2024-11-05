namespace WebVella.Tefter.Database.Dbo;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
internal class TfDboModelPropertyAttribute : Attribute
{
    public string ColumnName { get; set; } = String.Empty;
    private TfDboModelPropertyAttribute()
    {
    }

    public TfDboModelPropertyAttribute(string columnName)
    {
        ColumnName = columnName;
    }
}
