namespace WebVella.Tefter.UseCases.Models;

public record TucDataProviderSharedKeyForm
{
	public Guid Id { get; set; }
	public Guid DataProviderId { get; set; }
	public string DbName { get; set; }
	public string Description { get; set; }
	public List<TucDataProviderColumn> Columns { get; set; } = new();
	public int Version { get; set; }
	public DateTime LastModifiedOn { get; set; }

	public TucDataProviderSharedKeyForm() { }
	public TucDataProviderSharedKeyForm(TucDataProviderSharedKey model)
	{
		Id = model.Id;
		DbName = model.DbName;
		Description = model.Description;
		Columns = model.Columns;
		DataProviderId = model.DataProviderId;
		LastModifiedOn = model.LastModifiedOn;
		Version = model.Version;
	}
	public TfDataProviderSharedKey ToModel()
	{
		var model = new TfDataProviderSharedKey()
		{
			Id = Id,
			DbName = DbName,
			Description = Description,
			Columns = Columns.Select(x=> x.ToModel()).ToList(),
			DataProviderId = DataProviderId,
			LastModifiedOn = LastModifiedOn,
			Version = Version
		};
		return model;
	}

}
