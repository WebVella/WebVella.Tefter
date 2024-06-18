namespace WebVella.Tefter.Database.Dbo;
internal record DboModelPropertyMeta
{
	public PropertyInfo PropertyInfo { get; internal set; }
	public string ColumnName { get; internal set; }
	public IDboPropertyValueConverter Converter { get; set; } = null;

	public override string ToString()
	{
		return $"Name={PropertyInfo.Name}  Type={PropertyInfo.PropertyType}  Column={ColumnName}";
	}
}