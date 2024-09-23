namespace WebVella.Tefter.Web.ViewColumns;

public class TfBooleanViewColumnType : ITfSpaceViewColumnType
{
	const string TF_COLUMN_BOOLEAN_ID = Constants.TF_GENERIC_BOOLEAN_COLUMN_TYPE_ID;
	const string TF_COLUMN_BOOLEAN_NAME = "Boolean";
	const string TF_COLUMN_BOOLEAN_DESCRIPTION = "displays a boolean";
	const string TF_COLUMN_BOOLEAN_ICON = "CircleMultipleSubtractCheckmark";
	const string ALIAS = "Value";

	public Guid Id { get; init; }
	public string Name { get; init; }
	public string Description { get; init; }
	public string Icon { get; init; }
	public List<TfSpaceViewColumnDataMapping> DataMapping { get; init; }
	public Type DefaultComponentType { get; init; }
	public List<Type> SupportedComponentTypes { get; init; }
	public List<string> FilterAliases { get; init; }
	public List<string> SortAliases { get; init; }
	public List<Guid> SupportedAddonTypes { get; init; } = new();


	public TfBooleanViewColumnType()
	{
		Id = new Guid(TF_COLUMN_BOOLEAN_ID);

		Name = TF_COLUMN_BOOLEAN_NAME;

		Description = TF_COLUMN_BOOLEAN_DESCRIPTION;

		Icon = TF_COLUMN_BOOLEAN_ICON;

		DataMapping = new List<TfSpaceViewColumnDataMapping>
		{
			new TfSpaceViewColumnDataMapping
				{
					Alias = ALIAS,
					Description = "this column is compatible with the Boolean database column type",
					SupportedDatabaseColumnTypes = new List<DatabaseColumnType> { DatabaseColumnType.Boolean }
				}
		};

		FilterAliases = new List<string>() { ALIAS };

		SortAliases = new List<string> { ALIAS };

		DefaultComponentType = typeof(TfBooleanDisplayColumnComponent);

		SupportedComponentTypes = new List<Type> {
			typeof(TfBooleanDisplayColumnComponent),
			typeof(TfTextDisplayColumnComponent)
			};
	}
}

