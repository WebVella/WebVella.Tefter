namespace WebVella.Tefter;

public class TfFilterAnd : TfFilterBase
{
	[JsonIncludePrivateProperty]
	[JsonPropertyName("ft")]
	private List<TfFilterBase> _filters { get; set; }

	[JsonIgnore]
	public ReadOnlyCollection<TfFilterBase> Filters => _filters.AsReadOnly();

	public string GetColumnName() => "AND";
	public string GetFilterType() => "rule";

	public TfFilterAnd()
		: base(string.Empty, string.Empty)
	{
		_filters = new List<TfFilterBase>();
	}

	public TfFilterAnd(
		params TfFilterBase[] filters)
		: this()
	{
		if (filters is null)
			throw new ArgumentNullException(nameof(filters));

		_filters.AddRange(filters);
	}

	public TfFilterAnd(TfFilterQuery model) : base(string.Empty,null)
	{
		Id = Guid.NewGuid();
		_filters = new();
		foreach (var item in model.Items)
		{
			_filters.Add(TfFilterBase.FromQuery(item));
		}
	}
	public TfFilterQuery ToQuery()
	{
		var query = new TfFilterQuery
		{
			Type = GetFilterType(),
			Name = GetColumnName()
		};
		foreach (var item in Filters)
		{
			query.Items.Add(TfFilterBase.ToQuery(item));
		}
		return query;
	}

	public void Add(
		TfFilterBase filter)
	{
		if (filter is null)
			throw new ArgumentNullException(nameof(filter));

		_filters.Add(filter);
	}

	public void AddRange(
		params TfFilterBase[] filters)
	{
		if (filters is null)
			throw new ArgumentNullException(nameof(filters));

		_filters.AddRange(filters);
	}

	public void Remove(
		TfFilterBase filter)
	{
		if (filter is null)
			throw new ArgumentNullException(nameof(filter));

		_filters.Remove(filter);
	}


}
