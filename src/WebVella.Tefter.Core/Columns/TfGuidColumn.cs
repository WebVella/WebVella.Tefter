namespace WebVella.Tefter.Core;

[TfColumn(
	Id: Constants.TF_COLUMN_GUID_ID,
	Name: Constants.TF_COLUMN_GUID_NAME,
	Description: Constants.TF_COLUMN_GUID_DESCRIPTION)]
[TfColumnIcon(Constants.TF_COLUMN_GUID_ICON)]
public class TfGuidColumn : ITfColumn
{
	public TfColumnData Data { get; private set; }

	public TfColumnFilter Filter { get; private set; }

	public TfColumnSort Sort { get; private set; }

	public TfGuidColumn()
	{
		const string ALIAS_VALUE = "Value";

		Data = new TfColumnData
		{
			DefaultComponentType = typeof(TfGeneralDisplayComponent),
			
			SupportedComponentTypes = new List<Type> { typeof(TfGeneralDisplayComponent) },

			DatabaseRequirements = new List<TfColumnColumnDatabaseRequirement>
			{
				new TfColumnColumnDatabaseRequirement
				{
					Name = ALIAS_VALUE,
					Description = "This column works with Guid database column.",
					SupportedDatabaseColumnTypes = new List<DatabaseColumnType> { DatabaseColumnType.Guid }
				}
			},
		};
	
		Filter = new TfColumnFilter
		{
			DefaultComponentType = typeof(TfGeneralFilterComponent),
			
			SupportedComponentTypes = new List<Type> { typeof(TfGeneralFilterComponent) },
			
			DatabaseRequirementNames = new List<string> { ALIAS_VALUE }
		};
		
		Sort = new TfColumnSort
		{
			DefaultComponentType = typeof(TfGeneralSortComponent),
		
			SupportedComponentTypes = new List<Type> { typeof(TfGeneralSortComponent) },
			
			DatabaseRequirementNames = new List<string> { ALIAS_VALUE }
		};
	}
}

