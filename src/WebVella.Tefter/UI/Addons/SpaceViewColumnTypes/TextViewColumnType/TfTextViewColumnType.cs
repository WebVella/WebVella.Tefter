namespace WebVella.Tefter.UI.Addons;
public class TfTextViewColumnType : ITfSpaceViewColumnTypeAddon
{
	#region << INIT >>
	public const string ID = "f061a3ce-7813-4fd6-98cb-a10cccea4797";
	public const string NAME = "Text";
	public const string DESCRIPTION = "displays text";
	public const string FLUENT_ICON_NAME = "TextCaseTitle";
	public const string VALUE_ALIAS = "Value";

	public Guid AddonId { get; init; } = new Guid(ID);
	public string AddonName { get; init; } = NAME;
	public string AddonDescription { get; init; } = DESCRIPTION;
	public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
	public List<TfSpaceViewColumnAddonDataMapping> DataMapping { get; init; } = new()
	{
		new TfSpaceViewColumnAddonDataMapping
		{
			Alias = VALUE_ALIAS,
			Description =
				"this column is compatible with all column types, but its intended use is with text",
			SupportedDatabaseColumnTypes = new List<TfDatabaseColumnType>
			{
				TfDatabaseColumnType.ShortText, TfDatabaseColumnType.Text
			}
		}
	};
	#endregion
	
	#region << PUBLIC >>

	public void ProcessExcelCell(
		TfSpaceViewColumnScreenRegionContext regionContext,
		IXLCell excelCell) { }

	//Returns Value/s as string usually for CSV export
	public string? GetValueAsString(
		TfSpaceViewColumnScreenRegionContext regionContext)
	{
		return null;
	}

	public RenderFragment Render(TfSpaceViewColumnScreenRegionContext regionContext)
	{
		return builder =>
		{
			builder.OpenElement(0, "span");
			builder.AddContent(1, "test");
			builder.CloseElement();
		};
	}
	#endregion
}