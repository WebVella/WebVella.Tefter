namespace WebVella.Tefter.UseCases.Models;

public record TucDataProviderSharedKey
{
	public Guid Id { get; init; }
	public Guid DataProviderId { get; init; }
	public string DbName { get; init; }
	public string Description { get; init; }
	public List<TucDataProviderColumn> Columns { get; init; } = new();
	public int Version { get; init; }
	public DateTime LastModifiedOn { get; init; }
	
	public TucDataProviderSharedKey() { }
	public TucDataProviderSharedKey(TfDataProviderSharedKey model)
	{
		Id = model.Id;
		DataProviderId = model.DataProviderId;
		DbName = model.DbName;
		Description = model.Description;
		Columns = model.Columns is null ? null : model.Columns.Select(x=> new TucDataProviderColumn(x)).ToList();
		Version = model.Version;
		LastModifiedOn = model.LastModifiedOn;

	}
	public TfDataProviderSharedKey ToModel()
	{
		return new TfDataProviderSharedKey
		{
			Id = Id,
			DataProviderId = DataProviderId,
			DbName = DbName,
			Description = Description,
			Columns = Columns is null ? null : Columns.Select(x=> x.ToModel()).ToList(),
			Version = Version,
			LastModifiedOn = LastModifiedOn,
		};
	}

}
