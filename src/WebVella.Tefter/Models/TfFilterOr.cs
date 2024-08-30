namespace WebVella.Tefter;

public class TfFilterOr : TfFilterBase
{
	[JsonPropertyName("_filters")]
	private readonly List<TfFilterBase> _filters;

	[JsonIgnore]
	public ReadOnlyCollection<TfFilterBase> Filters => _filters.AsReadOnly();

	public TfFilterOr() 
		: base(string.Empty)
	{
		_filters = new List<TfFilterBase>();
	}

	public TfFilterOr(
		params TfFilterBase[] filters) 
		: this()
	{
		if (filters is null)
			throw new ArgumentNullException(nameof(filters));

		_filters.AddRange(filters);
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
