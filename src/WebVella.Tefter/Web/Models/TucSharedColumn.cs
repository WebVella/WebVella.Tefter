namespace WebVella.Tefter.Web.Models;

public record TucSharedColumn
{
	public Guid Id { get; init; }
	public string JoinKeyDbName { get; init; }
	public string DbName { get; init; }
	public TucDatabaseColumnTypeInfo DbType { get; init; }
	public bool IncludeInTableSearch { get; init; }
	public TucSharedColumn() { }
	public TucSharedColumn(TfSharedColumn model)
	{
		Id = model.Id;
		JoinKeyDbName = model.JoinKeyDbName;
		DbName = model.DbName;
		DbType = new TucDatabaseColumnTypeInfo(model.DbType);
		IncludeInTableSearch = model.IncludeInTableSearch;
	}

	public TfSharedColumn ToModel()
	{
		return new TfSharedColumn
		{
			Id = Id,
			JoinKeyDbName = JoinKeyDbName,
			DbName = DbName,
			DbType = DbType.TypeValue,
			IncludeInTableSearch = IncludeInTableSearch,
		};
	}

}
