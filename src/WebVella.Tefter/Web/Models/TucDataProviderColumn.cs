namespace WebVella.Tefter.Web.Models;

public record TucDataProviderColumn
{
	public Guid Id { get; set; }
	public Guid DataProviderId { get; set; }
	public string SourceName { get; set; }
	public string SourceType { get; set; }
	public DateTime CreatedOn { get; set; }
	public string DbName { get; set; }
	public TfDatabaseColumnType DbType { get; set; }
	public string DefaultValue { get; set; }
	public bool AutoDefaultValue { get; set; }
	public bool IsNullable { get; set; }
	public bool IsUnique { get; set; }
	public bool IsSortable { get; set; }
	public bool IsSearchable { get; set; }
	public TfDataProviderColumnSearchType PreferredSearchType { get; set; }
	public bool IncludeInTableSearch { get; set; }
	
	public TucDataProviderColumn() { }
	public TucDataProviderColumn(TfDataProviderColumn model)
	{
		Id = model.Id;
		DataProviderId = model.DataProviderId;
		SourceName = model.SourceName;
		SourceType = model.SourceType;
		CreatedOn = model.CreatedOn;
		DbName = model.DbName;
		DbType = model.DbType;
		DefaultValue = model.DefaultValue;
		AutoDefaultValue = model.AutoDefaultValue;
		IsNullable = model.IsNullable;
		IsUnique = model.IsUnique;
		IsSortable = model.IsSortable;
		IsSearchable = model.IsSearchable;
		PreferredSearchType = model.PreferredSearchType;
		IncludeInTableSearch = model.IncludeInTableSearch;
	}
	public TfDataProviderColumn ToModel()
	{
		return new TfDataProviderColumn
		{
			Id = Id,
			DataProviderId = DataProviderId,
			SourceName = SourceName,
			SourceType = SourceType,
			CreatedOn = CreatedOn,
			DbName = DbName,
			DbType = DbType,
			DefaultValue = DefaultValue,
			AutoDefaultValue = AutoDefaultValue,
			IsNullable = IsNullable,
			IsUnique = IsUnique,
			IsSortable = IsSortable,
			IsSearchable = IsSearchable,
			PreferredSearchType = PreferredSearchType,
			IncludeInTableSearch= IncludeInTableSearch,
		};
	}

}
