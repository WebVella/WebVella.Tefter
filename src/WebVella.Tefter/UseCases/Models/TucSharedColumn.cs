namespace WebVella.Tefter.UseCases.Models;

public record TucSharedColumn
{
	public Guid Id { get; init; }
	public string SharedKeyDbName { get; init; }
	public string DbName { get; init; }
	public TucDatabaseColumnTypeInfo DbType { get; init; }
	public bool IncludeInTableSearch { get; init; }
	public Guid? AddonId { get; init; }

	public TucSharedColumn() { }
	public TucSharedColumn(TfSharedColumn model)
	{
		Id = model.Id;
		SharedKeyDbName = model.SharedKeyDbName;
		DbName = model.DbName;
		DbType = new TucDatabaseColumnTypeInfo(model.DbType);
		IncludeInTableSearch = model.IncludeInTableSearch;
		AddonId = model.AddonId;
	}
	public TfSharedColumn ToModel()
	{
		return new TfSharedColumn
		{
			Id = Id,
			SharedKeyDbName = SharedKeyDbName,
			DbName = DbName,
			DbType = DbType.ToModel(),
			IncludeInTableSearch = IncludeInTableSearch,
			AddonId= AddonId
		};
	}

}
