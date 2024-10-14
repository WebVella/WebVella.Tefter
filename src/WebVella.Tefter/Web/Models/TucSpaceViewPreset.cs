namespace WebVella.Tefter.Web.Models;

public record TucSpaceViewPreset
{
	[JsonPropertyName("id")]
	public Guid Id { get; set; } //if there is a preset with the view Id, this should be used for the view
	
	[JsonPropertyName("name")]
	public string Name { get; set; }

	[JsonPropertyName("filters")]
	public List<TfFilterBase> Filters { get; set; } = new();

	[JsonPropertyName("sort_orders")]
	public List<TfSort> SortOrders { get; set; } = new();

	[JsonPropertyName("nodes")]
	public List<TucSpaceViewPreset> Nodes { get; set; } = new();

	[JsonPropertyName("is_group")]
	public bool IsGroup { get; set; } = false;

	public TucSpaceViewPreset() { }
	public TucSpaceViewPreset(TfSpaceViewPreset model)
	{
		Id = model.Id;
		Name = model.Name;
		Filters = model.Filters;
		SortOrders = model.SortOrders;
		Nodes = model.Nodes.Select(x=> new  TucSpaceViewPreset(x)).ToList();
		IsGroup = model.IsGroup;

	}
	public TfSpaceViewPreset ToModel()
	{
		return new TfSpaceViewPreset
		{
			Id = Id,
			Name = Name,
			Filters = Filters,
			SortOrders = SortOrders,
			Nodes = Nodes.Select(x=> x.ToModel()).ToList(),
			IsGroup = IsGroup,
		};
	}

}
