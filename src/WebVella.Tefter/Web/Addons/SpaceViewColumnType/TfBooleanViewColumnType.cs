namespace WebVella.Tefter.Web.Addons;

public class TfBooleanViewColumnType : ITfSpaceViewColumnTypeAddon
{
	const string TF_COLUMN_BOOLEAN_ID = TfConstants.TF_GENERIC_BOOLEAN_COLUMN_TYPE_ID;
	const string TF_COLUMN_BOOLEAN_NAME = "Boolean";
	const string TF_COLUMN_BOOLEAN_DESCRIPTION = "displays a boolean";
	const string TF_COLUMN_BOOLEAN_ICON = "CircleMultipleSubtractCheckmark";
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

	public TfBooleanViewColumnType()
	{
		Id = new Guid(TF_COLUMN_BOOLEAN_ID);

		Name = TF_COLUMN_BOOLEAN_NAME;

		Description = TF_COLUMN_BOOLEAN_DESCRIPTION;

		FluentIconName = TF_COLUMN_BOOLEAN_ICON;

		DataMapping = new List<TfSpaceViewColumnAddonDataMapping>
		{
			new TfSpaceViewColumnAddonDataMapping
				{
					Alias = ALIAS,
					Description = "this column is compatible with the Boolean database column type",
					SupportedDatabaseColumnTypes = new List<TfDatabaseColumnType> { TfDatabaseColumnType.Boolean }
				}
		};

		FilterAliases = new List<string>() { ALIAS };

		SortAliases = new List<string> { ALIAS };

		DefaultComponentId = new Guid(TfConstants.TF_COLUMN_COMPONENT_DISPLAY_BOOLEAN_ID);
	}
}

