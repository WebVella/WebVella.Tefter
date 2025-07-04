﻿namespace WebVella.Tefter.Models;

[DboCacheModel]
[TfDboModel("tf_shared_column")]
public record TfSharedColumn
{
	[TfDboModelProperty("id")]
	public Guid Id { get; set; }

	[TfDboModelProperty("data_identity")]
	public string DataIdentity { get; set; }

	[TfDboModelProperty("db_name")]
	public string DbName { get; set; }

	[TfDboTypeConverter(typeof(TfEnumPropertyConverter<TfDatabaseColumnType>))]
	[TfDboModelProperty("db_type")]
	public TfDatabaseColumnType DbType { get; set; }

	[TfDboModelProperty("include_table_search")]
	public bool IncludeInTableSearch { get; set; }

	public void FixPrefix()
	{
		if (!DbName.StartsWith(TfConstants.TF_SHARED_COLUMN_PREFIX))
		{
			DbName = TfConstants.TF_SHARED_COLUMN_PREFIX + DbName;
		}
	}
}
