namespace WebVella.Tefter.UseCases.Models;

public record TucSharedColumnForm
{
	public Guid Id { get; set; }
	public string SharedKeyDbName { get; set; }
	public string DbName { get; set; }
	public TucDatabaseColumnTypeInfo DbType { get; set; }
	public bool IncludeInTableSearch { get; set; }
	public Guid? AddonId { get; set; }

	public TfSharedColumn ToModel()
	{
		return new TfSharedColumn
		{
			Id = Id,
			SharedKeyDbName = SharedKeyDbName,
			DbName = DbName,
			DbType = DbType.ToModel(),
			IncludeInTableSearch = IncludeInTableSearch,
			AddonId = AddonId
		};
	}
}
