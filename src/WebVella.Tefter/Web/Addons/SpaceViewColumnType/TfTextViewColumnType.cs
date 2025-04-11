namespace WebVella.Tefter.Web.Addons;
public class TfTextViewColumnType : ITfSpaceViewColumnTypeAddon
{
	const string TF_COLUMN_TEXT_ID = TfConstants.TF_GENERIC_TEXT_COLUMN_TYPE_ID;
	const string TF_COLUMN_TEXT_NAME = "Text";
	const string TF_COLUMN_TEXT_DESCRIPTION = "displays text";
	const string TF_COLUMN_TEXT_ICON = "TextCaseTitle";
	const string ALIAS = "Value";

	public Guid Id { get; init; }
	public string Name { get; init; }
	public string Description { get; init; }
	public string FluentIconName { get; init; }
	public List<TfSpaceViewColumnAddonDataMapping> DataMapping { get; init; }
	public List<string> FilterAliases { get; init; }
	public List<string> SortAliases { get; init; }
	public Guid? DefaultComponentId { get; init; }
	public List<Guid> SupportedComponents { get; set; } = new();
	public TfTextViewColumnType()
	{
		Id = new Guid(TF_COLUMN_TEXT_ID);
		Name = TF_COLUMN_TEXT_NAME;
		Description = TF_COLUMN_TEXT_DESCRIPTION;
		FluentIconName = TF_COLUMN_TEXT_ICON;
		DataMapping = new List<TfSpaceViewColumnAddonDataMapping>
		{
			new TfSpaceViewColumnAddonDataMapping
				{
					Alias = ALIAS,
					Description = "this column is compatible with all column types, but its intended use is with text",
					SupportedDatabaseColumnTypes = new List<TfDatabaseColumnType> {
						TfDatabaseColumnType.ShortText,
						TfDatabaseColumnType.Text
					}
				}
		};
		FilterAliases = new List<string>() { ALIAS };
		SortAliases = new List<string> { ALIAS };
		DefaultComponentId = new Guid(TfConstants.TF_COLUMN_COMPONENT_DISPLAY_TEXT_ID);
	}
}

