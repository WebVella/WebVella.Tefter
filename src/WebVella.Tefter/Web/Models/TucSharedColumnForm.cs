namespace WebVella.Tefter.Web.Models;

public record TucSharedColumnForm
{
	public Guid Id { get; set; }
	public string JoinKeyDbName { get; set; }
	public string DbName { get; set; }
	public TucDatabaseColumnTypeInfo DbType { get; set; }
	public bool IncludeInTableSearch { get; set; }
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
