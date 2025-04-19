namespace WebVella.Tefter.Web.Models;

public record TucDataProviderJoinKeyForm
{
	public Guid Id { get; set; }
	public Guid DataProviderId { get; set; }
	public string DbName { get; set; }
	public string Description { get; set; }
	public List<TucDataProviderColumn> Columns { get; set; } = new();
	public short Version { get; set; }
	public DateTime LastModifiedOn { get; set; }

	public TucDataProviderJoinKeyForm() { }
	public TucDataProviderJoinKeyForm(TucDataProviderJoinKey model)
	{
		Id = model.Id;
		DbName = model.DbName;
		Description = model.Description;
		Columns = model.Columns;
		DataProviderId = model.DataProviderId;
		LastModifiedOn = model.LastModifiedOn;
		Version = model.Version;
	}
	public TfDataProviderJoinKey ToModel()
	{
		var model = new TfDataProviderJoinKey()
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
