namespace WebVella.Tefter.UseCases.Models;

public record TucDataProviderColumnForm
{
	public Guid Id { get; set; }

	public Guid DataProviderId { get; set; }

	public string SourceName { get; set; }

	public string SourceType { get; set; }

	public DateTime CreatedOn { get; set; }

	public string DbName { get; set; }

	public TucDatabaseColumnTypeInfo DbType { get; set; }

	public string DefaultValue { get; set; }

	public bool AutoDefaultValue { get; set; }

	public bool IsNullable { get; set; }

	public bool IsUnique { get; set; }

	public bool IsSortable { get; set; }

	public bool IsSearchable { get; set; }

	public TucDataProviderColumnSearchTypeInfo PreferredSearchType { get; set; }

	public bool IncludeInTableSearch { get; set; }

	public TucDataProviderColumnForm(){ }
	public TucDataProviderColumnForm(TfDataProviderColumn model)
	{
			AutoDefaultValue = model.AutoDefaultValue;
			IsNullable = model.IsNullable;
			DbName = model.DbName;
			SourceName = model.SourceName;
			SourceType = model.SourceType;
			DefaultValue = model.DefaultValue;
			CreatedOn = model.CreatedOn;
			DataProviderId = model.DataProviderId;
			DbType = new TucDatabaseColumnTypeInfo(model.DbType);
			Id = model.Id;
			IncludeInTableSearch = model.IncludeInTableSearch;
			IsSearchable = model.IsSearchable;
			IsSortable = model.IsSortable;
			IsUnique = model.IsUnique;
			PreferredSearchType = new TucDataProviderColumnSearchTypeInfo(model.PreferredSearchType);
	}
	public TucDataProviderColumnForm(TucDataProviderColumn model)
	{
			AutoDefaultValue = model.AutoDefaultValue;
			IsNullable = model.IsNullable;
			DbName = model.DbName;
			SourceName = model.SourceName;
			SourceType = model.SourceType;
			DefaultValue = model.DefaultValue;
			CreatedOn = model.CreatedOn;
			DataProviderId = model.DataProviderId;
			DbType = model.DbType;
			Id = model.Id;
			IncludeInTableSearch = model.IncludeInTableSearch;
			IsSearchable = model.IsSearchable;
			IsSortable = model.IsSortable;
			IsUnique = model.IsUnique;
			PreferredSearchType = model.PreferredSearchType;
	}
	public TfDataProviderColumn ToModel()
	{
		var model = new TfDataProviderColumn()
		{
			Id = Id,
			AutoDefaultValue = AutoDefaultValue,
			IsNullable = IsNullable,
			PreferredSearchType = PreferredSearchType.TypeValue.ConvertSafeToEnum<TucDataProviderColumnSearchType,TfDataProviderColumnSearchType>(),
			IsUnique = IsUnique,
			IsSortable = IsSortable,
			IsSearchable = IsSearchable,
			IncludeInTableSearch = IncludeInTableSearch,
			CreatedOn = CreatedOn,
			DataProviderId = DataProviderId,
			DbName = DbName,
			DbType = DbType.TypeValue.ConvertSafeToEnum<TucDatabaseColumnType,DatabaseColumnType>(),
			DefaultValue = DefaultValue,
			SourceName = SourceName,
			SourceType = SourceType,
		};

		return model;
	}

}
