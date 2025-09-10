using System.Text.Json.Serialization.Metadata;
using WebVella.Tefter.Models;

namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	public List<TfSpaceData> GetAllSpaceData(string? search = null);

	public List<TfSpaceData> GetSpaceDataList(
		Guid spaceId,
		string? search = null);

	public TfSpaceData GetSpaceData(
		Guid id);

	public TfSpaceData CreateSpaceData(
		TfCreateSpaceData spaceData);

	public TfSpaceData UpdateSpaceData(
		TfUpdateSpaceData spaceData);

	public void DeleteSpaceData(
		Guid id);

	public TfSpaceData CopySpaceData(
		Guid id);

	public void MoveSpaceDataUp(
		Guid id);

	public void MoveSpaceDataDown(
		Guid id);

	//Columns
	public List<TfAvailableSpaceDataColumn> GetSpaceDataAvailableColumns(
		Guid spaceDataId);

	List<TfSpaceDataColumn> GetSpaceDataColumns(Guid spaceDataId);
	List<TfSpaceDataColumn> GetSpaceDataColumnOptions(Guid spaceDataId);

	void AddSpaceDataColumn(Guid spaceDataId, TfSpaceDataColumn column);
	void RemoveSpaceDataColumn(Guid spaceDataId, TfSpaceDataColumn column);

	//Filters
	public void UpdateSpaceDataFilters(Guid spaceDataId,
		List<TfFilterBase> filters);
	public void UpdateSpaceDataSorts(Guid spaceDataId,
		List<TfSort> sorts);
}

public partial class TfService : ITfService
{
	public List<TfSpaceData> GetAllSpaceData(string? search = null)
	{
		try
		{
			var dbos = _dboManager.GetList<TfSpaceDataDbo>();

			var spaceDatas = dbos.Select(x => ConvertDboToModel(x)).ToList();
			foreach (var spaceData in spaceDatas)
			{
				spaceData.Identities = new ReadOnlyCollection<TfSpaceDataIdentity>(
					GetSpaceDataIdentities(spaceData.Id).ToList());
			}

			if (String.IsNullOrWhiteSpace(search))
				return spaceDatas;
			search = search.Trim().ToLowerInvariant();
			return spaceDatas.Where(x =>
				x.Name.ToLowerInvariant().Contains(search)
				).ToList();

		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public List<TfSpaceData> GetSpaceDataList(
		Guid spaceId, string? search = null)
	{
		try
		{
			var orderSettings = new TfOrderSettings(
				nameof(TfSpaceData.Position),
				OrderDirection.ASC);

			var dbos = _dboManager.GetList<TfSpaceDataDbo>(
				spaceId,
				nameof(TfSpaceData.SpaceId), order: orderSettings);

			var spaceDatas = dbos.Select(x => ConvertDboToModel(x)).ToList();
			foreach (var spaceData in spaceDatas)
			{
				spaceData.Identities = new ReadOnlyCollection<TfSpaceDataIdentity>(
					GetSpaceDataIdentities(spaceData.Id).ToList());
			}
			if (String.IsNullOrWhiteSpace(search))
				return spaceDatas;
			search = search.Trim().ToLowerInvariant();
			return spaceDatas.Where(x =>
				x.Name.ToLowerInvariant().Contains(search)
				).ToList();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfSpaceData GetSpaceData(
		Guid id)
	{
		try
		{
			var dbo = _dboManager.Get<TfSpaceDataDbo>(id);
			if (dbo == null)
				return null;

			var spaceData = ConvertDboToModel(dbo);
			spaceData.Identities = new ReadOnlyCollection<TfSpaceDataIdentity>(
					GetSpaceDataIdentities(spaceData.Id).ToList());
			return spaceData;
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public List<TfAvailableSpaceDataColumn> GetSpaceDataAvailableColumns(
		Guid spaceDataId)
	{
		try
		{
			List<TfAvailableSpaceDataColumn> columns = new List<TfAvailableSpaceDataColumn>();

			var dbo = _dboManager.Get<TfSpaceDataDbo>(spaceDataId);
			var spaceData = ConvertDboToModel(dbo);

			if (spaceData == null)
				return columns;

			var provider = GetDataProvider(spaceData.DataProviderId);
			if (provider is null)
				throw new TfException("Not found specified data provider");

			foreach (var column in provider.Columns)
			{
				columns.Add(new TfAvailableSpaceDataColumn
				{
					DbName = column.DbName,
					DbType = column.DbType
				});
			}

			foreach (var identity in provider.Identities)
			{
				columns.Add(new TfAvailableSpaceDataColumn
				{
					DbName = identity.DataIdentity,
					DbType = TfDatabaseColumnType.ShortText,
				});
			}

			foreach (var column in provider.SharedColumns)
			{
				columns.Add(new TfAvailableSpaceDataColumn
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

	public TfSpaceData CreateSpaceData(
		TfCreateSpaceData createSpaceData)
	{
		if (createSpaceData is null) throw new ArgumentException("required", nameof(createSpaceData));
		try
		{
			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var spaceData = new TfSpaceData();
				spaceData.Id = createSpaceData.Id;
				spaceData.DataProviderId = createSpaceData.DataProviderId;
				spaceData.Name = createSpaceData.Name;
				spaceData.SpaceId = createSpaceData.SpaceId;
				spaceData.Filters = createSpaceData.Filters ?? new List<TfFilterBase>();
				spaceData.Columns = createSpaceData.Columns ?? new List<string>();
				spaceData.SortOrders = createSpaceData.SortOrders ?? new List<TfSort>();

				if (spaceData.Id == Guid.Empty)
					spaceData.Id = Guid.NewGuid();

				new TfSpaceDataValidator(this)
					.ValidateCreate(spaceData)
					.ToValidationException()
					.ThrowIfContainsErrors();

				var spaceDataList = GetSpaceDataList(spaceData.SpaceId);

				var dbo = ConvertModelToDbo(spaceData);
				dbo.Position = (short)(spaceDataList.Count + 1);

				var success = _dboManager.Insert<TfSpaceDataDbo>(dbo);

				if (!success)
					throw new TfDboServiceException("Insert<TfSpaceDataDbo> failed.");

				if (createSpaceData.Identities.Count > 0)
				{
					var allProviders = GetDataProviders();
					var sharedColumns = GetSharedColumns();

					foreach (var sdIdentity in createSpaceData.Identities)
					{
						foreach (var column in sdIdentity.Columns)
						{
							var columnProvider = allProviders.FirstOrDefault(x => x.Columns.Any(y => y.DbName == column));
							var sharedColumn = sharedColumns.FirstOrDefault(x => x.DbName == column);
							if (columnProvider is null && sharedColumn is null) continue;

							var colSubmit = new TfSpaceDataColumn
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

							AddSpaceDataColumn(spaceData.Id, colSubmit);
						}
					}
				}

				scope.Complete();

				return GetSpaceData(spaceData.Id);
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfSpaceData UpdateSpaceData(
		TfUpdateSpaceData updateSpaceData)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				TfSpaceData spaceData = null;
				TfSpaceDataDbo existingSpaceData = null;

				if (updateSpaceData is not null)
				{
					existingSpaceData = _dboManager.Get<TfSpaceDataDbo>(updateSpaceData.Id);

					spaceData = new TfSpaceData();
					spaceData.Id = updateSpaceData.Id;
					spaceData.DataProviderId = updateSpaceData.DataProviderId;
					spaceData.Name = updateSpaceData.Name;
					spaceData.Filters = updateSpaceData.Filters ?? new List<TfFilterBase>();
					spaceData.Columns = updateSpaceData.Columns ?? new List<string>();
					spaceData.SortOrders = updateSpaceData.SortOrders ?? new List<TfSort>();

					if (existingSpaceData is not null)
						spaceData.SpaceId = existingSpaceData.SpaceId;
				}

				new TfSpaceDataValidator(this)
					.ValidateUpdate(spaceData)
					.ToValidationException()
					.ThrowIfContainsErrors();

				var dbo = ConvertModelToDbo(spaceData);
				dbo.Position = existingSpaceData.Position;

				var success = _dboManager.Update<TfSpaceDataDbo>(dbo);

				if (!success)
					throw new TfDboServiceException("Update<TfSpaceDataDbo> failed.");

				scope.Complete();

				return GetSpaceData(spaceData.Id);
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void DeleteSpaceData(
		Guid id)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var spaceData = GetSpaceData(id);
				new TfSpaceDataValidator(this)
					.ValidateDelete(spaceData)
					.ToValidationException()
					.ThrowIfContainsErrors();

				var spaceDatasForSpace = GetSpaceDataList(spaceData.SpaceId);

				//update positions for spaces after the one being deleted
				foreach (var sd in spaceDatasForSpace.Where(x => x.Position > spaceData.Position))
				{
					sd.Position--;
					var successUpdatePosition = _dboManager.Update<TfSpaceDataDbo>(ConvertModelToDbo(sd));

					if (!successUpdatePosition)
						throw new TfDboServiceException("Update<TfSpaceDataDbo> failed during delete space process.");
				}

				//delete identities
				var spaceDataIdentities = GetSpaceDataIdentities(spaceData.Id);
				foreach (var identity in spaceDataIdentities)
				{
					var successDeleteIdentity = _dboManager.Delete<TfSpaceDataIdentityDbo>(identity.Id);
					if (!successDeleteIdentity)
						throw new TfDboServiceException("Delete<TfSpaceDataIdentityDbo> failed during delete space process.");
				}

				var success = _dboManager.Delete<TfSpaceDataDbo>(id);

				if (!success)
					throw new TfDboServiceException("Delete<TfSpaceDataDbo> failed.");

				scope.Complete();
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfSpaceData CopySpaceData(
		Guid originalId)
	{
		TfSpaceData originalSD = GetSpaceData(originalId);
		if (originalSD is null)
			throw new Exception("Space data not found");

		try
		{
			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var spaceDataList = GetSpaceDataList(originalSD.SpaceId);

				var copyName = getSpaceDataCopyName(originalSD.Name, spaceDataList);
				if (String.IsNullOrWhiteSpace(copyName))
					throw new Exception("Space data copy name could not be generated");

				TfSpaceData spaceData = new()
				{
					Id = Guid.NewGuid(),
					DataProviderId = originalSD.DataProviderId,
					Name = copyName,
					SpaceId = originalSD.SpaceId,
					Filters = originalSD.Filters ?? new List<TfFilterBase>(),
					Columns = originalSD.Columns ?? new List<string>(),
					SortOrders = originalSD.SortOrders ?? new List<TfSort>(),
					Identities = originalSD.Identities,
				};
				new TfSpaceDataValidator(this)
					.ValidateCreate(spaceData)
					.ToValidationException()
					.ThrowIfContainsErrors();

				var dbo = ConvertModelToDbo(spaceData);
				dbo.Position = (short)(spaceDataList.Count + 1);

				var success = _dboManager.Insert<TfSpaceDataDbo>(dbo);

				if (!success)
					throw new TfDboServiceException("Insert<TfSpaceDataDbo> failed.");

				scope.Complete();

				return GetSpaceData(spaceData.Id);
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}


	public void MoveSpaceDataUp(
		Guid id)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var spaceData = GetSpaceData(id);
				if (spaceData == null)
					throw new TfException("Found no space data for specified identifier.");

				var spaceDataList = GetSpaceDataList(spaceData.SpaceId);

				if (spaceData.Position == 1)
					return;

				var prevSpaceData = spaceDataList.SingleOrDefault(x => x.Position == (spaceData.Position - 1));

				spaceData.Position = (short)(spaceData.Position - 1);

				if (prevSpaceData != null)
					prevSpaceData.Position = (short)(prevSpaceData.Position + 1);

				var success = _dboManager.Update<TfSpaceDataDbo>(ConvertModelToDbo(spaceData));

				if (!success)
					throw new TfDboServiceException("Update<TfSpaceDataDbo> failed.");

				if (prevSpaceData != null)
				{
					success = _dboManager.Update<TfSpaceDataDbo>(ConvertModelToDbo(prevSpaceData));

					if (!success)
						throw new TfDboServiceException("Update<TfSpaceDataDbo> failed.");
				}

				scope.Complete();
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void MoveSpaceDataDown(
		Guid id)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var spaceData = GetSpaceData(id);
				if (spaceData == null)
					throw new TfException("Found no space data for specified identifier.");

				var spaceDataList = GetSpaceDataList(spaceData.SpaceId);


				if (spaceData.Position == spaceDataList.Count)
					return;

				var nextSpaceData = spaceDataList.SingleOrDefault(x => x.Position == (spaceData.Position + 1));

				spaceData.Position = (short)(spaceData.Position + 1);

				if (nextSpaceData != null)
					nextSpaceData.Position = (short)(nextSpaceData.Position - 1);

				var success = _dboManager.Update<TfSpaceDataDbo>(ConvertModelToDbo(spaceData));

				if (!success)
					throw new TfDboServiceException("Update<TfSpaceDataDbo> failed.");

				if (nextSpaceData != null)
				{
					success = _dboManager.Update<TfSpaceDataDbo>(ConvertModelToDbo(nextSpaceData));

					if (!success)
						throw new TfDboServiceException("Update<TfSpaceDataDbo> failed.");
				}

				scope.Complete();
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}


	public List<TfSpaceDataColumn> GetSpaceDataColumns(Guid spaceDataId)
	{
		var spaceData = GetSpaceData(spaceDataId);
		if (spaceData is null) throw new Exception("Space data not found");
		var result = new List<TfSpaceDataColumn>();
		var allColumns = GetSpaceDataColumnOptions(spaceDataId);
		var spaceDataColumns = spaceData.Columns.ToList();
		foreach (var identity in spaceData.Identities)
		{
			foreach (var column in identity.Columns)
			{
				spaceDataColumns.Add($"{identity.DataIdentity}.{column}");
			}
		}

		foreach (var item in spaceDataColumns)
		{
			TfSpaceDataColumn? column = null;
			column = allColumns.FirstOrDefault(x => x.ColumnName == item);
			if (column is null)
			{
				result.Add(new TfSpaceDataColumn
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

	public List<TfSpaceDataColumn> GetSpaceDataColumnOptions(Guid spaceDataId)
	{
		var spaceData = GetSpaceData(spaceDataId);
		if (spaceData is null) throw new Exception("Space data not found");
		var provider = GetDataProvider(spaceData.DataProviderId);
		var auxDataSchema = GetDataProviderAuxDataSchema(provider.Id);

		var result = new List<TfSpaceDataColumn>();

		foreach (var providerColumn in provider.Columns)
		{
			var item = new TfSpaceDataColumn
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

		foreach (var dataIdentity in auxDataSchema.DataIdentities)
		{
			foreach (var schemaColumn in dataIdentity.Columns)
			{
				var item = new TfSpaceDataColumn
				{
					DataIdentity = schemaColumn.DataIdentity?.DataIdentity,
					ColumnName = schemaColumn.DbName,
				};
				if (schemaColumn.SharedColumn is not null)
				{
					item.ColumnName = $"{dataIdentity.DataIdentity.DataIdentity}.{schemaColumn.SharedColumn.DbName}";
					item.SourceName = dataIdentity.DataIdentity.DataIdentity;
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

	public void AddSpaceDataColumn(Guid spaceDataId, TfSpaceDataColumn column)
	{
		if (column is null)
			new ArgumentException(nameof(column), "column is required");

		if (String.IsNullOrWhiteSpace(column!.ColumnName))
			new ArgumentException(nameof(column), "column name is required");

		var spaceData = GetSpaceData(spaceDataId);
		if (spaceData is null)
			new TfException("spaceData not found");

		if (column!.SourceType == TfAuxDataSourceType.PrimatyDataProvider)
		{
			if (!spaceData!.Columns.Any(x => x.ToLowerInvariant() == column!.ColumnName!.ToLowerInvariant()))
			{
				var submit = new TfUpdateSpaceData
				{
					Id = spaceData.Id,
					Columns = spaceData.Columns,
					DataProviderId = spaceData.DataProviderId,
					Filters = spaceData.Filters,
					Name = spaceData.Name,
					SortOrders = spaceData.SortOrders,
				};
				submit.Columns.Add(column.ColumnName!);
				var updatedSpaceData = UpdateSpaceData(submit);
			}
		}
		else if (column.SourceType == TfAuxDataSourceType.AuxDataProvider
		|| column.SourceType == TfAuxDataSourceType.SharedColumn)
		{
			if (String.IsNullOrWhiteSpace(column.DataIdentity))
				throw new Exception("column Data Identity is required");

			if (column.SourceColumnName is null)
				throw new Exception("column SourceColumnName is required");

			var dataIdentity = spaceData!.Identities.FirstOrDefault(x => x.DataIdentity == column!.DataIdentity);
			if (dataIdentity is null)
			{
				CreateSpaceDataIdentity(new TfSpaceDataIdentity
				{
					Id = Guid.NewGuid(),
					SpaceDataId = spaceDataId,
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
				UpdateSpaceDataIdentity(new TfSpaceDataIdentity
				{
					Id = dataIdentity.Id,
					SpaceDataId = dataIdentity.SpaceDataId,
					Columns = dataIdentity.Columns,
					DataIdentity = dataIdentity.DataIdentity,
				});
			}
		}

	}
	public void RemoveSpaceDataColumn(Guid spaceDataId, TfSpaceDataColumn column)
	{
		if (column is null)
			new ArgumentException(nameof(column), "column is required");

		if (String.IsNullOrWhiteSpace(column!.ColumnName))
			new ArgumentException(nameof(column), "column name is required");

		var spaceData = GetSpaceData(spaceDataId);
		if (spaceData is null)
			new TfException("spaceData not found");

		if (column.SourceType == TfAuxDataSourceType.PrimatyDataProvider)
		{
			if (spaceData!.Columns.Any(x => x.ToLowerInvariant() == column!.ColumnName!.ToLowerInvariant()))
			{
				var submit = new TfUpdateSpaceData
				{
					Id = spaceData.Id,
					Columns = spaceData.Columns,
					DataProviderId = spaceData.DataProviderId,
					Filters = spaceData.Filters,
					Name = spaceData.Name,
					SortOrders = spaceData.SortOrders,
				};
				submit.Columns = spaceData.Columns.Where(x => x.ToLowerInvariant() != column!.ColumnName!.ToLowerInvariant()).ToList();
				var updatedSpaceData = UpdateSpaceData(submit);
			}
		}
		else if (column.SourceType == TfAuxDataSourceType.AuxDataProvider
			|| column.SourceType == TfAuxDataSourceType.SharedColumn)
		{
			if (String.IsNullOrWhiteSpace(column.DataIdentity))
				throw new Exception("column Data Identity is required");

			if (column.SourceColumnName is null)
				throw new Exception("column SourceColumnName is required");

			var dataIdentity = spaceData!.Identities.FirstOrDefault(x => x.DataIdentity == column!.DataIdentity);
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
					UpdateSpaceDataIdentity(new TfSpaceDataIdentity
					{
						Id = dataIdentity.Id,
						SpaceDataId = dataIdentity.SpaceDataId,
						Columns = dataIdentity.Columns,
						DataIdentity = dataIdentity.DataIdentity,
					});
				}
				else
				{
					DeleteSpaceDataIdentity(dataIdentity.Id);
				}
			}
		}

	}

	//Filters
	public void UpdateSpaceDataFilters(Guid spaceDataId,
			List<TfFilterBase> filters)
	{
		var spaceData = GetSpaceData(spaceDataId);
		if (spaceData is null)
			throw new TfException("spaceData not found");
		var submit = new TfUpdateSpaceData(spaceData);
		submit.Filters = filters;
		var updatedSpaceData = UpdateSpaceData(submit);
	}

	//Sorts
	public void UpdateSpaceDataSorts(Guid spaceDataId,
			List<TfSort> sorts)
	{
		var spaceData = GetSpaceData(spaceDataId);
		if (spaceData is null)
			throw new TfException("spaceData not found");
		var submit = new TfUpdateSpaceData(spaceData);
		submit.SortOrders = sorts;
		var updatedSpaceData = UpdateSpaceData(submit);
	}

	private TfSpaceData ConvertDboToModel(TfSpaceDataDbo dbo)
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

		return new TfSpaceData
		{
			Id = dbo.Id,
			DataProviderId = dbo.DataProviderId,
			Name = dbo.Name,
			Position = dbo.Position,
			SpaceId = dbo.SpaceId,
			Filters = JsonSerializer.Deserialize<List<TfFilterBase>>(dbo.FiltersJson, jsonOptions),
			Columns = JsonSerializer.Deserialize<List<string>>(dbo.ColumnsJson, jsonOptions),
			SortOrders = JsonSerializer.Deserialize<List<TfSort>>(dbo.SortOrdersJson, jsonOptions)
		};

	}

	private TfSpaceDataDbo ConvertModelToDbo(TfSpaceData model)
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

		return new TfSpaceDataDbo
		{
			Id = model.Id,
			DataProviderId = model.DataProviderId,
			Name = model.Name,
			Position = model.Position,
			SpaceId = model.SpaceId,
			FiltersJson = JsonSerializer.Serialize(model.Filters ?? new List<TfFilterBase>(), jsonOptions),
			ColumnsJson = JsonSerializer.Serialize(model.Columns ?? new List<string>(), jsonOptions),
			SortOrdersJson = JsonSerializer.Serialize(model.SortOrders ?? new List<TfSort>(), jsonOptions)
		};
	}


	#region <--- validation --->

	internal class TfSpaceDataValidator
	: AbstractValidator<TfSpaceData>
	{
		public TfSpaceDataValidator(
			ITfService tfService)
		{

			RuleSet("general", () =>
			{
				RuleFor(spaceData => spaceData.Id)
					.NotEmpty()
					.WithMessage("The space data id is required.");

				RuleFor(spaceData => spaceData.Name)
					.NotEmpty()
					.WithMessage("The space data name is required.");

				RuleFor(spaceData => spaceData.SpaceId)
					.NotEmpty()
					.WithMessage("The space id is required.");

				RuleFor(spaceData => spaceData.SpaceId)
					.Must(spaceId =>
					{
						return tfService.GetSpace(spaceId) != null;
					})
					.WithMessage("There is no existing space for specified space id.");

				RuleFor(spaceData => spaceData.DataProviderId)
					.NotEmpty()
					.WithMessage("The data provider is required.");

				RuleFor(spaceData => spaceData.DataProviderId)
					.Must(providerId =>
					{
						return tfService.GetDataProvider(providerId) != null;
					})
					.WithMessage("There is no existing data provider for specified provider id.");

				RuleFor(spaceData => spaceData.Columns)
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
				RuleFor(spaceData => spaceData.Id)
						.Must((spaceData, id) => { return tfService.GetSpaceData(id) == null; })
						.WithMessage("There is already existing space data with specified identifier.");

				//RuleFor(spaceData => spaceData.Name)
				//		.Must((spaceData, name) =>
				//		{
				//			if (string.IsNullOrEmpty(name))
				//				return true;

				//			var spaceDataList = spaceManager.GetSpaceDataList(spaceData.SpaceId).Value;
				//			return !spaceDataList.Any(x => x.Name.ToLowerInvariant().Trim() == name.ToLowerInvariant().Trim());
				//		})
				//		.WithMessage("There is already existing space data with same name in same space.");
			});

			RuleSet("update", () =>
			{
				RuleFor(spaceData => spaceData.Id)
						.Must((spaceData, id) =>
						{
							return tfService.GetSpaceData(id) != null;
						})
						.WithMessage("There is not existing space data with specified identifier.");

				RuleFor(spaceData => spaceData.SpaceId)
					.Must((spaceData, spaceId) =>
					{
						var existingSpaceData = tfService.GetSpaceData(spaceData.Id);
						if (existingSpaceData == null)
							return true;

						return existingSpaceData.SpaceId == spaceId;
					})
					.WithMessage("Space cannot be changed for space data.");

			});

			RuleSet("delete", () =>
			{
			});

		}

		public ValidationResult ValidateCreate(
			TfSpaceData spaceData)
		{
			if (spaceData == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The space data is null.") });

			return this.Validate(spaceData, options =>
			{
				options.IncludeRuleSets("general", "create");
			});
		}

		public ValidationResult ValidateUpdate(
			TfSpaceData spaceData)
		{
			if (spaceData == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The space data is null.") });

			return this.Validate(spaceData, options =>
			{
				options.IncludeRuleSets("general", "update");
			});
		}

		public ValidationResult ValidateDelete(
			TfSpaceData spaceData)
		{
			if (spaceData == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The space data with specified identifier is not found.") });

			return this.Validate(spaceData, options =>
			{
				options.IncludeRuleSets("delete");
			});
		}
	}

	#endregion

	#region << Private >>
	private string? getSpaceDataCopyName(string originalName, List<TfSpaceData> spaceDataList)
	{
		var index = 1;
		var presentNamesHS = spaceDataList.Select(x => x.Name).ToHashSet();

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
