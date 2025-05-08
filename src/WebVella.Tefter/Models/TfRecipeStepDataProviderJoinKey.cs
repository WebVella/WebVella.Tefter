namespace WebVella.Tefter;

public class TfRecipeStepDataProviderJoinKey
{
	public Guid Id { get; set; }

	public string DbName { get; set; }

	public string Description { get; set; }

	public List<string> Columns { get; set; } = new();

	public short Version { get; set; } = 1;

	public DateTime LastModifiedOn { get; set; }

	public void FixProviderPrefix(string dpPrefix)
	{
		if (Columns is not null)
		{
			var fixedList = new List<string>();
			foreach (var columnName in Columns)
			{
				var fixColumnName = columnName;
				if (!fixColumnName.StartsWith(dpPrefix))
				{
					fixColumnName = dpPrefix + fixColumnName;
				}
				fixedList.Add(fixColumnName);
			}
			Columns = fixedList;
		}
	}

	public TfRecipeStepDataProviderJoinKey()
	{
		
	}

	public TfRecipeStepDataProviderJoinKey(Guid id, string dbName, List<string> columns)
	{
		Id = id;
		DbName = dbName;
		Description = string.Empty;
		Columns = columns;
		Version = 1;
		LastModifiedOn = DateTime.Now;
	}
}
