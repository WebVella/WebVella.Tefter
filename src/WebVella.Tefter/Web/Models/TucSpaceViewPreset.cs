namespace WebVella.Tefter.Web.Models;

public record TucSpaceViewPreset
{
	[JsonPropertyName("id")]
	public Guid Id { get; set; } //if there is a preset with the view Id, this should be used for the view
	
	[JsonPropertyName("name")]
	public string Name { get; set; }

	[JsonPropertyName("parent_id")]
	public Guid? ParentId { get; set; }

	[JsonPropertyName("filters")]
	public List<TucFilterBase> Filters { get; set; } = new();

	[JsonPropertyName("sort_orders")]
	public List<TucSort> SortOrders { get; set; } = new();

	[JsonPropertyName("nodes")]
	public List<TucSpaceViewPreset> Nodes { get; set; } = new();

	[JsonPropertyName("is_group")]
	public bool IsGroup { get; set; } = false;


	public TucSpaceViewPreset() { }
	public TucSpaceViewPreset(TfSpaceViewPreset model)
	{
		Id = model.Id;
		Name = model.Name;
		Filters = model.Filters.Select(x=> TucFilterBase.FromModel(x)).ToList();
		SortOrders = model.SortOrders.Select(x=> new TucSort(x)).ToList();
		Nodes = model.Nodes.Select(x=> new  TucSpaceViewPreset(x)).ToList();
		IsGroup = model.IsGroup;
		ParentId = model.ParentId;

	}
	public TfSpaceViewPreset ToModel()
	{
		return new TfSpaceViewPreset
		{
			Id = Id,
			Name = Name,
			Filters = Filters.Select(x=> TucFilterBase.ToModel(x)).ToList(),
			SortOrders = SortOrders.Select(x=> x.ToModel()).ToList(),
			Nodes = Nodes.Select(x=> x.ToModel()).ToList(),
			IsGroup = IsGroup,
			ParentId = ParentId
		};
	}

}
