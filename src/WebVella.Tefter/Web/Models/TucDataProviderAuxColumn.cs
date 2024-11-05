namespace WebVella.Tefter.Web.Models;

public record TucDataProviderAuxColumn
{
	public Guid Id { get; init; }
	public Guid DataProviderId { get; init; }
	public string SourceName { get; init; }
	public string SourceType { get; init; }
	public DateTime CreatedOn { get; init; }
	public string DbName { get; init; }
	public TucDatabaseColumnTypeInfo DbType { get; init; }
	public string DefaultValue { get; init; }
	public bool AutoDefaultValue { get; init; }
	public bool IsNullable { get; init; }
	public bool IsUnique { get; init; }
	public bool IsSortable { get; init; }
	public bool IsSearchable { get; init; }
	public TucDataProviderColumnSearchTypeInfo PreferredSearchType { get; init; }
	public bool IncludeInTableSearch { get; init; }
	
	public TucDataProviderAuxColumn() { }
	public TucDataProviderAuxColumn(TfDataProviderColumn model)
	{
		Id = model.Id;
		DataProviderId = model.DataProviderId;
		SourceName = model.SourceName;
		SourceType = model.SourceType;
		CreatedOn = model.CreatedOn;
		DbName = model.DbName;
		DbType = new TucDatabaseColumnTypeInfo(model.DbType);
		DefaultValue = model.DefaultValue;
		AutoDefaultValue = model.AutoDefaultValue;
		IsNullable = model.IsNullable;
		IsUnique = model.IsUnique;
		IsSortable = model.IsSortable;
		IsSearchable = model.IsSearchable;
		PreferredSearchType = new TucDataProviderColumnSearchTypeInfo(model.PreferredSearchType);
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
			DbType = DbType.TypeValue.ConvertSafeToEnum<TucDatabaseColumnType,TfDatabaseColumnType>(),
			DefaultValue = DefaultValue,
			AutoDefaultValue = AutoDefaultValue,
			IsNullable = IsNullable,
			IsUnique = IsUnique,
			IsSortable = IsSortable,
			IsSearchable = IsSearchable,
			PreferredSearchType = PreferredSearchType.TypeValue.ConvertSafeToEnum<TucDataProviderColumnSearchType,TfDataProviderColumnSearchType>(),
			IncludeInTableSearch= IncludeInTableSearch,
		};
	}

}
