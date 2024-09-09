namespace WebVella.Tefter.Web.Components;
public partial class TfTextColumn : TfBaseColumn
{


	private string _value = string.Empty;

	protected override void OnInitialized()
	{
		base.OnInitialized();
		_value = string.Empty;
		if (Data is not null && Data.Fields is not null && Data.Fields.ContainsKey(Meta.ColumnName))
		{
			if (Data.Fields[Meta.ColumnName].Type == DataFieldType.Text)
			{
				_value = Data.Fields[Meta.ColumnName].Value as string;
			}
		}
	}

}