namespace WebVella.Tefter.Models;

public record TfSortQuery
{
	[JsonPropertyName("n")]
	public string Name { get; set; } = default!;

	[JsonPropertyName("d")]
	public int Direction { get; set; } = (int)TfSortDirection.ASC;

	public TfSort? ToSort(List<TfSpaceViewColumn> viewColumns){ 
		var column = viewColumns.FirstOrDefault(x=> x.QueryName.ToLowerInvariant() == Name.ToLowerInvariant());
		if(column == null) return null;
		if(column.DataMapping is null || column.DataMapping.Keys.Count == 0) return null;

		var sort = new TfSort(){ 
			ColumnName = column.DataMapping[column.DataMapping.Keys.First()],
			Direction = Direction.ConvertIntToEnum<TfSortDirection>(TfSortDirection.ASC)
		};
		return sort;
	}

}

