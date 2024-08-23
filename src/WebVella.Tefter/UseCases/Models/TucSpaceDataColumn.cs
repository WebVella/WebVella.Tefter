namespace WebVella.Tefter.UseCases.Models;

public record TucSpaceDataColumn
{
	public string DbName { get; set; }
	public DatabaseColumnType DbType { get; set; }
	public bool Selected { get; set; } = false;

	public TucSpaceDataColumn() { }

	public TucSpaceDataColumn(TfSpaceDataColumn model)
	{
		DbName = model.DbName;
		DbType = model.DbType;
		Selected = model.Selected;
	}

	public TfSpaceDataColumn ToModel()
	{
		return new TfSpaceDataColumn
		{
			DbName = DbName,
			DbType = DbType,
			Selected = Selected
		};
	}
}
