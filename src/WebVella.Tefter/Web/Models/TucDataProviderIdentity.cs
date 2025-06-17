namespace WebVella.Tefter.Web.Models;

public record TucDataProviderIdentity
{
	public Guid Id { get; internal set; }
	public Guid DataProviderId { get; internal set; }
	public string Name { get; set; }
	public bool IsSystem { get; set; }
	public List<string> Columns { get; internal set; } = new();

	public TucDataProviderIdentity() { }
	public TucDataProviderIdentity(TfDataProviderIdentity model)
	{
		Id = model.Id;
		DataProviderId = model.DataProviderId;
		Name = model.DataIdentity;
		IsSystem = model.IsSystem;
		Columns = model.Columns;
	}

	public TucDataProviderIdentity(TucDataProviderIdentity model)
	{
		Id = model.Id;
		DataProviderId = model.DataProviderId;
		Name = model.Name;
		IsSystem = model.IsSystem;
		Columns = model.Columns;
	}

	public TfDataProviderIdentity ToModel()
	{
		return new TfDataProviderIdentity
		{
			Id = Id,
			DataProviderId = DataProviderId,
			DataIdentity = Name,
			Columns = Columns
		};
	}
}
