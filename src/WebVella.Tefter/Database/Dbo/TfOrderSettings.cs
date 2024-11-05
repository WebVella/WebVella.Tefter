namespace WebVella.Tefter.Database.Dbo;

internal enum OrderDirection
{
	DESC,
	ASC
}

internal record TfOrderSettings
{
	public List<Tuple<string, OrderDirection>> PropOrderList { get; set; } = new List<Tuple<string, OrderDirection>>();

	public TfOrderSettings(string propertyName, OrderDirection direction)
	{
		PropOrderList.Add(new Tuple<string, OrderDirection>(propertyName, direction));
	}

	public TfOrderSettings Add(string propertyName, OrderDirection direction)
	{
		if (PropOrderList.Any(x => x.Item1 == propertyName))
			throw new Exception($"Property {propertyName} is already in order list");

		PropOrderList.Add(new Tuple<string, OrderDirection>(propertyName, direction));
		return this;
	}
}
