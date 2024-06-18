namespace WebVella.Tefter.Core;

[TfColumn(
	Id: Constants.TF_COLUMN_DATE_ID,
	Name: Constants.TF_COLUMN_DATE_NAME,
	Description: Constants.TF_COLUMN_DATE_DESCRIPTION)]
[TfColumnIcon(Constants.TF_COLUMN_DATE_ICON)]
public class TfDateColumn : ITfColumn
{
	public TfColumnData Data { get; private set; }

	public TfColumnFilter Filter { get; private set; }

	public TfColumnSort Sort { get; private set; }

	public TfDateColumn()
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
					Description = "This column works with Date database column.",
					SupportedDatabaseColumnTypes = new List<DatabaseColumnType> { DatabaseColumnType.Date }
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

