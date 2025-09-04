namespace WebVella.Tefter;

public class TfRecipeStepDataProviderDataIdentity
{
	public Guid Id { get; set; }

	public string DataIdentity { get; set; }
	public List<string> Columns { get; set; } = new();
	public void FixPrefix(string prefix)
	{
		if (Columns is not null)
		{
			var fixedList = new List<string>();
			foreach (var columnName in Columns)
			{
				var fixColumnName = columnName;
				if (!fixColumnName.StartsWith(prefix))
				{
					fixColumnName = prefix + fixColumnName;
				}
				fixedList.Add(fixColumnName);
			}
			Columns = fixedList;
		}
	}

	public TfRecipeStepDataProviderDataIdentity()
	{
		
	}

	public TfRecipeStepDataProviderDataIdentity(Guid id, string dataIdentity, List<string> columns)
	{
		Id = id;
		DataIdentity = dataIdentity;
		Columns = columns;
	}
}
