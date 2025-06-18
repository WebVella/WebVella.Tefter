namespace WebVella.Tefter.Web.Models;

public record TucSharedColumnForm
{
	public Guid Id { get; set; }
	public string DataIdentity { get; set; } = TfConstants.TF_ROW_ID_DATA_IDENTITY;
	public string DbName { get; set; }
	public TucDatabaseColumnTypeInfo DbType { get; set; }
	public bool IncludeInTableSearch { get; set; }
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
