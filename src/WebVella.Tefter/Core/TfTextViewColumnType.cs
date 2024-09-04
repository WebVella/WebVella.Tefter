namespace WebVella.Tefter.Core;

public class TfTextViewColumnType : ITfSpaceViewColumnType
{
	const string TF_COLUMN_TEXT_ID = "f061a3ce-7813-4fd6-98cb-a10cccea4797";
	const string TF_COLUMN_TEXT_NAME = "Text";
	const string TF_COLUMN_TEXT_DESCRIPTION = "A text column";
	const string TF_COLUMN_TEXT_ICON = "TextCaseTitle";
	const string ALIAS = "Value";

	public Guid Id { get; init; }
	public string Name { get; init; }
	public string Description { get; init; }
	public string Icon { get; init; }
	public List<TfSpaceViewColumnDataMapping> DataMapping { get; init; }
	public Type DefaultComponentType { get; init; }
	public Type CustomOptionsComponentType { get; init; }
	public List<Type> SupportedComponentTypes { get; init; }
	public List<string> FilterAliases { get; init; }
	public List<string> SortAliases { get; init; }
	public List<Guid> SupportedAddonTypes { get; init; } = new();


	public TfTextViewColumnType()
	{

		Id = new Guid(TF_COLUMN_TEXT_ID);

		Name = TF_COLUMN_TEXT_NAME;

		Description = TF_COLUMN_TEXT_DESCRIPTION;

		Icon = TF_COLUMN_TEXT_ICON;

		DataMapping = new List<TfSpaceViewColumnDataMapping>
		{
			new TfSpaceViewColumnDataMapping
				{
					Alias = ALIAS,
					Description = "This column works with Text database column.",
					SupportedDatabaseColumnTypes = new List<DatabaseColumnType> {
						DatabaseColumnType.ShortText,
						DatabaseColumnType.Text
					}
				}
		};

		FilterAliases = new List<string>() { ALIAS };

		SortAliases = new List<string> { ALIAS };

		DefaultComponentType = typeof(TfGeneralViewColumnComponent);

		SupportedComponentTypes = new List<Type> { typeof(TfGeneralViewColumnComponent) };
	}
}

