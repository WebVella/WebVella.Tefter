/// <summary>
/// Description attribute is needed when presenting the component to the user as a select option
/// Localization attributes is needed to strongly type the location of the components translation resource
/// </summary>

namespace WebVella.Tefter.Web.ViewColumns;
[Description("Tefter Number")]
[LocalizationResource("WebVella.Tefter.Web.ViewColumns.Components.NumberDisplayColumnComponent.TfNumberDisplayColumnComponent", "WebVella.Tefter")]
public partial class TfNumberDisplayColumnComponent : TfBaseViewColumn<TfNumberDisplayColumnComponentOptions>
{
	/// <summary>
	/// Needed because of the custom constructor
	/// </summary>
	public TfNumberDisplayColumnComponent()
	{
	}
	
	/// <summary>
	/// The custom constructor is needed because in varoius cases we need to instance the component without
	/// rendering. The export to excel is one of those cases.
	/// </summary>
	/// <param name="context">this value contains options, the entire DataTable as well as the row index that needs to be processed</param>	
	public TfNumberDisplayColumnComponent(TfComponentContext context)
	{
		Context = context;
	}

	/// <summary>
	/// Overrides the default export method in order to apply its own options
	/// </summary>
	/// <returns></returns>
	public override object GetData()
	{
		return GetDataObjectByAlias<decimal>("Value");
	}

	private string _getCastedValue()
	{
		var value = GetDataObjectByAlias<decimal>("Value", null);
		var column = Context.DataTable.Columns[Context.DataMapping["Value"]];
		var format = options.Format?.Trim();
		if (column.DbType == DatabaseColumnType.Number)
		{
			return ((decimal)value).ToString(format);
		}
		else if (column.DbType == DatabaseColumnType.ShortInteger)
		{
			return ((short)value).ToString(format);
		}
		else if (column.DbType == DatabaseColumnType.Integer)
		{
			return ((int)value).ToString(format);
		}
		else if (column.DbType == DatabaseColumnType.LongInteger)
		{
			return ((Int64)value).ToString(format);
		}
		else
		{
			return value.ToString();
		}
	}
}

/// <summary>
/// The options used by the system to serialize and restore the components option
/// from the database
/// </summary>
public class TfNumberDisplayColumnComponentOptions
{
	[JsonPropertyName("Format")]
	public string Format { get; set; }
}