﻿using System.Text.Json.Serialization.Metadata;
using WebVella.Tefter.Models;

namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	public List<TfSpaceData> GetAllSpaceData();

	public List<TfSpaceData> GetSpaceDataList(
		Guid spaceId);

	public TfSpaceData GetSpaceData(
		Guid id);

	public TfSpaceData CreateSpaceData(
		TfCreateSpaceData spaceData);

	public TfSpaceData UpdateSpaceData(
		TfUpdateSpaceData spaceData);

	public void DeleteSpaceData(
		Guid id);

	public void MoveSpaceDataUp(
		Guid id);

	public void MoveSpaceDataDown(
		Guid id);
	public List<TfAvailableSpaceDataColumn> GetSpaceDataAvailableColumns(
		Guid spaceDataId);
}

public partial class TfService : ITfService
{
	public List<TfSpaceData> GetAllSpaceData()
	{
		try
		{
			var dbos = _dboManager.GetList<TfSpaceDataDbo>();

			var spaceDatas = dbos.Select(x => ConvertDboToModel(x)).ToList();
			foreach(var spaceData in spaceDatas)
			{
				spaceData.Identities = new ReadOnlyCollection<TfSpaceDataIdentity>(
					GetSpaceDataIdentities(spaceData.Id).ToList());
			}
			return spaceDatas;
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public List<TfSpaceData> GetSpaceDataList(
		Guid spaceId)
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
			return spaceDatas;
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
			if(dbo == null)
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

			foreach (var joinKey in provider.JoinKeys)
			{
				columns.Add(new TfAvailableSpaceDataColumn
				{
					DbName = joinKey.DbName,
					DbType = TfDatabaseColumnType.Guid,
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
		try
		{
			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				TfSpaceData spaceData = null;

				if (createSpaceData is not null)
				{
					spaceData = new TfSpaceData();
					spaceData.Id = createSpaceData.Id;
					spaceData.DataProviderId = createSpaceData.DataProviderId;
					spaceData.Name = createSpaceData.Name;
					spaceData.SpaceId = createSpaceData.SpaceId;
					spaceData.Filters = createSpaceData.Filters ?? new List<TfFilterBase>();
					spaceData.Columns = createSpaceData.Columns ?? new List<string>();
					spaceData.SortOrders = createSpaceData.SortOrders ?? new List<TfSort>();
					
					if (spaceData.Id == Guid.Empty)
						spaceData.Id = Guid.NewGuid();
				}

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
				foreach(var identity in spaceDataIdentities)
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
}
