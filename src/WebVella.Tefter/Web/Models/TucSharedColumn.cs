namespace WebVella.Tefter.Web.Models;

public record TucSharedColumn
{
	public Guid Id { get; init; }
	public string DataIdentity { get; init; }
	public string DbName { get; init; }
	public TucDatabaseColumnTypeInfo DbType { get; init; }
	public bool IncludeInTableSearch { get; init; }
	public TucSharedColumn() { }
	public TucSharedColumn(TfSharedColumn model)
	{
		Id = model.Id;
		DataIdentity = model.DataIdentity;
		DbName = model.DbName;
		DbType = new TucDatabaseColumnTypeInfo(model.DbType);
		IncludeInTableSearch = model.IncludeInTableSearch;
	}

	public TfSharedColumn ToModel()
	{
		return new TfSharedColumn
		{
			Id = Id,
			DataIdentity = DataIdentity,
			DbName = DbName,
			DbType = DbType.TypeValue,
			IncludeInTableSearch = IncludeInTableSearch,
		};
	}

}
