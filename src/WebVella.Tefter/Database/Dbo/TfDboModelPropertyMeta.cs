namespace WebVella.Tefter.Database.Dbo;
internal record TfDboModelPropertyMeta
{
	public PropertyInfo PropertyInfo { get; internal set; }
	public string ColumnName { get; internal set; }
	public ITfDboPropertyValueConverter Converter { get; set; } = null;
	public bool IsAutoIncrement { get; set; } = false;

	public override string ToString()
	{
		return $"Name={PropertyInfo.Name}  Type={PropertyInfo.PropertyType}  Column={ColumnName} IsAutoIncrement={IsAutoIncrement}";
	}
}