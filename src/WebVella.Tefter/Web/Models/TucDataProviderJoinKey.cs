﻿namespace WebVella.Tefter.Web.Models;

public record TucDataProviderJoinKey
{
	public Guid Id { get; init; }
	public Guid DataProviderId { get; init; }
	public string DbName { get; init; }
	public string Description { get; init; }
	public List<TucDataProviderColumn> Columns { get; init; } = new();
	public short Version { get; init; }
	public DateTime LastModifiedOn { get; init; }
	
	public TucDataProviderJoinKey() { }
	public TucDataProviderJoinKey(TfDataProviderJoinKey model)
	{
		Id = model.Id;
		DataProviderId = model.DataProviderId;
		DbName = model.DbName;
		Description = model.Description;
		Columns = model.Columns is null ? null : model.Columns.Select(x=> new TucDataProviderColumn(x)).ToList();
		Version = model.Version;
		LastModifiedOn = model.LastModifiedOn;

	}
	public TfDataProviderJoinKey ToModel()
	{
		return new TfDataProviderJoinKey
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
