namespace WebVella.Tefter.Web.Models;

public record TucSpaceViewPreset
{
	[JsonPropertyName("id")]
	public Guid Id { get; set; }
	
	[JsonPropertyName("name")]
	public string Name { get; set; }

	[JsonPropertyName("filters")]
	public List<TfFilterBase> Filters { get; set; } = new();

	[JsonPropertyName("sort_orders")]
	public List<TfSort> SortOrders { get; set; } = new();

	public TucSpaceViewPreset() { }
	public TucSpaceViewPreset(TfSpaceViewPreset model)
	{
		Id = model.Id;
		Name = model.Name;
		Filters = model.Filters;
		SortOrders = model.SortOrders;
	}
	public TfSpaceViewPreset ToModel()
	{
		return new TfSpaceViewPreset
		{
			Id = Id,
			Name = Name,
			Filters = Filters,
			SortOrders = SortOrders
		};
	}

}
