using System.Text.Json.Serialization.Metadata;
using WebVella.Tefter.Models;

namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	public List<TfDataset> GetDatasets(string? search = null, Guid? providerId = null);

	public TfDataset? GetDataset(
		Guid id);

	public TfDataset CreateDataset(
		TfCreateDataset dataset);

	public TfDataset UpdateDataset(
		TfUpdateDataset dataset);

	public void DeleteDataset(
		Guid id);

	public TfDataset? CopyDataset(
		Guid id);

	//Columns
	public List<TfAvailableDatasetColumn> GetDatasetAvailableColumns(
		Guid datasetId);

	List<TfDatasetColumn> GetDatasetColumns(
		Guid datasetId);

	List<TfDatasetColumn> GetDatasetColumnOptions(
		Guid datasetId);

	void AddDatasetColumn(
		Guid datasetId,
		TfDatasetColumn column);

	void AddAvailableColumnsToDataset(
		Guid datasetId);

	void RemoveDatasetColumn(
		Guid datasetId,
		TfDatasetColumn column);

	public void UpdateDatasetFilters(
		Guid datasetId,
		List<TfFilterBase> filters);

	public void UpdateDatasetSorts(
		Guid datasetId,
		List<TfSort> sorts);
}

public partial class TfService : ITfService
{
	public List<TfDataset> GetDatasets(
		string? search = null, Guid? providerId = null)
	{
		try
		{
			var dbos = _dboManager.GetList<TfDatasetDbo>();
			var datasets = dbos.Where(x => x is not null).Select(x => ConvertDboToModel(x)).OrderBy(x=> x.Name).ToList();
			var result = new List<TfDataset>();
			foreach (var dataset in datasets)
			{
				if (dataset == null)
					continue;

				if (providerId is not null && dataset.DataProviderId != providerId.Value)
					continue;
				if (!String.IsNullOrWhiteSpace(search)
					&& !dataset.Name.ToLowerInvariant().Contains(search.Trim().ToLowerInvariant()))
					continue;

				dataset.Identities = new ReadOnlyCollection<TfDatasetIdentity>(
					GetDatasetIdentities(dataset.Id).ToList());

				result.Add(dataset);
			}
			return result;
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfDataset? GetDataset(
		Guid id)
	{
		try
		{
			var dbo = _dboManager.Get<TfDatasetDbo>(id);
			if (dbo == null)
				return null;

			var dataset = ConvertDboToModel(dbo);

			if (dataset == null)
				return null;

			dataset.Identities = new ReadOnlyCollection<TfDatasetIdentity>(
					GetDatasetIdentities(dataset.Id).ToList());
			return dataset;
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfDataset CreateDataset(
		TfCreateDataset dataset)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var newDataset = new TfDataset();
				newDataset.Id = dataset.Id;
				newDataset.DataProviderId = dataset.DataProviderId;
				newDataset.Name = dataset.Name;
				newDataset.Filters = dataset.Filters ?? new List<TfFilterBase>();
				newDataset.Columns = dataset.Columns ?? new List<string>();
				newDataset.SortOrders = dataset.SortOrders ?? new List<TfSort>();

				if (newDataset.Id == Guid.Empty)
					newDataset.Id = Guid.NewGuid();

				new TfDatasetValidator(this)
					.ValidateCreate(newDataset)
					.ToValidationException()
					.ThrowIfContainsErrors();

				var datasets = GetDatasets();

				var dbo = ConvertModelToDbo(newDataset);
				var success = _dboManager.Insert<TfDatasetDbo>(dbo!);

				if (!success)
					throw new TfDboServiceException("Insert<TfDatasetDbo> failed.");

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

							var colSubmit = new TfDatasetColumn
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

							AddDatasetColumn(newDataset.Id, colSubmit);
						}
					}
				}

				scope.Complete();

				return GetDataset(newDataset.Id) ?? throw new Exception($"GetDataset failed for id: {newDataset.Id}");
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfDataset UpdateDataset(
		TfUpdateDataset updateDataset)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var dataset = new TfDataset();
				dataset.Id = updateDataset.Id;
				dataset.DataProviderId = updateDataset.DataProviderId;
				dataset.Name = updateDataset.Name ?? String.Empty;
				dataset.Filters = updateDataset.Filters ?? new List<TfFilterBase>();
				dataset.Columns = updateDataset.Columns ?? new List<string>();
				dataset.SortOrders = updateDataset.SortOrders ?? new List<TfSort>();

				new TfDatasetValidator(this)
					.ValidateUpdate(dataset)
					.ToValidationException()
					.ThrowIfContainsErrors();

				var dbo = ConvertModelToDbo(dataset);

				if (dbo is null)
					throw new TfDboServiceException("ConvertModelToDbo returned null.");

				var success = _dboManager.Update<TfDatasetDbo>(dbo);
				if (!success)
					throw new TfDboServiceException("Update<TfDatasetDbo> failed.");

				scope.Complete();

				return GetDataset(dbo.Id) ?? throw new Exception($"UpdateDataset failed for id: {updateDataset.Id}");
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}


	public void DeleteDataset(
		Guid id)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var dataset = GetDataset(id);

				new TfDatasetValidator(this)
					.ValidateDelete(dataset)
					.ToValidationException()
					.ThrowIfContainsErrors();

				//delete identities
				var dataSetIdentities = GetDatasetIdentities(id);
				foreach (var identity in dataSetIdentities)
				{
					var successDeleteIdentity = _dboManager.Delete<TfDatasetIdentityDbo>(identity.Id);
					if (!successDeleteIdentity)
						throw new TfDboServiceException("Delete<TfDatasetIdentityDbo> failed during delete dataset process.");
				}

				var success = _dboManager.Delete<TfDatasetDbo>(id);

				if (!success)
					throw new TfDboServiceException("Delete<TfDatasetDbo> failed.");

				scope.Complete();
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfDataset CopyDataset(
		Guid originalId)
	{
		var originalDataset = GetDataset(originalId);
		if (originalDataset is null)
			throw new Exception("Dataset not found");

		try
		{
			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var datasetList = GetDatasets();

				var copyName = GetDatasetCopyName(originalDataset.Name, datasetList);
				if (String.IsNullOrWhiteSpace(copyName))
					throw new Exception("Dataset copy name could not be generated");

				TfDataset dataset = new()
				{
					Id = Guid.NewGuid(),
					DataProviderId = originalDataset.DataProviderId,
					Name = copyName,
					Filters = originalDataset.Filters ?? new List<TfFilterBase>(),
					Columns = originalDataset.Columns ?? new List<string>(),
					SortOrders = originalDataset.SortOrders ?? new List<TfSort>(),
					Identities = originalDataset.Identities,
				};
				new TfDatasetValidator(this)
					.ValidateCreate(dataset)
					.ToValidationException()
					.ThrowIfContainsErrors();

				var dbo = ConvertModelToDbo(dataset);

				var success = _dboManager.Insert<TfDatasetDbo>(dbo);
				if (!success)
					throw new TfDboServiceException("Insert<TfDatasetDbo> failed.");

				scope.Complete();

				return GetDataset(dataset.Id) ?? throw new Exception($"CopyDataset failed for id: {originalId}");
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public List<TfAvailableDatasetColumn> GetDatasetAvailableColumns(
			Guid datasetId)
	{
		try
		{
			List<TfAvailableDatasetColumn> columns = new List<TfAvailableDatasetColumn>();

			var dbo = _dboManager.Get<TfDatasetDbo>(datasetId);
			var dataset = ConvertDboToModel(dbo);

			if (dataset == null)
				return columns;

			var provider = GetDataProvider(dataset.DataProviderId);
			if (provider is null)
				throw new TfException("Not found specified data provider");

			foreach (var column in provider.Columns)
			{
				columns.Add(new TfAvailableDatasetColumn
				{
					DbName = column.DbName!,
					DbType = column.DbType
				});
			}

			foreach (var identity in provider.Identities)
			{
				columns.Add(new TfAvailableDatasetColumn
				{
					DbName = identity.DataIdentity,
					DbType = TfDatabaseColumnType.ShortText,
				});
			}

			foreach (var column in provider.SharedColumns)
			{
				columns.Add(new TfAvailableDatasetColumn
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


	public List<TfDatasetColumn> GetDatasetColumns(
		Guid datasetId)
	{
		var dataset = GetDataset(datasetId);
		if (dataset is null)
			throw new Exception("Dataset not found");

		var result = new List<TfDatasetColumn>();
		var allColumns = GetDatasetColumnOptions(datasetId);
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
			TfDatasetColumn? column = null;
			column = allColumns.FirstOrDefault(x => x.ColumnName == item);
			if (column is null)
			{
				result.Add(new TfDatasetColumn
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

	public List<TfDatasetColumn> GetDatasetColumnOptions(
		Guid datasetId)
	{
		var dataset = GetDataset(datasetId);
		if (dataset is null)
			throw new Exception("Dataset not found");

		var provider = GetDataProvider(dataset.DataProviderId);
		var auxDataSchema = GetDataProviderAuxDataSchema(provider.Id);

		var result = new List<TfDatasetColumn>();

		foreach (var providerColumn in provider.Columns)
		{
			var item = new TfDatasetColumn
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
				var item = new TfDatasetColumn
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

	public void AddDatasetColumn(
		Guid datasetId,
		TfDatasetColumn column)
	{
		if (column is null)
			new ArgumentException(nameof(column), "Column is required");

		if (String.IsNullOrWhiteSpace(column!.ColumnName))
			new ArgumentException(nameof(column), "Column name is required");

		var dataset = GetDataset(datasetId);
		if (dataset is null)
			new TfException("Dataset not found");

		if (column!.SourceType == TfAuxDataSourceType.PrimatyDataProvider)
		{
			if (!dataset!.Columns.Any(x => x.ToLowerInvariant() == column!.ColumnName!.ToLowerInvariant()))
			{
				var submit = new TfUpdateDataset
				{
					Id = dataset.Id,
					Columns = dataset.Columns,
					DataProviderId = dataset.DataProviderId,
					Filters = dataset.Filters,
					Name = dataset.Name,
					SortOrders = dataset.SortOrders,
				};
				submit.Columns.Add(column.ColumnName!);
				UpdateDataset(submit);
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
				CreateDatasetIdentity(new TfDatasetIdentity
				{
					Id = Guid.NewGuid(),
					DatasetId = datasetId,
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
				UpdateDatasetIdentity(new TfDatasetIdentity
				{
					Id = dataIdentity.Id,
					DatasetId = dataIdentity.DatasetId,
					Columns = dataIdentity.Columns,
					DataIdentity = dataIdentity.DataIdentity,
				});
			}
		}
	}

	public void AddAvailableColumnsToDataset(
		Guid datasetId)
	{
		var columnOptions = GetDatasetColumnOptions(datasetId);
		try
		{
			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				foreach (var column in columnOptions)
				{
					AddDatasetColumn(datasetId, column);
				}
				scope.Complete();
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void RemoveDatasetColumn(
		Guid datasetId,
		TfDatasetColumn column)
	{
		if (column is null)
			new ArgumentException(nameof(column), "column is required");

		if (String.IsNullOrWhiteSpace(column!.ColumnName))
			new ArgumentException(nameof(column), "column name is required");

		var dataset = GetDataset(datasetId);
		if (dataset is null)
			new TfException("Dataset not found");

		if (column.SourceType == TfAuxDataSourceType.PrimatyDataProvider)
		{
			if (dataset!.Columns.Any(x => x.ToLowerInvariant() == column!.ColumnName!.ToLowerInvariant()))
			{
				var submit = new TfUpdateDataset
				{
					Id = dataset.Id,
					Columns = dataset.Columns,
					DataProviderId = dataset.DataProviderId,
					Filters = dataset.Filters,
					Name = dataset.Name,
					SortOrders = dataset.SortOrders,
				};
				submit.Columns = dataset.Columns.Where(x => x.ToLowerInvariant() != column!.ColumnName!.ToLowerInvariant()).ToList();
				var updatedSpaceData = UpdateDataset(submit);
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
					UpdateDatasetIdentity(new TfDatasetIdentity
					{
						Id = dataIdentity.Id,
						DatasetId = dataIdentity.DatasetId,
						Columns = dataIdentity.Columns,
						DataIdentity = dataIdentity.DataIdentity,
					});
				}
				else
				{
					DeleteDatasetIdentity(dataIdentity.Id);
				}
			}
		}

	}

	public void UpdateDatasetFilters(
		Guid datasetId,
		List<TfFilterBase> filters)
	{
		var dataset = GetDataset(datasetId);
		if (dataset is null)
			throw new TfException("Dataset not found");

		var submit = new TfUpdateDataset(dataset);
		submit.Filters = filters;

		UpdateDataset(submit);
	}

	public void UpdateDatasetSorts(
		Guid datasetId,
		List<TfSort> sorts)
	{
		var dataset = GetDataset(datasetId);
		if (dataset is null)
			throw new TfException("Dataset not found");

		var submit = new TfUpdateDataset(dataset);
		submit.SortOrders = sorts;

		UpdateDataset(submit);
	}

	#region <--- validation --->

	internal class TfDatasetValidator
	: AbstractValidator<TfDataset>
	{
		public TfDatasetValidator(
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
						.Must((dataset, id) => { return tfService.GetDataset(id) == null; })
						.WithMessage("There is already existing dataset with specified identifier.");

			});

			RuleSet("update", () =>
			{
				RuleFor(dataset => dataset.Id)
						.Must((dataset, id) =>
						{
							return tfService.GetDataset(id) != null;
						})
						.WithMessage("There is not existing dataset with specified identifier.");
			});

			RuleSet("delete", () =>
			{
			});

		}

		public ValidationResult ValidateCreate(
			TfDataset? dataset)
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
			TfDataset? dataset)
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
			TfDataset? dataset)
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

	private TfDataset? ConvertDboToModel(
		TfDatasetDbo? dbo)
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

		return new TfDataset
		{
			Id = dbo.Id,
			DataProviderId = dbo.DataProviderId,
			Name = dbo.Name,
			Filters = filters ?? new(),
			Columns = columns ?? new(),
			SortOrders = sortOrders ?? new()
		};

	}

	private TfDatasetDbo? ConvertModelToDbo(
		TfDataset? model)
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

		return new TfDatasetDbo
		{
			Id = model.Id,
			DataProviderId = model.DataProviderId,
			Name = model.Name,
			FiltersJson = JsonSerializer.Serialize(model.Filters ?? new List<TfFilterBase>(), jsonOptions),
			ColumnsJson = JsonSerializer.Serialize(model.Columns ?? new List<string>(), jsonOptions),
			SortOrdersJson = JsonSerializer.Serialize(model.SortOrders ?? new List<TfSort>(), jsonOptions)
		};
	}

	private string? GetDatasetCopyName(
		string originalName,
		List<TfDataset> datasets)
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
