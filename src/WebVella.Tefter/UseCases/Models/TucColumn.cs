namespace WebVella.Tefter.UseCases.Models;

public record TucColumn
{
	public Guid Id { get; init; }
	public string DbName { get; init; }
	public TucDatabaseColumnTypeInfo DbType { get; init; }

	public bool IsShared { get; init; }

	public TucColumn() { }
	public TucColumn(TucDataProviderColumn model)
	{
		Id = model.Id;
		DbName = model.DbName;
		DbType = model.DbType;
		IsShared = false;
	}
	public TucColumn(TucSharedColumn model)
	{
		Id = model.Id;
		DbName = model.DbName;
		DbType = model.DbType;
		IsShared = true;
	}

	public TucColumn(TucDataProviderSystemColumn model)
	{
		Id = Guid.Empty;
		DbName = model.DbName;
		DbType = model.DbType;
		IsShared = true;
	}
	
}
