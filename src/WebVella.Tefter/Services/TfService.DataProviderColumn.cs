namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	/// <summary>
	/// Gets data provider column instance for specified identifier
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	internal TfDataProviderColumn GetDataProviderColumn(
		Guid id);

	/// <summary>
	/// Gets list of data provider columns for specified provider identifier
	/// </summary>
	/// <param name="providerId"></param>
	/// <returns></returns>
	internal List<TfDataProviderColumn> GetDataProviderColumns(
		Guid providerId);

	/// <summary>
	/// Creates new data provider column
	/// </summary>
	/// <param name="column"></param>
	/// <returns></returns>
	public TfDataProvider CreateDataProviderColumn(
		TfDataProviderColumn column);


	/// <summary>
	/// Creates new data provider column
	/// </summary>
	/// <param name="column"></param>
	/// <returns></returns>
	public TfDataProvider CreateBulkDataProviderColumn(Guid providerId,
		List<TfDataProviderColumn> columns);

	/// <summary>
	/// Updates existing data provider column
	/// </summary>
	/// <param name="column"></param>
	/// <returns></returns>
	public TfDataProvider UpdateDataProviderColumn(
		TfDataProviderColumn column);

	/// <summary>
	/// Deletes existing data provider column
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public TfDataProvider DeleteDataProviderColumn(
		Guid id);

	ReadOnlyCollection<DatabaseColumnTypeInfo> GetDatabaseColumnTypeInfos();
}

public partial class TfService : ITfService
{
	/// <summary>
	/// Gets data provider column instance for specified identifier
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public TfDataProviderColumn GetDataProviderColumn(
		Guid id)
	{
		try
		{
			return _dboManager.Get<TfDataProviderColumn>(id);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	/// <summary>
	/// Gets list of data provider columns for specified provider identifier
	/// </summary>
	/// <param name="providerId"></param>
	/// <returns></returns>
	public List<TfDataProviderColumn> GetDataProviderColumns(
		Guid providerId)
	{
		try
		{
			var orderSettings = new TfOrderSettings(nameof(TfDataProviderColumn.CreatedOn), OrderDirection.ASC);
			return _dboManager.GetList<TfDataProviderColumn>(providerId,
				nameof(TfDataProviderColumn.DataProviderId), order: orderSettings);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	private List<TfDataProviderSystemColumn> GetDataProviderSystemColumns(
		List<TfDataProviderSharedKey> sharedKeys)
	{
		try
		{
			var systemColumns = new List<TfDataProviderSystemColumn>();

			systemColumns.Add(new TfDataProviderSystemColumn
			{
				DbName = "tf_id",
				DbType = TfDatabaseColumnType.Guid
			});

			systemColumns.Add(new TfDataProviderSystemColumn
			{
				DbName = "tf_row_index",
				DbType = TfDatabaseColumnType.Integer
			});

			systemColumns.Add(new TfDataProviderSystemColumn
			{
				DbName = "tf_created_on",
				DbType = TfDatabaseColumnType.DateTime
			});

			systemColumns.Add(new TfDataProviderSystemColumn
			{
				DbName = "tf_updated_on",
				DbType = TfDatabaseColumnType.DateTime
			});

			systemColumns.Add(new TfDataProviderSystemColumn
			{
				DbName = "tf_search",
				DbType = TfDatabaseColumnType.Text
			});

			foreach (var sharedKey in sharedKeys)
			{
				systemColumns.Add(new TfDataProviderSystemColumn
				{
					DbName = $"tf_sk_{sharedKey.DbName}_id",
					DbType = TfDatabaseColumnType.Guid
				});

				systemColumns.Add(new TfDataProviderSystemColumn
				{
					DbName = $"tf_sk_{sharedKey.DbName}_version",
					DbType = TfDatabaseColumnType.ShortInteger
				});
			}

			return systemColumns;
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	/// <summary>
	/// Creates new data provider column
	/// </summary>
	/// <param name="column"></param>
	/// <returns></returns>
	public TfDataProvider CreateDataProviderColumn(
		TfDataProviderColumn column)
	{
		try
		{
			if (column != null && column.Id == Guid.Empty)
				column.Id = Guid.NewGuid();

			new TfDataProviderColumnValidator(this)
				.ValidateCreate(column)
				.ToValidationException()
				.ThrowIfContainsErrors();

			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var success = _dboManager.Insert<TfDataProviderColumn>(column);
				if (!success)
					throw new TfDboServiceException("Insert<TfDataProviderColumn> failed.");

				var provider = GetDataProvider(column.DataProviderId);
				if (provider is null)
					throw new TfException("Failed to create new data provider column");

				CreateDatabaseColumn(provider, column);

				scope.Complete();

				return provider;
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}


	/// <summary>
	/// Creates new data provider columns in bulk
	/// </summary>
	/// <param name="column"></param>
	/// <returns></returns>
	public TfDataProvider CreateBulkDataProviderColumn(Guid providerId,
		List<TfDataProviderColumn> columns)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				List<ValidationError> validationErrors = new();
				foreach (var column in columns)
				{
					column.DataProviderId = providerId;
					CreateDataProviderColumn(column);
				}
				scope.Complete();

				var provider = GetDataProvider(providerId);
				if (provider is null)
					throw new TfException("Failed to create new data provider column");

				return provider;
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}


	/// <summary>
	/// Updates existing data provider column
	/// </summary>
	/// <param name="column"></param>
	/// <returns></returns>
	public TfDataProvider UpdateDataProviderColumn(
		TfDataProviderColumn column)
	{
		try
		{
			new TfDataProviderColumnValidator(this)
				.ValidateUpdate(column)
				.ToValidationException()
				.ThrowIfContainsErrors();

			var existingColumn = _dboManager.Get<TfDataProviderColumn>(column.Id);

			var success = _dboManager.Update<TfDataProviderColumn>(column);

			if (!success)
				throw new TfDboServiceException("Update<TfDataProviderColumn> failed.");

			var provider = GetDataProvider(column.DataProviderId);
			if (provider is null)
				throw new TfException("Failed to create new data provider column");

			UpdateDatabaseColumn(provider, column, existingColumn);

			return GetDataProvider(column.DataProviderId);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	/// <summary>
	/// Deletes existing data provider column
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public TfDataProvider DeleteDataProviderColumn(
		Guid id)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var column = GetDataProviderColumn(id);

				new TfDataProviderColumnValidator(this)
					.ValidateDelete(column)
					.ToValidationException()
					.ThrowIfContainsErrors();

				var success = _dboManager.Delete<TfDataProviderColumn>(id);

				if (!success)
					throw new TfDboServiceException("Delete<TfDataProviderColumn> failed");

				var provider = GetDataProvider(column.DataProviderId);
				if (provider is null)
					throw new TfException("Failed to create new data provider column");

				string providerTableName = $"dp{provider.Index}";

				TfDatabaseBuilder dbBuilder = _dbManager.GetDatabaseBuilder();

				dbBuilder.WithTableBuilder(providerTableName).WithColumns(columns => columns.Remove(column.DbName));

				_dbManager.SaveChanges(dbBuilder);

				scope.Complete();

				return GetDataProvider(column.DataProviderId);
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	#region <--- utility --->

	private void CreateDatabaseColumn(
		TfDataProvider provider,
		TfDataProviderColumn column)
	{
		string providerTableName = $"dp{provider.Index}";

		TfDatabaseBuilder dbBuilder = _dbManager.GetDatabaseBuilder();
		var tableBuilder = dbBuilder.WithTableBuilder(providerTableName);
		var columnsBuilder = tableBuilder.WithColumnsBuilder();

		switch (column.DbType)
		{
			case TfDatabaseColumnType.Boolean:
				{
					columnsBuilder.AddBooleanColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();

						if (column.DefaultValue is not null)
							c.WithDefaultValue(Convert.ToBoolean(column.DefaultValue));
					});

					if (column.IsSearchable || column.IsSortable)
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddBTreeIndex($"ix_{providerTableName}_{column.DbName}", c => { c.WithColumns(column.DbName); });
						});
					}

				}
				break;
			case TfDatabaseColumnType.Text:
				{
					columnsBuilder.AddTextColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();
						c.WithDefaultValue(column.DefaultValue);
					});

					if (column.IsSearchable || column.IsSortable)
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddGinIndexBuilder($"ix_{providerTableName}_{column.DbName}", c => { c.WithColumns(column.DbName); });
						});
					}
				}
				break;
			case TfDatabaseColumnType.ShortText:
				{
					columnsBuilder.AddShortTextColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();
						c.WithDefaultValue(column.DefaultValue);
					});

					if (column.IsSearchable || column.IsSortable)
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddBTreeIndexBuilder($"ix_{providerTableName}_{column.DbName}", c => { c.WithColumns(column.DbName); });
						});
					}
				}
				break;
			case TfDatabaseColumnType.Guid:
				{
					columnsBuilder.AddGuidColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();
						if (column.AutoDefaultValue)
							c.WithAutoDefaultValue();
						else
						{
							if (column.DefaultValue is not null)
								c.WithDefaultValue(new Guid(column.DefaultValue));
						}
					});

					if (column.IsSearchable || column.IsSortable)
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddBTreeIndex($"ix_{providerTableName}_{column.DbName}", c => { c.WithColumns(column.DbName); });
						});
					}
				}
				break;
			case TfDatabaseColumnType.Date:
				{
					columnsBuilder.AddDateColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();
						if (column.AutoDefaultValue)
							c.WithAutoDefaultValue();
						else
						{
							if (column.DefaultValue is not null)
							{
								var datetime = DateTime.Parse(column.DefaultValue, CultureInfo.InvariantCulture);
								DateOnly date = DateOnly.FromDateTime(datetime);
								c.WithDefaultValue(date);
							}
						}
					});

					if (column.IsSearchable || column.IsSortable)
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddBTreeIndex($"ix_{providerTableName}_{column.DbName}", c => { c.WithColumns(column.DbName); });
						});
					}
				}
				break;
			case TfDatabaseColumnType.DateTime:
				{
					columnsBuilder.AddDateTimeColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();
						if (column.AutoDefaultValue)
							c.WithAutoDefaultValue();
						else
						{
							if (column.DefaultValue is not null)
							{
								var datetime = DateTime.Parse(column.DefaultValue, CultureInfo.InvariantCulture);
								c.WithDefaultValue(datetime);
							}
						}
					});

					if (column.IsSearchable || column.IsSortable)
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddBTreeIndex($"ix_{providerTableName}_{column.DbName}", c => { c.WithColumns(column.DbName); });
						});
					}
				}
				break;
			case TfDatabaseColumnType.Number:
				{
					columnsBuilder.AddNumberColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();
						if (column.DefaultValue is not null)
						{
							var number = Convert.ToDecimal(column.DefaultValue, CultureInfo.InvariantCulture);
							c.WithDefaultValue(number);
						}
					});
					if (column.IsSearchable || column.IsSortable)
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddBTreeIndex($"ix_{providerTableName}_{column.DbName}", c => { c.WithColumns(column.DbName); });
						});
					}
				}
				break;
			case TfDatabaseColumnType.ShortInteger:
				{
					columnsBuilder.AddShortIntegerColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();
						if (column.DefaultValue is not null)
						{
							var number = short.Parse(column.DefaultValue);
							c.WithDefaultValue(number);
						}
					});
					if (column.IsSearchable || column.IsSortable)
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddBTreeIndex($"ix_{providerTableName}_{column.DbName}", c => { c.WithColumns(column.DbName); });
						});
					}
				}
				break;
			case TfDatabaseColumnType.Integer:
				{
					columnsBuilder.AddIntegerColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();
						if (column.DefaultValue is not null)
						{
							var number = int.Parse(column.DefaultValue);
							c.WithDefaultValue(number);
						}
					});
					if (column.IsSearchable || column.IsSortable)
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddBTreeIndex($"ix_{providerTableName}_{column.DbName}", c => { c.WithColumns(column.DbName); });
						});
					}
				}
				break;
			case TfDatabaseColumnType.LongInteger:
				{
					columnsBuilder.AddLongIntegerColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();
						if (column.DefaultValue is not null)
						{
							var number = long.Parse(column.DefaultValue);
							c.WithDefaultValue(number);
						}
					});
					if (column.IsSearchable || column.IsSortable)
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddBTreeIndex($"ix_{providerTableName}_{column.DbName}", c => { c.WithColumns(column.DbName); });
						});
					}
				}
				break;
			default:
				throw new Exception("Not supported database column type");
		}

		if (column.IsUnique)
		{
			tableBuilder.WithConstraints(constraints =>
			{
				constraints.AddUniqueKeyConstraintBuilder($"ux_{providerTableName}_{column.DbName}",
					c => { c.WithColumns(column.DbName); });
			});
		}

		var result = _dbManager.SaveChanges(dbBuilder);
		if (!result.IsSuccess)
			throw new TfException("Failed to save changes to database schema");
	}

	private void UpdateDatabaseColumn(
		TfDataProvider provider,
		TfDataProviderColumn column,
		TfDataProviderColumn existingColumn)
	{
		string providerTableName = $"dp{provider.Index}";

		TfDatabaseBuilder dbBuilder = _dbManager.GetDatabaseBuilder();
		var tableBuilder = dbBuilder.WithTableBuilder(providerTableName);
		var columnsBuilder = tableBuilder.WithColumnsBuilder();

		switch (column.DbType)
		{
			case TfDatabaseColumnType.Boolean:
				{
					columnsBuilder.WithBooleanColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();

						if (existingColumn.DefaultValue != column.DefaultValue && column.DefaultValue is not null)
						{
							c.WithDefaultValue(Convert.ToBoolean(column.DefaultValue));
						}
					});

					string indexName = $"ix_{providerTableName}_{column.DbName}";

					if ((!existingColumn.IsSearchable && !existingColumn.IsSortable) &&
						(column.IsSearchable || column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddGinIndexBuilder(indexName, c => { c.WithColumns(column.DbName); });
						});
					}

					if ((existingColumn.IsSearchable || existingColumn.IsSortable) &&
						(!column.IsSearchable && !column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes => { indexes.Remove(indexName); });
					}

				}
				break;
			case TfDatabaseColumnType.Text:
				{
					columnsBuilder.WithTextColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();

						if (existingColumn.DefaultValue != column.DefaultValue && column.DefaultValue is not null)
						{
							c.WithDefaultValue(column.DefaultValue);
						}
					});


					string indexName = $"ix_{providerTableName}_{column.DbName}";

					if ((!existingColumn.IsSearchable && !existingColumn.IsSortable) &&
						(column.IsSearchable || column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddGinIndexBuilder(indexName, c => { c.WithColumns(column.DbName); });
						});
					}

					if ((existingColumn.IsSearchable || existingColumn.IsSortable) &&
						(!column.IsSearchable && !column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes => { indexes.Remove(indexName); });
					}

				}
				break;
			case TfDatabaseColumnType.ShortText:
				{
					columnsBuilder.WithShortTextColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();

						if (existingColumn.DefaultValue != column.DefaultValue && column.DefaultValue is not null)
						{
							c.WithDefaultValue(column.DefaultValue);
						}
					});

					string indexName = $"ix_{providerTableName}_{column.DbName}";

					if ((!existingColumn.IsSearchable && !existingColumn.IsSortable) &&
						(column.IsSearchable || column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddBTreeIndexBuilder(indexName, c => { c.WithColumns(column.DbName); });
						});
					}

					if ((existingColumn.IsSearchable || existingColumn.IsSortable) &&
						(!column.IsSearchable && !column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes => { indexes.Remove(indexName); });
					}

				}
				break;
			case TfDatabaseColumnType.Guid:
				{
					columnsBuilder.WithGuidColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();
						if (column.AutoDefaultValue)
							c.WithAutoDefaultValue();
						else
						{
							if (column.DefaultValue is not null)
								c.WithDefaultValue(new Guid(column.DefaultValue));
						}
					});

					string indexName = $"ix_{providerTableName}_{column.DbName}";

					if ((!existingColumn.IsSearchable && !existingColumn.IsSortable) &&
						(column.IsSearchable || column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddBTreeIndexBuilder(indexName, c => { c.WithColumns(column.DbName); });
						});
					}

					if ((existingColumn.IsSearchable || existingColumn.IsSortable) &&
						(!column.IsSearchable && !column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes => { indexes.Remove(indexName); });
					}
				}
				break;
			case TfDatabaseColumnType.Date:
				{
					columnsBuilder.WithDateColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();
						if (column.AutoDefaultValue)
							c.WithAutoDefaultValue();
						else
						{
							if (column.DefaultValue is not null)
							{
								var datetime = DateTime.Parse(column.DefaultValue, CultureInfo.InvariantCulture);
								DateOnly date = DateOnly.FromDateTime(datetime);
								c.WithDefaultValue(date);
							}
						}
					});

					string indexName = $"ix_{providerTableName}_{column.DbName}";

					if ((!existingColumn.IsSearchable && !existingColumn.IsSortable) &&
						(column.IsSearchable || column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddBTreeIndexBuilder(indexName, c => { c.WithColumns(column.DbName); });
						});
					}

					if ((existingColumn.IsSearchable || existingColumn.IsSortable) &&
						(!column.IsSearchable && !column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes => { indexes.Remove(indexName); });
					}
				}
				break;
			case TfDatabaseColumnType.DateTime:
				{
					columnsBuilder.WithDateTimeColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();
						if (column.AutoDefaultValue)
							c.WithAutoDefaultValue();
						else
						{
							if (column.DefaultValue is not null)
							{
								var datetime = DateTime.Parse(column.DefaultValue, CultureInfo.InvariantCulture);
								c.WithDefaultValue(datetime);
							}
						}
					});

					string indexName = $"ix_{providerTableName}_{column.DbName}";

					if ((!existingColumn.IsSearchable && !existingColumn.IsSortable) &&
						(column.IsSearchable || column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddBTreeIndexBuilder(indexName, c => { c.WithColumns(column.DbName); });
						});
					}

					if ((existingColumn.IsSearchable || existingColumn.IsSortable) &&
						(!column.IsSearchable && !column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes => { indexes.Remove(indexName); });
					}
				}
				break;
			case TfDatabaseColumnType.Number:
				{
					columnsBuilder.WithNumberColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();
						if (column.DefaultValue is not null)
						{
							var number = Convert.ToDecimal(column.DefaultValue, CultureInfo.InvariantCulture);
							c.WithDefaultValue(number);
						}
					});

					string indexName = $"ix_{providerTableName}_{column.DbName}";

					if ((!existingColumn.IsSearchable && !existingColumn.IsSortable) &&
						(column.IsSearchable || column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddBTreeIndexBuilder(indexName, c => { c.WithColumns(column.DbName); });
						});
					}

					if ((existingColumn.IsSearchable || existingColumn.IsSortable) &&
						(!column.IsSearchable && !column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes => { indexes.Remove(indexName); });
					}
				}
				break;
			case TfDatabaseColumnType.ShortInteger:
				{
					columnsBuilder.WithShortIntegerColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();
						if (column.DefaultValue is not null)
						{
							var number = short.Parse(column.DefaultValue);
							c.WithDefaultValue(number);
						}
					});

					string indexName = $"ix_{providerTableName}_{column.DbName}";

					if ((!existingColumn.IsSearchable && !existingColumn.IsSortable) &&
						(column.IsSearchable || column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddBTreeIndexBuilder(indexName, c => { c.WithColumns(column.DbName); });
						});
					}

					if ((existingColumn.IsSearchable || existingColumn.IsSortable) &&
						(!column.IsSearchable && !column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes => { indexes.Remove(indexName); });
					}
				}
				break;
			case TfDatabaseColumnType.Integer:
				{
					columnsBuilder.WithIntegerColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();
						if (column.DefaultValue is not null)
						{
							var number = int.Parse(column.DefaultValue);
							c.WithDefaultValue(number);
						}
					});

					string indexName = $"ix_{providerTableName}_{column.DbName}";

					if ((!existingColumn.IsSearchable && !existingColumn.IsSortable) &&
						(column.IsSearchable || column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddBTreeIndexBuilder(indexName, c => { c.WithColumns(column.DbName); });
						});
					}

					if ((existingColumn.IsSearchable || existingColumn.IsSortable) &&
						(!column.IsSearchable && !column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes => { indexes.Remove(indexName); });
					}
				}
				break;
			case TfDatabaseColumnType.LongInteger:
				{
					columnsBuilder.WithLongIntegerColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();
						if (column.DefaultValue is not null)
						{
							var number = long.Parse(column.DefaultValue);
							c.WithDefaultValue(number);
						}
					});

					string indexName = $"ix_{providerTableName}_{column.DbName}";

					if ((!existingColumn.IsSearchable && !existingColumn.IsSortable) &&
						(column.IsSearchable || column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddBTreeIndexBuilder(indexName, c => { c.WithColumns(column.DbName); });
						});
					}

					if ((existingColumn.IsSearchable || existingColumn.IsSortable) &&
						(!column.IsSearchable && !column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes => { indexes.Remove(indexName); });
					}
				}
				break;
			default:
				throw new Exception("Not supported database column type");
		}

		if (column.IsUnique && !existingColumn.IsUnique)
		{
			tableBuilder.WithConstraints(constraints =>
			{
				constraints.AddUniqueKeyConstraintBuilder($"ux_{providerTableName}_{column.DbName}",
					c => { c.WithColumns(column.DbName); });
			});
		}

		if (!column.IsUnique && existingColumn.IsUnique)
		{
			tableBuilder.WithConstraints(constraints =>
			{
				constraints.Remove($"ux_{providerTableName}_{column.DbName}");
			});
		}

		var result = _dbManager.SaveChanges(dbBuilder);
		if (!result.IsSuccess)
			throw new TfException("Failed to save changes to database schema");
	}
	#endregion

	public static ReadOnlyCollection<DatabaseColumnTypeInfo> GetDatabaseColumnTypeInfosList()
	{

		List<DatabaseColumnTypeInfo> databaseColumnTypeInfos =
			new List<DatabaseColumnTypeInfo>();

		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "AUTO INCREMENT",
				Type = TfDatabaseColumnType.AutoIncrement,
				CanBeProviderDataType = false,
				SupportAutoDefaultValue = false
			});

		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "GUID",
				Type = TfDatabaseColumnType.Guid,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = true
			});

		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "DATE",
				Type = TfDatabaseColumnType.Date,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = true
			});

		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "DATE AND TIME",
				Type = TfDatabaseColumnType.DateTime,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = true
			});


		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "BOOLEAN",
				Type = TfDatabaseColumnType.Boolean,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = false
			});

		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "TEXT",
				Type = TfDatabaseColumnType.Text,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = false
			});

		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "SHORT TEXT",
				Type = TfDatabaseColumnType.ShortText,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = false
			});

		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "SHORT INTEGER (16bit)",
				Type = TfDatabaseColumnType.ShortInteger,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = false
			});

		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "INTEGER (32bit)",
				Type = TfDatabaseColumnType.Integer,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = false
			});

		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "LONG INTEGER (64bit)",
				Type = TfDatabaseColumnType.LongInteger,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = false
			});

		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "DECIMAL",
				Type = TfDatabaseColumnType.Number,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = false
			});

		return databaseColumnTypeInfos.AsReadOnly();
	}
	public ReadOnlyCollection<DatabaseColumnTypeInfo> GetDatabaseColumnTypeInfos()
	{
		try
		{
			return GetDatabaseColumnTypeInfosList();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	#region <--- validation --->

	internal class TfDataProviderColumnValidator
	: AbstractValidator<TfDataProviderColumn>
	{
		public TfDataProviderColumnValidator(
			ITfService tfService)
		{
			RuleSet("general", () =>
			{
				RuleFor(column => column.Id)
					.NotEmpty()
					.WithMessage("The data provider column id is required.");

				RuleFor(column => column.DataProviderId)
					.NotEmpty()
					.WithMessage("The data provider id is required.");

				RuleFor(column => column.DataProviderId)
					.Must(providerId =>
					{
						return tfService.GetDataProvider(providerId) != null;
					})
					.WithMessage("There is no existing data provider for specified provider id.");

				RuleFor(column => column.SourceType)
					.NotEmpty()
					.WithMessage("The data provider column source type is required.");

				RuleFor(column => column.SourceType)
					.Must((column, sourceType) =>
					{
						if (string.IsNullOrWhiteSpace(sourceType))
							return true;

						var provider = tfService.GetDataProvider(column.DataProviderId);
						if (provider is null)
							return true;

						var supportedSourceTypes = provider.ProviderType.GetSupportedSourceDataTypes();

						return supportedSourceTypes.Any(x => x == sourceType);

					})
					.WithMessage($"Selected source type is not in the list of provider supported source types.");

				RuleFor(column => column.SourceType)
					.Must((column, sourceType) =>
					{
						if (string.IsNullOrWhiteSpace(sourceType))
							return true;

						var provider = tfService.GetDataProvider(column.DataProviderId);
						if (provider is null)
							return true;

						var supportedDatabaseColumnTypes =
							provider.ProviderType.GetDatabaseColumnTypesForSourceDataType(sourceType);

						return supportedDatabaseColumnTypes.Any();

					})
					.WithMessage($"Selected source type does not provide any supported provider data type.");

				RuleFor(column => column.SourceType)
					.Must((column, sourceType) =>
					{
						if (string.IsNullOrWhiteSpace(sourceType))
							return true;

						var provider = tfService.GetDataProvider(column.DataProviderId);
						if (provider is null)
							return true;

						var supportedDatabaseColumnTypes =
							provider.ProviderType.GetDatabaseColumnTypesForSourceDataType(sourceType);

						return supportedDatabaseColumnTypes.Any(x => x == column.DbType);

					})
					.WithMessage($"The selected source type is not supported for use with selected provider data type.");

				RuleFor(column => column.DbName)
					.NotEmpty()
					.WithMessage("The data provider column database name is required.");

				RuleFor(column => column.DbName)
					.Must((column, dbName) =>
					{
						if (string.IsNullOrWhiteSpace(dbName))
							return true;

						return !dbName.StartsWith("tf_");
					})
					.WithMessage("The data provider column database name cannot start with 'tf_'.");

				RuleFor(column => column.DbName)
					.Must((column, dbName) =>
					{
						if (string.IsNullOrWhiteSpace(dbName))
							return true;

						return dbName.Length >= Constants.DB_MIN_OBJECT_NAME_LENGTH;
					})
					.WithMessage($"The database name must be at least {Constants.DB_MIN_OBJECT_NAME_LENGTH} characters long.");

				RuleFor(column => column.DbName)
					.Must((column, dbName) =>
					{
						if (string.IsNullOrWhiteSpace(dbName))
							return true;

						return dbName.Length <= Constants.DB_MAX_OBJECT_NAME_LENGTH;
					})
					.WithMessage($"The length of database name must be less or equal than {Constants.DB_MAX_OBJECT_NAME_LENGTH} characters");

				RuleFor(column => column.DbName)
					.Must((column, dbName) =>
					{
						if (string.IsNullOrWhiteSpace(dbName))
							return true;

						//other validation will trigger
						if (dbName.Length < Constants.DB_MIN_OBJECT_NAME_LENGTH)
							return true;

						//other validation will trigger
						if (dbName.Length > Constants.DB_MAX_OBJECT_NAME_LENGTH)
							return true;

						Match match = Regex.Match(dbName, Constants.DB_OBJECT_NAME_VALIDATION_PATTERN);
						return match.Success && match.Value == dbName.Trim();
					})
					.WithMessage($"Name can only contains underscores and lowercase alphanumeric characters. It must begin with a letter, " +
						$"not include spaces, not end with an underscore, and not contain two consecutive underscores");

				RuleFor(column => column.DefaultValue)
					.Must((column, defaultValue) =>
					{
						if (!column.IsNullable && defaultValue is null)
							return false;

						return true;
					})
					.WithMessage($"Column is marked as not nullable, but no default value is specified.");

				RuleFor(column => column.DefaultValue)
					.Must((column, defaultValue) =>
					{
						if (defaultValue == null)
							return true;

						try
						{
							switch (column.DbType)
							{
								case TfDatabaseColumnType.Boolean:
									{
										if (column.DefaultValue is not null)
										{
											var booleanValue = Convert.ToBoolean(column.DefaultValue);
										}
									}
									break;
								case TfDatabaseColumnType.Text:
								case TfDatabaseColumnType.ShortText:
									break;
								case TfDatabaseColumnType.Guid:
									{
										if (column.AutoDefaultValue == false && column.DefaultValue is not null)
										{
											var guid = Guid.Parse(column.DefaultValue);
										}
									}
									break;
								case TfDatabaseColumnType.Date:
									{
										if (column.AutoDefaultValue == false && column.DefaultValue is not null)
										{
											var date = DateOnly.Parse(column.DefaultValue, CultureInfo.InvariantCulture);
										}
									}
									break;
								case TfDatabaseColumnType.DateTime:
									{
										if (column.AutoDefaultValue == false && column.DefaultValue is not null)
										{
											var datetime = DateTime.Parse(column.DefaultValue, CultureInfo.InvariantCulture);
										}
									}
									break;
								case TfDatabaseColumnType.Number:
									{
										if (column.DefaultValue is not null)
										{
											var number = Convert.ToDecimal(column.DefaultValue, CultureInfo.InvariantCulture);
										}
									}
									break;
								case TfDatabaseColumnType.ShortInteger:
									{
										if (column.DefaultValue is not null)
										{
											short number = Convert.ToInt16(column.DefaultValue);
										}

									}
									break;
								case TfDatabaseColumnType.Integer:
									{
										if (column.DefaultValue is not null)
										{
											int number = Convert.ToInt32(column.DefaultValue);
										}

									}
									break;
								case TfDatabaseColumnType.LongInteger:
									{
										if (column.DefaultValue is not null)
										{
											long number = Convert.ToInt64(column.DefaultValue);
										}

									}
									break;
								default:
									throw new Exception("Not supported database column type while validate default value.");
							}
							return true;
						}
						catch
						{
							return false;
						}
					})
					.WithMessage($"Column is marked not nullable. Default value is required. Specified default value is empty or not correct for selected provider data type.");

			});

			RuleSet("create", () =>
			{
				RuleFor(column => column.Id)
						.Must((column, id) => { return tfService.GetDataProviderColumn(id) == null; })
						.WithMessage("There is already existing data provider column with specified identifier.");

				RuleFor(column => column.DbName)
						.Must((column, dbName) =>
						{
							if (string.IsNullOrEmpty(dbName))
								return true;

							var columns = tfService.GetDataProviderColumns(column.DataProviderId);
							return !columns.Any(x => x.DbName.ToLowerInvariant().Trim() == dbName.ToLowerInvariant().Trim());
						})
						.WithMessage("There is already existing data provider column with specified database name.");
			});

			RuleSet("update", () =>
			{
				RuleFor(column => column.Id)
						.Must((column, id) =>
						{
							return tfService.GetDataProviderColumn(id) != null;
						})
						.WithMessage("There is not existing data provider column with specified identifier.");

				RuleFor(column => column.DataProviderId)
						.Must((column, providerId) =>
						{

							var existingColumn = tfService.GetDataProviderColumn(column.Id);
							if (existingColumn is null)
								return true;

							return existingColumn.DataProviderId == providerId;
						})
						.WithMessage("There data provider cannot be changed for data provider column.");

				RuleFor(column => column.DbName)
						.Must((column, dbName) =>
						{

							var existingColumn = tfService.GetDataProviderColumn(column.Id);
							if (existingColumn is null)
								return true;

							return existingColumn.DbName == dbName;
						})
						.WithMessage("There database name of column cannot be changed.");

				RuleFor(column => column.DbType)
					.Must((column, dbType) =>
					{

						var existingColumn = tfService.GetDataProviderColumn(column.Id);
						if (existingColumn is null)
							return true;

						return existingColumn.DbType == dbType;
					})
					.WithMessage("There database type of column cannot be changed.");

			});

			RuleSet("delete", () =>
			{
				RuleFor(column => column.Id)
						.Must((column, id) =>
						{
							var sharedKeys = tfService.GetDataProviderSharedKeys(column.DataProviderId);
							var found = sharedKeys.Any(x => x.Columns.Any(c => c.Id == id));
							return !found;
						})
						.WithMessage("There data provider column cannot be deleted, because it is part of shared key.");
			});

		}

		public ValidationResult ValidateCreate(
			TfDataProviderColumn column)
		{
			if (column == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The data provider column is null.") });

			return this.Validate(column, options =>
			{
				options.IncludeRuleSets("general", "create");
			});
		}

		public ValidationResult ValidateUpdate(
			TfDataProviderColumn column)
		{
			if (column == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The data provider column is null.") });

			return this.Validate(column, options =>
			{
				options.IncludeRuleSets("general", "update");
			});
		}

		public ValidationResult ValidateDelete(
			TfDataProviderColumn column)
		{
			if (column == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The data provider column with specified identifier is not found.") });

			return this.Validate(column, options =>
			{
				options.IncludeRuleSets("delete");
			});
		}
	}

	#endregion
}