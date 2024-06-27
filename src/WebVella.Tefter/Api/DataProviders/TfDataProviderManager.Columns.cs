namespace WebVella.Tefter;

public partial interface ITfDataProviderManager
{
	internal TfDataProviderColumn GetDataProviderColumn(Guid id);

	internal List<TfDataProviderColumn> GetDataProviderColumns(Guid providerId);

	public Result<TfDataProvider> CreateDataProviderColumn(TfDataProviderColumn column);

	public Result<TfDataProvider> UpdateDataProviderColumn(TfDataProviderColumn column);

	public Result<TfDataProvider> DeleteDataProviderColumn(Guid id);
}

public partial class TfDataProviderManager : ITfDataProviderManager
{
	public TfDataProviderColumn GetDataProviderColumn(Guid id)
	{
		return _dboManager.Get<TfDataProviderColumn>(id);
	}

	public List<TfDataProviderColumn> GetDataProviderColumns(Guid providerId)
	{
		var orderSettings = new OrderSettings(nameof(TfDataProviderColumn.CreatedOn), OrderDirection.ASC);
		return _dboManager.GetList<TfDataProviderColumn>(providerId, nameof(TfDataProviderColumn.DataProviderId), order: orderSettings);
	}

	public Result<TfDataProvider> CreateDataProviderColumn(TfDataProviderColumn column)
	{
		try
		{
			if (column.Id == Guid.Empty)
				column.Id = Guid.NewGuid();

			TfDataProviderColumnValidator validator =
				new TfDataProviderColumnValidator(_dboManager, this);

			var validationResult = validator.ValidateCreate(column);

			if (!validationResult.IsValid)
				return validationResult.ToResult();

			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{

				var success = _dboManager.Insert<TfDataProviderColumn>(column);

				if (!success)
					return Result.Fail(new DboManagerError("Insert", column));

				var providerResult = GetProvider(column.DataProviderId);

				if (providerResult.IsFailed)
					return Result.Fail(new Error("Failed to create new data provider column")
						.CausedBy(providerResult.Errors));

				var provider = providerResult.Value;

				var result = CreateDatabaseColumn(provider, column);

				if (result.IsFailed)
					return Result.Fail(new Error("Failed to create new data provider column.")
						.CausedBy(result.Errors));

				scope.Complete();

				return Result.Ok(provider);
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to create new data provider column.").CausedBy(ex));
		}
	}

	public Result<TfDataProvider> UpdateDataProviderColumn(TfDataProviderColumn column)
	{
		try
		{
			TfDataProviderColumnValidator validator =
				new TfDataProviderColumnValidator(_dboManager, this);

			var validationResult = validator.ValidateUpdate(column);

			if (!validationResult.IsValid)
				return validationResult.ToResult();

			var existingColumn = _dboManager.Get<TfDataProviderColumn>(column.Id);

			var success = _dboManager.Update<TfDataProviderColumn>(column);

			if (!success)
				return Result.Fail(new DboManagerError("Update", column));

			var providerResult = GetProvider(column.DataProviderId);
			
			if (providerResult.IsFailed)
				return Result.Fail(new Error("Failed to update data provider column")
					.CausedBy(providerResult.Errors));

			var provider = providerResult.Value;

			UpdateDatabaseColumn(provider, column, existingColumn);

			return Result.Ok(GetProvider(column.DataProviderId).Value);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to update data provider column.").CausedBy(ex));
		}
	}

	public Result<TfDataProvider> DeleteDataProviderColumn(Guid id)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				TfDataProviderColumnValidator validator =
				new TfDataProviderColumnValidator(_dboManager, this);

				var column = GetDataProviderColumn(id);

				var validationResult = validator.ValidateDelete(column);
				
				if (!validationResult.IsValid)
					return validationResult.ToResult();

				var success = _dboManager.Delete<TfDataProviderColumn>(id);

				if (!success)
					return Result.Fail(new DboManagerError("Delete", id));

				var providerResult = GetProvider(column.DataProviderId);
				
				if (providerResult.IsFailed)
					return Result.Fail(new Error("Failed to delete provider column.")
						.CausedBy(providerResult.Errors));

				var provider = providerResult.Value;
				
				string providerTableName = $"dp{provider.Index}";

				DatabaseBuilder dbBuilder = _dbManager.GetDatabaseBuilder();

				dbBuilder.WithTableBuilder(providerTableName).WithColumns(columns => columns.Remove(column.DbName));

				_dbManager.SaveChanges(dbBuilder);

				scope.Complete();

				return Result.Ok(GetProvider(column.DataProviderId).Value);
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to delete data provider column.").CausedBy(ex));
		}
	}

	private Result CreateDatabaseColumn(TfDataProvider provider, TfDataProviderColumn column)
	{
		try
		{
			string providerTableName = $"dp{provider.Index}";

			DatabaseBuilder dbBuilder = _dbManager.GetDatabaseBuilder();
			var tableBuilder = dbBuilder.WithTableBuilder(providerTableName);
			var columnsBuilder = tableBuilder.WithColumnsBuilder();

			switch (column.DbType)
			{
				case DatabaseColumnType.Text:
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
				case DatabaseColumnType.ShortText:
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
				case DatabaseColumnType.Guid:
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
				case DatabaseColumnType.Date:
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
				case DatabaseColumnType.DateTime:
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
				case DatabaseColumnType.Number:
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
				case DatabaseColumnType.ShortInteger:
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
				case DatabaseColumnType.Integer:
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
				case DatabaseColumnType.LongInteger:
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

			_dbManager.SaveChanges(dbBuilder);

			return Result.Ok();
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to create database column.").CausedBy(ex));
		}
	}

	private Result UpdateDatabaseColumn(TfDataProvider provider, TfDataProviderColumn column, TfDataProviderColumn existingColumn)
	{
		try
		{
			string providerTableName = $"dp{provider.Index}";

			DatabaseBuilder dbBuilder = _dbManager.GetDatabaseBuilder();
			var tableBuilder = dbBuilder.WithTableBuilder(providerTableName);
			var columnsBuilder = tableBuilder.WithColumnsBuilder();

			switch (column.DbType)
			{
				case DatabaseColumnType.Text:
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
				case DatabaseColumnType.ShortText:
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
				case DatabaseColumnType.Guid:
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
				case DatabaseColumnType.Date:
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
				case DatabaseColumnType.DateTime:
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
				case DatabaseColumnType.Number:
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
				case DatabaseColumnType.ShortInteger:
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
				case DatabaseColumnType.Integer:
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
				case DatabaseColumnType.LongInteger:
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

			_dbManager.SaveChanges(dbBuilder);

			return Result.Ok();
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to create database column.").CausedBy(ex));
		}
	}

	#region <--- Validator --->

	internal class TfDataProviderColumnValidator
	: AbstractValidator<TfDataProviderColumn>
	{
		private readonly IDboManager _dboManager;
		private readonly ITfDataProviderManager _providerManager;

		public TfDataProviderColumnValidator(
			IDboManager dboManager,
			ITfDataProviderManager providerManager)
		{
			_dboManager = dboManager;
			_providerManager = providerManager;

			RuleSet("general", () =>
			{
				RuleFor(column => column.Id)
					.NotEmpty()
					.WithMessage("The data provider column id is required.");

				RuleFor(column => column.DataProviderId)
					.NotEmpty()
					.WithMessage("The data provider id is required.");

				RuleFor(provider => provider.DataProviderId)
					.Must(providerId => { return providerManager.GetProvider(providerId).Value != null; })
					.WithMessage("There is no existing data provider for specified provider id.");

				RuleFor(column => column.SourceName)
					.NotEmpty()
					.WithMessage("The data provider column source name is required.");

				RuleFor(column => column.SourceType)
					.NotEmpty()
					.WithMessage("The data provider column source type is required.");

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

						Match match = Regex.Match(dbName, Constants.DB_OBJECT_NAME_VALIDATION_PATTERN);
						return match.Success && match.Value == dbName.Trim();
					})
					.WithMessage($"Name can only contains underscores and lowercase alphanumeric characters. It must begin with a letter, " +
						$"not include spaces, not end with an underscore, and not contain two consecutive underscores");

				RuleFor(column => column.DefaultValue)
					.Must((column, defaultValue) =>
					{
						if (string.IsNullOrWhiteSpace(defaultValue))
							return true;

						try
						{
							switch (column.DbType)
							{
								case DatabaseColumnType.Text:
								case DatabaseColumnType.ShortText:
									break;
								case DatabaseColumnType.Guid:
									{
										if (column.AutoDefaultValue == false && column.DefaultValue is not null)
										{
											var guid = Guid.Parse(column.DefaultValue);
										}
									}
									break;
								case DatabaseColumnType.Date:
									{
										if (column.AutoDefaultValue == false && column.DefaultValue is not null)
										{
											var date = DateOnly.Parse(column.DefaultValue, CultureInfo.InvariantCulture);
										}
									}
									break;
								case DatabaseColumnType.DateTime:
									{
										if (column.AutoDefaultValue == false && column.DefaultValue is not null)
										{
											var datetime = DateTime.Parse(column.DefaultValue, CultureInfo.InvariantCulture);
										}
									}
									break;
								case DatabaseColumnType.Number:
									{
										if (column.DefaultValue is not null)
										{
											var number = Convert.ToDecimal(column.DefaultValue, CultureInfo.InvariantCulture);
										}
									}
									break;
								case DatabaseColumnType.ShortInteger:
									{
										if (column.DefaultValue is not null)
										{
											short number = Convert.ToInt16(column.DefaultValue);
										}

									}
									break;
								case DatabaseColumnType.Integer:
									{
										if (column.DefaultValue is not null)
										{
											int number = Convert.ToInt32(column.DefaultValue);
										}

									}
									break;
								case DatabaseColumnType.LongInteger:
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
						catch (Exception ex)
						{
							return false;
						}
					})
					.WithMessage($"Specified default value is not correct for selected database column type.");

			});

			RuleSet("create", () =>
			{
				RuleFor(column => column.Id)
						.Must((column, id) => { return providerManager.GetDataProviderColumn(id) == null; })
						.WithMessage("There is already existing data provider column with specified identifier.");

				//RuleFor(column => column.SourceName)
				//		.Must( (column,sourceName) => {
				//			if (string.IsNullOrEmpty(sourceName))
				//				return true;

				//			var columns = providerManager.GetDataProviderColumns(column.DataProviderId);
				//			return !columns.Any(x => x.SourceName.ToLowerInvariant()?.Trim() == sourceName.ToLowerInvariant().Trim());
				//		})
				//		.WithMessage("There is already existing data provider column with specified source name.");

				RuleFor(column => column.DbName)
						.Must((column, dbName) =>
						{
							if (string.IsNullOrEmpty(dbName))
								return true;

							var columns = providerManager.GetDataProviderColumns(column.DataProviderId);
							return !columns.Any(x => x.DbName.ToLowerInvariant().Trim() == dbName.ToLowerInvariant().Trim());
						})
						.WithMessage("There is already existing data provider column with specified database name.");
			});

			RuleSet("update", () =>
			{
				RuleFor(column => column.Id)
						.Must((column, id) => { return providerManager.GetDataProviderColumn(id) != null; })
						.WithMessage("There is not existing data provider column with specified identifier.");

				RuleFor(column => column.DataProviderId)
						.Must((column, providerId) =>
						{

							var existingColumn = providerManager.GetDataProviderColumn(column.Id);
							if (existingColumn is null)
								return true;

							return existingColumn.DataProviderId == providerId;
						})
						.WithMessage("There data provider cannot be changed for data provider column.");

				RuleFor(column => column.DbName)
						.Must((column, dbName) =>
						{

							var existingColumn = providerManager.GetDataProviderColumn(column.Id);
							if (existingColumn is null)
								return true;

							return existingColumn.DbName == dbName;
						})
						.WithMessage("There database name of column cannot be changed.");

				RuleFor(column => column.DbType)
					.Must((column, dbType) =>
					{

						var existingColumn = providerManager.GetDataProviderColumn(column.Id);
						if (existingColumn is null)
							return true;

						return existingColumn.DbType == dbType;
					})
					.WithMessage("There database type of column cannot be changed.");

			});


			RuleSet("delete", () =>
			{
				// Add more check when available

			});

		}

		public ValidationResult ValidateCreate(TfDataProviderColumn column)
		{
			if (column == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The data provider column is null.") });

			return this.Validate(column, options =>
			{
				options.IncludeRuleSets("general", "create");
			});
		}

		public ValidationResult ValidateUpdate(TfDataProviderColumn column)
		{
			if (column == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The data provider column is null.") });

			return this.Validate(column, options =>
			{
				options.IncludeRuleSets("general", "update");
			});
		}

		public ValidationResult ValidateDelete(TfDataProviderColumn column)
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
