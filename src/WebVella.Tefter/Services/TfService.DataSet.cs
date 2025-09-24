using System.Text.Json.Serialization.Metadata;
using WebVella.Tefter.Models;

namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	public List<TfDataSet> GetAllDataSets(string? search = null);

	[Obsolete("SpaceId parameter is no longer used. Use GetAllDataSets(string? search = null) instead.", false)]
	public List<TfDataSet> GetDataSetList(
		Guid spaceId,
		string? search = null);

	public TfDataSet? GetDataSet(
		Guid id);

	public TfDataSet? CreateDataSet(
		TfCreateDataSet dataset);

	public TfDataSet? UpdateDataSet(
		TfUpdateDataSet dataset);

	public void DeleteDataSet(
		Guid id);

	public TfDataSet? CopyDataSet(
		Guid id);

	[Obsolete("This method is not used and not implemented")]
	public void MoveDataSetUp(
		Guid id);

	[Obsolete("This method is not used and not implemented")]
	public void MoveDataSetDown(
		Guid id);

	//Columns
	public List<TfAvailableDataSetColumn> GetDataSetAvailableColumns(
		Guid datasetId);

	List<TfDataSetColumn> GetDataSetColumns(
		Guid datasetId);

	List<TfDataSetColumn> GetDataSetColumnOptions(
		Guid datasetId);

	void AddDataSetColumn(
		Guid datasetId, 
		TfDataSetColumn column);
	
	void AddAvailableColumnsToDataSet(
		Guid datasetId);
	
	void RemoveDataSetColumn(
		Guid datasetId,
		TfDataSetColumn column);

	public void UpdateDataSetFilters(
		Guid datasetId,
		List<TfFilterBase> filters);
	
	public void UpdateDataSetSorts(
		Guid datasetId,
		List<TfSort> sorts);
}

public partial class TfService : ITfService
{
	public List<TfDataSet> GetAllDataSets(
		string? search = null)
	{
		try
		{
			var dbos = _dboManager.GetList<TfDataSetDbo>();

			var datasets = dbos.Select(x => ConvertDboToModel(x)).ToList();
			foreach (var dataset in datasets)
			{
				if(dataset == null)
					continue;

				dataset.Identities = new ReadOnlyCollection<TfDataSetIdentity>(
					GetDataSetIdentities(dataset.Id).ToList());
			}
			
			//disable nullability warning for the Linq below - we filter out nulls above
			#pragma warning disable CS8619

			if (String.IsNullOrWhiteSpace(search))
				return datasets;

			return datasets.Where(x =>
				x.Name.ToLowerInvariant().Contains(search.Trim().ToLowerInvariant())
				).ToList();

			#pragma warning restore CS8619

		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	[Obsolete("SpaceId parameter is no longer used. Use GetAllDataSets(string? search = null) instead.", false)]
	public List<TfDataSet> GetDataSetList(
		Guid spaceId,
		string? search = null)
	{
		try
		{
			throw new NotImplementedException();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfDataSet? GetDataSet(
		Guid id)
	{
		try
		{
			var dbo = _dboManager.Get<TfDataSetDbo>(id);
			if (dbo == null)
				return null;

			var dataset = ConvertDboToModel(dbo);
			
			if(dataset == null)
				return null;

			dataset.Identities = new ReadOnlyCollection<TfDataSetIdentity>(
					GetDataSetIdentities(dataset.Id).ToList());
			return dataset;
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public List<TfAvailableDataSetColumn> GetDataSetAvailableColumns(
		Guid datasetId)
	{
		try
		{
			List<TfAvailableDataSetColumn> columns = new List<TfAvailableDataSetColumn>();

			var dbo = _dboManager.Get<TfDataSetDbo>(datasetId);
			var dataset = ConvertDboToModel(dbo);

			if (dataset == null)
				return columns;

			var provider = GetDataProvider(dataset.DataProviderId);
			if (provider is null)
				throw new TfException("Not found specified data provider");

			foreach (var column in provider.Columns)
			{
				columns.Add(new TfAvailableDataSetColumn
				{
					DbName = column.DbName,
					DbType = column.DbType
				});
			}

			foreach (var identity in provider.Identities)
			{
				columns.Add(new TfAvailableDataSetColumn
				{
					DbName = identity.DataIdentity,
					DbType = TfDatabaseColumnType.ShortText,
				});
			}

			foreach (var column in provider.SharedColumns)
			{
				columns.Add(new TfAvailableDataSetColumn
				{
					DbName = column.DbName,
					DbType = column.DbType,
				});
			}

			return columns;
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfDataSet? CreateDataSet(
		TfCreateDataSet? dataset)
	{
		if (dataset is null) throw new ArgumentException("required", nameof(dataset));
		try
		{
			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var newDataSet = new TfDataSet();
				newDataSet.Id = dataset.Id;
				newDataSet.DataProviderId = dataset.DataProviderId;
				newDataSet.Name = dataset.Name;
				newDataSet.Filters = dataset.Filters ?? new List<TfFilterBase>();
				newDataSet.Columns = dataset.Columns ?? new List<string>();
				newDataSet.SortOrders = dataset.SortOrders ?? new List<TfSort>();

				if (newDataSet.Id == Guid.Empty)
					newDataSet.Id = Guid.NewGuid();

				new TfDataSetValidator(this)
					.ValidateCreate(newDataSet)
					.ToValidationException()
					.ThrowIfContainsErrors();

				var datasets = GetAllDataSets();

				var dbo = ConvertModelToDbo(newDataSet);
				var success = _dboManager.Insert<TfDataSetDbo>(dbo);

				if (!success)
					throw new TfDboServiceException("Insert<TfDataSetDbo> failed.");

				if (dataset.Identities.Count > 0)
				{
					var allProviders = GetDataProviders();
					var sharedColumns = GetSharedColumns();

					foreach (var sdIdentity in dataset.Identities)
					{
						foreach (var column in sdIdentity.Columns)
						{
							var columnProvider = allProviders.FirstOrDefault(x => x.Columns.Any(y => y.DbName == column));
							var sharedColumn = sharedColumns.FirstOrDefault(x => x.DbName == column);
							if (columnProvider is null && sharedColumn is null) continue;

							var colSubmit = new TfDataSetColumn
							{
								DataIdentity = sdIdentity.DataIdentity,
								ColumnName = $"{sdIdentity}.{column}",
								SourceName = null,
								SourceCode = null,
								SourceColumnName = null,
								SourceType = TfAuxDataSourceType.NotFound,
								DbType = TfDatabaseColumnType.Text


							};
							if (columnProvider is not null)
							{
								var providerColumn = columnProvider!.Columns.Single(x => x.DbName == column);
								colSubmit.SourceName = columnProvider.Name;
								colSubmit.SourceCode = columnProvider.ColumnPrefix;
								colSubmit.SourceColumnName = column;
								colSubmit.SourceType = TfAuxDataSourceType.AuxDataProvider;
								colSubmit.DbType = providerColumn.DbType;
							}
							else
							{
								colSubmit.SourceName = sharedColumn!.DbName;
								colSubmit.SourceCode = null;
								colSubmit.SourceColumnName = sharedColumn!.DbName;
								colSubmit.SourceType = TfAuxDataSourceType.SharedColumn;
								colSubmit.DbType = sharedColumn!.DbType;
							}

							AddDataSetColumn(newDataSet.Id, colSubmit);
						}
					}
				}

				scope.Complete();

				return GetDataSet(newDataSet.Id);
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfDataSet? UpdateDataSet(
		TfUpdateDataSet? updateDataSet)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				TfDataSet dataset = null;
				TfDataSetDbo existingDataSet = null;

				if (updateDataSet is not null)
				{
					existingDataSet = _dboManager.Get<TfDataSetDbo>(updateDataSet.Id);

					dataset = new TfDataSet();
					dataset.Id = updateDataSet.Id;
					dataset.DataProviderId = updateDataSet.DataProviderId;
					dataset.Name = updateDataSet.Name;
					dataset.Filters = updateDataSet.Filters ?? new List<TfFilterBase>();
					dataset.Columns = updateDataSet.Columns ?? new List<string>();
					dataset.SortOrders = updateDataSet.SortOrders ?? new List<TfSort>();
				}

				new TfDataSetValidator(this)
					.ValidateUpdate(dataset)
					.ToValidationException()
					.ThrowIfContainsErrors();

				var dbo = ConvertModelToDbo(dataset);

				if(dbo is null)
					throw new TfDboServiceException("ConvertModelToDbo returned null.");

				var success = _dboManager.Update<TfDataSetDbo>(dbo);
				if (!success)
					throw new TfDboServiceException("Update<TfDataSetDbo> failed.");

				scope.Complete();

				return GetDataSet(dbo.Id);
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void DeleteDataSet(
		Guid id)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var dataset = GetDataSet(id);
				
				new TfDataSetValidator(this)
					.ValidateDelete(dataset)
					.ToValidationException()
					.ThrowIfContainsErrors();

				//delete identities
				var dataSetIdentities = GetDataSetIdentities(id);
				foreach (var identity in dataSetIdentities)
				{
					var successDeleteIdentity = _dboManager.Delete<TfDataSetIdentityDbo>(identity.Id);
					if (!successDeleteIdentity)
						throw new TfDboServiceException("Delete<TfDataSetIdentityDbo> failed during delete dataset process.");
				}

				var success = _dboManager.Delete<TfDataSetDbo>(id);

				if (!success)
					throw new TfDboServiceException("Delete<TfDataSetDbo> failed.");

				scope.Complete();
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfDataSet? CopyDataSet(
		Guid originalId)
	{
		TfDataSet? originalDataSet = GetDataSet(originalId);
		if (originalDataSet is null)
			throw new Exception("DataSet not found");

		try
		{
			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var datasetList = GetAllDataSets();

				var copyName = GetDataSetCopyName(originalDataSet.Name, datasetList);
				if (String.IsNullOrWhiteSpace(copyName))
					throw new Exception("DataSet copy name could not be generated");

				TfDataSet dataset = new()
				{
					Id = Guid.NewGuid(),
					DataProviderId = originalDataSet.DataProviderId,
					Name = copyName,
					Filters = originalDataSet.Filters ?? new List<TfFilterBase>(),
					Columns = originalDataSet.Columns ?? new List<string>(),
					SortOrders = originalDataSet.SortOrders ?? new List<TfSort>(),
					Identities = originalDataSet.Identities,
				};
				new TfDataSetValidator(this)
					.ValidateCreate(dataset)
					.ToValidationException()
					.ThrowIfContainsErrors();

				var dbo = ConvertModelToDbo(dataset);

				var success = _dboManager.Insert<TfDataSetDbo>(dbo);
				if (!success)
					throw new TfDboServiceException("Insert<TfDataSetDbo> failed.");

				scope.Complete();

				return GetDataSet(dataset.Id);
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	[Obsolete("This method is not used and not implemented")]
	public void MoveDataSetUp(
		Guid id)
	{
		try
		{
			throw new NotImplementedException();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	[Obsolete("This method is not used and not implemented")]
	public void MoveDataSetDown(
		Guid id)
	{
		try
		{
			throw new NotImplementedException();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}


	public List<TfDataSetColumn> GetDataSetColumns(
		Guid datasetId)
	{
		var dataset = GetDataSet(datasetId);
		if (dataset is null) 
			throw new Exception("DataSet not found");

		var result = new List<TfDataSetColumn>();
		var allColumns = GetDataSetColumnOptions(datasetId);
		var datasetColumns = dataset.Columns.ToList();

		foreach (var identity in dataset.Identities)
		{
			foreach (var column in identity.Columns)
			{
				datasetColumns.Add($"{identity.DataIdentity}.{column}");
			}
		}

		foreach (var item in datasetColumns)
		{
			TfDataSetColumn? column = null;
			column = allColumns.FirstOrDefault(x => x.ColumnName == item);
			if (column is null)
			{
				result.Add(new TfDataSetColumn
				{
					ColumnName = item,
					DataIdentity = null,
					DbType = TfDatabaseColumnType.Text,
					SourceCode = null,
					SourceColumnName = null,
					SourceName = null,
					SourceType = TfAuxDataSourceType.NotFound
				});
			}
			else
			{
				result.Add(column);
			}
		}

		return result;
	}

	public List<TfDataSetColumn> GetDataSetColumnOptions(
		Guid datasetId)
	{
		var dataset = GetDataSet(datasetId);
		if (dataset is null) 
			throw new Exception("DataSet not found");

		var provider = GetDataProvider(dataset.DataProviderId);
		var auxDataSchema = GetDataProviderAuxDataSchema(provider.Id);

		var result = new List<TfDataSetColumn>();

		foreach (var providerColumn in provider.Columns)
		{
			var item = new TfDataSetColumn
			{
				DataIdentity = null,
				ColumnName = providerColumn.DbName,
				SourceColumnName = providerColumn.DbName,
				SourceName = provider.Name,
				SourceCode = $"dp{provider.Index}",
				SourceType = TfAuxDataSourceType.PrimatyDataProvider,
				DbType = providerColumn.DbType
			};
			result.Add(item);
		}

		foreach (var implementedIdentity in auxDataSchema.DataIdentities)
		{
			if (implementedIdentity.DataIdentity.DataIdentity == TfConstants.TF_ROW_ID_DATA_IDENTITY) continue;
			foreach (var schemaColumn in implementedIdentity.Columns)
			{
				var item = new TfDataSetColumn
				{
					DataIdentity = schemaColumn.DataIdentity?.DataIdentity,
					ColumnName = schemaColumn.DbName,
				};
				if (schemaColumn.SharedColumn is not null)
				{
					item.ColumnName = $"{implementedIdentity.DataIdentity.DataIdentity}.{schemaColumn.SharedColumn.DbName}";
					item.SourceName = implementedIdentity.DataIdentity.DataIdentity;
					item.SourceColumnName = schemaColumn.SharedColumn.DbName;
					item.SourceCode = null;
					item.SourceType = TfAuxDataSourceType.SharedColumn;
					item.DbType = schemaColumn.SharedColumn.DbType;
				}
				else if (schemaColumn.DataProvider is not null && schemaColumn.DataProviderColumn is not null)
				{
					item.SourceColumnName = schemaColumn.DataProviderColumn.DbName;
					item.SourceName = schemaColumn.DataProvider.Name;
					item.SourceCode = $"dp{schemaColumn.DataProvider.Index}";
					item.SourceType = TfAuxDataSourceType.AuxDataProvider;
					item.DbType = schemaColumn.DataProviderColumn.DbType;
				}

				result.Add(item);

			}
		}

		return result;
	}

	public void AddDataSetColumn(
		Guid datasetId,
		TfDataSetColumn column)
	{
		if (column is null)
			new ArgumentException(nameof(column), "Column is required");

		if (String.IsNullOrWhiteSpace(column!.ColumnName))
			new ArgumentException(nameof(column), "Column name is required");

		var dataset = GetDataSet(datasetId);
		if (dataset is null)
			new TfException("DataSet not found");

		if (column!.SourceType == TfAuxDataSourceType.PrimatyDataProvider)
		{
			if (!dataset!.Columns.Any(x => x.ToLowerInvariant() == column!.ColumnName!.ToLowerInvariant()))
			{
				var submit = new TfUpdateDataSet
				{
					Id = dataset.Id,
					Columns = dataset.Columns,
					DataProviderId = dataset.DataProviderId,
					Filters = dataset.Filters,
					Name = dataset.Name,
					SortOrders = dataset.SortOrders,
				};
				submit.Columns.Add(column.ColumnName!);
				UpdateDataSet(submit);
			}
		}
		else if (column.SourceType == TfAuxDataSourceType.AuxDataProvider
		|| column.SourceType == TfAuxDataSourceType.SharedColumn)
		{
			if (String.IsNullOrWhiteSpace(column.DataIdentity))
				throw new Exception("Column Data Identity is required");

			if (column.SourceColumnName is null)
				throw new Exception("Column SourceColumnName is required");

			var dataIdentity = dataset!.Identities.FirstOrDefault(x => x.DataIdentity == column!.DataIdentity);
			if (dataIdentity is null)
			{
				CreateDataSetIdentity(new TfDataSetIdentity
				{
					Id = Guid.NewGuid(),
					DataSetId = datasetId,
					Columns = new List<string> { column!.SourceColumnName },
					DataIdentity = column!.DataIdentity,
				});
			}
			else
			{
				if (dataIdentity.Columns is null)
					dataIdentity.Columns = new();

				if (!dataIdentity.Columns.Any(x => x == column.SourceColumnName))
				{
					dataIdentity.Columns.Add(column.SourceColumnName);
				}
				UpdateDataSetIdentity(new TfDataSetIdentity
				{
					Id = dataIdentity.Id,
					DataSetId = dataIdentity.DataSetId,
					Columns = dataIdentity.Columns,
					DataIdentity = dataIdentity.DataIdentity,
				});
			}
		}
	}

	public void AddAvailableColumnsToDataSet(
		Guid datasetId)
	{
		var columnOptions = GetDataSetColumnOptions(datasetId);
		try
		{
			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				foreach (var column in columnOptions)
				{
					AddDataSetColumn(datasetId, column);
				}
				scope.Complete();
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void RemoveDataSetColumn(
		Guid datasetId, 
		TfDataSetColumn column)
	{
		if (column is null)
			new ArgumentException(nameof(column), "column is required");

		if (String.IsNullOrWhiteSpace(column!.ColumnName))
			new ArgumentException(nameof(column), "column name is required");

		var dataset = GetDataSet(datasetId);
		if (dataset is null)
			new TfException("DataSet not found");

		if (column.SourceType == TfAuxDataSourceType.PrimatyDataProvider)
		{
			if (dataset!.Columns.Any(x => x.ToLowerInvariant() == column!.ColumnName!.ToLowerInvariant()))
			{
				var submit = new TfUpdateDataSet
				{
					Id = dataset.Id,
					Columns = dataset.Columns,
					DataProviderId = dataset.DataProviderId,
					Filters = dataset.Filters,
					Name = dataset.Name,
					SortOrders = dataset.SortOrders,
				};
				submit.Columns = dataset.Columns.Where(x => x.ToLowerInvariant() != column!.ColumnName!.ToLowerInvariant()).ToList();
				var updatedSpaceData = UpdateDataSet(submit);
			}
		}
		else if (column.SourceType == TfAuxDataSourceType.AuxDataProvider
			|| column.SourceType == TfAuxDataSourceType.SharedColumn)
		{
			if (String.IsNullOrWhiteSpace(column.DataIdentity))
				throw new Exception("Column Data Identity is required");

			if (column.SourceColumnName is null)
				throw new Exception("Column SourceColumnName is required");

			var dataIdentity = dataset!.Identities.FirstOrDefault(x => x.DataIdentity == column!.DataIdentity);
			if (dataIdentity is not null)
			{
				if (dataIdentity.Columns is null)
					dataIdentity.Columns = new();

				if (dataIdentity.Columns.Any(x => x == column.SourceColumnName))
				{
					dataIdentity.Columns.Remove(column.SourceColumnName);
				}
				if (dataIdentity.Columns.Count > 0)
				{
					UpdateDataSetIdentity(new TfDataSetIdentity
					{
						Id = dataIdentity.Id,
						DataSetId = dataIdentity.DataSetId,
						Columns = dataIdentity.Columns,
						DataIdentity = dataIdentity.DataIdentity,
					});
				}
				else
				{
					DeleteDataSetIdentity(dataIdentity.Id);
				}
			}
		}

	}

	public void UpdateDataSetFilters(
		Guid datasetId,
		List<TfFilterBase> filters)
	{
		var dataset = GetDataSet(datasetId);
		if (dataset is null)
			throw new TfException("DataSet not found");

		var submit = new TfUpdateDataSet(dataset);
		submit.Filters = filters;
		
		UpdateDataSet(submit);
	}

	public void UpdateDataSetSorts(
		Guid datasetId,
		List<TfSort> sorts)
	{
		var dataset = GetDataSet(datasetId);
		if (dataset is null)
			throw new TfException("DataSet not found");

		var submit = new TfUpdateDataSet(dataset);
		submit.SortOrders = sorts;
		
		UpdateDataSet(submit);
	}
	
	#region <--- validation --->

	internal class TfDataSetValidator
	: AbstractValidator<TfDataSet>
	{
		public TfDataSetValidator(
			ITfService tfService)
		{

			RuleSet("general", () =>
			{
				RuleFor(dataset => dataset.Id)
					.NotEmpty()
					.WithMessage("The dataset id is required.");

				RuleFor(dataset => dataset.Name)
					.NotEmpty()
					.WithMessage("The dataset name is required.");

				RuleFor(dataset => dataset.DataProviderId)
					.NotEmpty()
					.WithMessage("The data provider is required.");

				RuleFor(dataset => dataset.DataProviderId)
					.Must(providerId =>
					{
						return tfService.GetDataProvider(providerId) != null;
					})
					.WithMessage("There is no existing data provider for specified provider id.");

				RuleFor(dataset => dataset.Columns)
					.Must(columns =>
					{
						HashSet<string> columnsHS = new HashSet<string>();
						foreach (var column in columns)
						{
							if (columnsHS.Contains(column))
								return false;

							columnsHS.Add(column);
						}

						return true;
					})
					.WithMessage("There are duplicate columns. This is not allowed");
			});

			RuleSet("create", () =>
			{
				RuleFor(dataset => dataset.Id)
						.Must((dataset, id) => { return tfService.GetDataSet(id) == null; })
						.WithMessage("There is already existing dataset with specified identifier.");

			});

			RuleSet("update", () =>
			{
				RuleFor(dataset => dataset.Id)
						.Must((dataset, id) =>
						{
							return tfService.GetDataSet(id) != null;
						})
						.WithMessage("There is not existing dataset with specified identifier.");
			});

			RuleSet("delete", () =>
			{
			});

		}

		public ValidationResult ValidateCreate(
			TfDataSet? dataset)
		{
			if (dataset == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The dataset is null.") });

			return this.Validate(dataset, options =>
			{
				options.IncludeRuleSets("general", "create");
			});
		}

		public ValidationResult ValidateUpdate(
			TfDataSet? dataset)
		{
			if (dataset == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The dataset is null.") });

			return this.Validate(dataset, options =>
			{
				options.IncludeRuleSets("general", "update");
			});
		}

		public ValidationResult ValidateDelete(
			TfDataSet? dataset)
		{
			if (dataset == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The dataset with specified identifier is not found.") });

			return this.Validate(dataset, options =>
			{
				options.IncludeRuleSets("delete");
			});
		}
	}

	#endregion

	#region <--- private --->

	private TfDataSet? ConvertDboToModel(
		TfDataSetDbo? dbo)
	{
		if (dbo == null)
			return null;

		var jsonOptions = new JsonSerializerOptions
		{
			TypeInfoResolver = new DefaultJsonTypeInfoResolver
			{
				Modifiers = { JsonExtensions.AddPrivateProperties<JsonIncludePrivatePropertyAttribute>() },
			},
		};

		var filters = JsonSerializer.Deserialize<List<TfFilterBase>>(dbo.FiltersJson, jsonOptions);
		var columns = JsonSerializer.Deserialize<List<string>>(dbo.ColumnsJson, jsonOptions);
		var sortOrders = JsonSerializer.Deserialize<List<TfSort>>(dbo.SortOrdersJson, jsonOptions);

		return new TfDataSet
		{
			Id = dbo.Id,
			DataProviderId = dbo.DataProviderId,
			Name = dbo.Name,
			Filters = filters ?? new(),
			Columns = columns ?? new(),
			SortOrders = sortOrders ?? new()
		};

	}

	private TfDataSetDbo? ConvertModelToDbo(
		TfDataSet? model)
	{
		if (model == null)
			return null;

		var jsonOptions = new JsonSerializerOptions
		{
			TypeInfoResolver = new DefaultJsonTypeInfoResolver
			{
				Modifiers = { JsonExtensions.AddPrivateProperties<JsonIncludePrivatePropertyAttribute>() },
			},
		};

		return new TfDataSetDbo
		{
			Id = model.Id,
			DataProviderId = model.DataProviderId,
			Name = model.Name,
			FiltersJson = JsonSerializer.Serialize(model.Filters ?? new List<TfFilterBase>(), jsonOptions),
			ColumnsJson = JsonSerializer.Serialize(model.Columns ?? new List<string>(), jsonOptions),
			SortOrdersJson = JsonSerializer.Serialize(model.SortOrders ?? new List<TfSort>(), jsonOptions)
		};
	}

	private string? GetDataSetCopyName(
		string originalName, 
		List<TfDataSet> datasets)
	{
		var index = 1;
		var presentNamesHS = datasets.Select(x => x.Name).ToHashSet();

		while (true)
		{
			var suggestion = $"{originalName} {index}";
			if (!presentNamesHS.Contains(suggestion))
				return suggestion;

			index++;

			if (index > 100000) break;
		}

		return null;
	}

	#endregion
}
