namespace WebVella.Tefter;

public class TfFilterAnd : TfFilterBase
{
	[JsonPropertyName("_filters")]
	private readonly List<TfFilterBase> _filters;

	[JsonIgnore]
	public ReadOnlyCollection<TfFilterBase> Filters => _filters.AsReadOnly();

	public TfFilterAnd()
		: base(string.Empty)
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
