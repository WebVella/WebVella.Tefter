namespace WebVella.Tefter.Web.ViewColumns;

/// <summary>
/// Description attribute is needed when presenting the component to the user as a select option
/// Localization attributes is needed to strongly type the location of the components translation resource
/// </summary>
[Description("Tefter Boolean Display")]
[LocalizationResource("WebVella.Tefter.Web.ViewColumns.Components.BooleanDisplayColumnComponent.TfBooleanDisplayColumnComponent", "WebVella.Tefter")]
public partial class TfBooleanDisplayColumnComponent : TfBaseViewColumn<TfBooleanDisplayColumnComponentOptions>
{
	/// <summary>
	/// Needed because of the custom constructor
	/// </summary>
	public TfBooleanDisplayColumnComponent()
	{
	}

	/// <summary>
	/// The custom constructor is needed because in varoius cases we need to instance the component without
	/// rendering. The export to excel is one of those cases.
	/// </summary>
	/// <param name="context">this value contains options, the entire DataTable as well as the row index that needs to be processed</param>
	public TfBooleanDisplayColumnComponent(TfComponentContext context)
	{
		Context = context;
	}

	/// <summary>
	/// The alias of the column name that stores the value.
	/// Depends on the ITfSpaceViewColumnType that renders this component
	/// by default it is 'Value'. The alias<>column name mapping is set by the user
	/// upon space view column configuration
	/// </summary>
	private string _valueAlias = "Value";

	/// <summary>
	/// Overrides the default export method in order to apply its own options
	/// </summary>
	/// <returns></returns>
	public override object GetData()
	{
		//options are not inited yet as the component is not rendered
		bool? value = GetDataObjectByAlias<bool>(_valueAlias, null);
		
		if (value is null) return null;

		var options = GetOptions();

		if (value.Value && !String.IsNullOrWhiteSpace(options.TrueValueOverrideText)) return options.TrueValueOverrideText;
		else if (!value.Value && !String.IsNullOrWhiteSpace(options.FalseValueOverrideText)) return options.FalseValueOverrideText;
		
		return value;
	}
}

/// <summary>
/// The options used by the system to serialize and restore the components option
/// from the database
/// </summary>
public class TfBooleanDisplayColumnComponentOptions
{
	[JsonPropertyName("TrueValueOverrideText")]
	public string TrueValueOverrideText { get; set; }

	[JsonPropertyName("TrueValueShowAsIcon")]
	public bool TrueValueShowAsIcon { get; set; }

	[JsonPropertyName("FalseValueOverrideText")]
	public string FalseValueOverrideText { get; set; }

	[JsonPropertyName("FalseValueShowAsIcon")]
	public bool FalseValueShowAsIcon { get; set; }

	[JsonPropertyName("NullValueOverrideText")]
	public string NullValueOverrideText { get; set; }

	[JsonPropertyName("NullValueShowAsIcon")]
	public bool NullValueShowAsIcon { get; set; }
}