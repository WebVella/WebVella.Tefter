namespace WebVella.Tefter.Core;

public class TfGuidViewColumnType : ITfSpaceViewColumnType
{
	const string TF_COLUMN_GUID_ID = "b736b4f9-2138-44d2-a5d5-4a320b6556db";
	const string TF_COLUMN_GUID_NAME = "Global unique identifier";
	const string TF_COLUMN_GUID_DESCRIPTION = "if you need an identifier value. The Guid is a unique 128-bit identifier generated to ensure global uniqueness.";
	const string TF_COLUMN_GUID_ICON = "WebVella.Tefter.Core.Assets.GuidIcon.png";
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

	public TfGuidViewColumnType()
	{
		

		Id = new Guid(TF_COLUMN_GUID_ID);

		Name = TF_COLUMN_GUID_NAME;

		Description = TF_COLUMN_GUID_DESCRIPTION;

		Icon = TF_COLUMN_GUID_ICON;

		DataMapping = new List<TfSpaceViewColumnDataMapping>
		{
			new TfSpaceViewColumnDataMapping
				{
					Alias = ALIAS,
					Description = "This column works with Guid database column.",
					SupportedDatabaseColumnTypes = new List<DatabaseColumnType> { DatabaseColumnType.Guid }
				}
		};

		FilterAliases = new List<string>() { ALIAS };

		SortAliases = new List<string> { ALIAS };

		DefaultComponentType = typeof(TfGeneralViewColumnComponent);

		SupportedComponentTypes = new List<Type> { typeof(TfGeneralViewColumnComponent) };
	}
}

