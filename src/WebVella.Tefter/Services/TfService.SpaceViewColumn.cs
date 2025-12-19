namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	public ReadOnlyCollection<ITfSpaceViewColumnTypeAddon> GetAvailableSpaceViewColumnTypes();

	public List<TfSpaceViewColumn> GetSpaceViewColumnsList(
		Guid spaceViewId);

	public TfSpaceViewColumn GetSpaceViewColumn(
		Guid id);

	public Task<TfSpaceViewColumn> CreateSpaceViewColumn(
		TfSpaceViewColumn spaceViewColumn);

	public Task<TfSpaceViewColumn> UpdateSpaceViewColumn(
		TfSpaceViewColumn spaceViewColumn);

	public Task DeleteSpaceViewColumn(
		Guid id);

	public Task MoveSpaceViewColumnUp(
		Guid id);

	public Task MoveSpaceViewColumnDown(
		Guid id);

	ReadOnlyCollection<ITfSpaceViewColumnTypeAddon> GetCompatibleViewColumnTypesMeta(
		TfSpaceViewColumn viewColumn);

	Task<(bool, List<TfSpaceViewColumn>)> ImportMissingColumnsFromDataset(
		Guid spaceViewId);
}

public partial class TfService : ITfService
{
	public ReadOnlyCollection<ITfSpaceViewColumnTypeAddon> GetAvailableSpaceViewColumnTypes()
		=> _metaService.GetSpaceViewColumnTypesMeta();


	public List<TfSpaceViewColumn> GetSpaceViewColumnsList(
		Guid spaceViewId)
	{
		try
		{
			var orderSettings = new TfOrderSettings(
				nameof(TfSpace.Position),
				OrderDirection.ASC);

			var spaceViewColumns = _dboManager.GetList<TfSpaceViewColumnDbo>(
				spaceViewId,
				nameof(TfSpaceViewColumn.SpaceViewId),
				order: orderSettings);

			return spaceViewColumns.Select(x => ConvertDboToModel(x)).ToList();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfSpaceViewColumn GetSpaceViewColumn(
		Guid id)
	{
		try
		{
			var spaceViewColumn = _dboManager.Get<TfSpaceViewColumnDbo>(id);
			return ConvertDboToModel(spaceViewColumn);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public async Task<TfSpaceViewColumn> CreateSpaceViewColumn(
		TfSpaceViewColumn spaceViewColumn)
	{
		try
		{
			if (spaceViewColumn != null && spaceViewColumn.Id == Guid.Empty)
				spaceViewColumn.Id = Guid.NewGuid();

			new TfSpaceViewColumnValidator(this)
				.ValidateCreate(spaceViewColumn)
				.ToValidationException()
				.ThrowIfContainsErrors();

			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var spaceViewColumns = GetSpaceViewColumnsList(spaceViewColumn.SpaceViewId);

				if (spaceViewColumn.Position == null)
				{
					spaceViewColumn.Position = (short)(spaceViewColumns.Count + 1);
				}
				else if (spaceViewColumn.Position.Value > (spaceViewColumns.Count + 1))
				{
					spaceViewColumn.Position = (short)(spaceViewColumns.Count + 1);
				}
				else if (spaceViewColumn.Position.Value <= 0)
				{
					spaceViewColumn.Position = 1;
				}

				//increment column position for columns for same and after new position
				var columnsAfter = spaceViewColumns
					.Where(x => x.Position >= spaceViewColumn.Position).ToList();

				foreach (var columnAfter in columnsAfter)
				{
					columnAfter.Position++;

					var successUpdatePosition =
						_dboManager.Update<TfSpaceViewColumnDbo>(ConvertModelToDbo(columnAfter));
					if (!successUpdatePosition)
						throw new TfDboServiceException("Update<TfSpaceViewColumnDbo> failed");
				}


				var success = _dboManager.Insert<TfSpaceViewColumnDbo>(ConvertModelToDbo(spaceViewColumn));
				if (!success)
					throw new TfDboServiceException("Insert<TfSpaceViewColumnDbo> failed");

				scope.Complete();

				var viewColumns = GetSpaceViewColumnsList(spaceViewColumn.SpaceViewId);
			
				await _eventBus.PublishAsync(
					key: null,
					payload: new TfSpaceViewColumnUpdatedEventPayload(spaceViewColumn.SpaceViewId, viewColumns));				
				return viewColumns.Single(x => x.Id == spaceViewColumn.Id);
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public async Task<TfSpaceViewColumn> UpdateSpaceViewColumn(
		TfSpaceViewColumn spaceViewColumn)
	{
		try
		{
			new TfSpaceViewColumnValidator(this)
				.ValidateUpdate(spaceViewColumn)
				.ToValidationException()
				.ThrowIfContainsErrors();

			var spaceViewColumns = GetSpaceViewColumnsList(spaceViewColumn.SpaceViewId);

			var existingSpaceView = spaceViewColumns.Single(x => x.Id == spaceViewColumn.Id);

			if (spaceViewColumn.Position == null)
			{
				spaceViewColumn.Position = existingSpaceView.Position;
			}
			else if (spaceViewColumn.Position.Value > spaceViewColumns.Count)
			{
				spaceViewColumn.Position = (short)(spaceViewColumns.Count);
			}
			else if (spaceViewColumn.Position.Value < 0)
			{
				spaceViewColumn.Position = 1;
			}

			if (spaceViewColumn.Position.Value != existingSpaceView.Position)
			{
				//decrement column position for columns after old position (move up)
				var columnsAfter = spaceViewColumns
					.Where(x => x.Position > existingSpaceView.Position).ToList();

				foreach (var columnAfter in columnsAfter)
				{
					columnAfter.Position--;

					var successUpdatePosition =
						_dboManager.Update<TfSpaceViewColumnDbo>(ConvertModelToDbo(columnAfter));
					if (!successUpdatePosition)
						throw new TfDboServiceException("Update<TfSpaceViewColumnDbo> failed");
				}

				//increment column position for columns after new position (move down)
				columnsAfter = spaceViewColumns
					.Where(x => x.Position >= spaceViewColumn.Position.Value).ToList();

				foreach (var columnAfter in columnsAfter)
				{
					columnAfter.Position++;

					var successUpdatePosition =
						_dboManager.Update<TfSpaceViewColumnDbo>(ConvertModelToDbo(columnAfter));
					if (!successUpdatePosition)
						throw new TfDboServiceException("Update<TfSpaceViewColumnDbo> failed");
				}
			}


			var success = _dboManager.Update<TfSpaceViewColumnDbo>(ConvertModelToDbo(spaceViewColumn));
			if (!success)
				throw new TfDboServiceException("Update<TfSpaceViewColumnDbo> failed");
			var viewColumns = GetSpaceViewColumnsList(spaceViewColumn.SpaceViewId);
			await _eventBus.PublishAsync(
				key: null,
				payload: new TfSpaceViewColumnUpdatedEventPayload(spaceViewColumn.SpaceViewId, viewColumns));						
			return viewColumns.Single(x => x.Id == spaceViewColumn.Id);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public async Task MoveSpaceViewColumnUp(
		Guid id)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var spaceViewColumn = GetSpaceViewColumn(id);

				if (spaceViewColumn == null)
				{
					throw new TfValidationException(nameof(id),
						"Found no space view column for specified identifier.");
				}

				if (spaceViewColumn.Position == 1)
					return;

				var spaceViewColumns = GetSpaceViewColumnsList(spaceViewColumn.SpaceViewId);

				var prevSpaceViewColumn =
					spaceViewColumns.SingleOrDefault(x => x.Position == (spaceViewColumn.Position - 1));
				spaceViewColumn.Position = (short)(spaceViewColumn.Position - 1);

				if (prevSpaceViewColumn != null)
					prevSpaceViewColumn.Position = (short)(prevSpaceViewColumn.Position + 1);

				var success = _dboManager.Update<TfSpaceViewColumnDbo>(ConvertModelToDbo(spaceViewColumn));

				if (!success)
					throw new TfDboServiceException("Update<TfSpaceViewColumnDbo> failed");

				if (prevSpaceViewColumn != null)
				{
					success = _dboManager.Update<TfSpaceViewColumnDbo>(ConvertModelToDbo(prevSpaceViewColumn));

					if (!success)
						throw new TfDboServiceException("Update<TfSpaceViewColumnDbo> failed");
				}

				scope.Complete();
				var viewColumns = GetSpaceViewColumnsList(spaceViewColumn.SpaceViewId);
				await _eventBus.PublishAsync(
					key: null,
					payload: new TfSpaceViewColumnUpdatedEventPayload(spaceViewColumn.SpaceViewId, viewColumns));							
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public async Task MoveSpaceViewColumnDown(
		Guid id)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var spaceViewColumn = GetSpaceViewColumn(id);

				if (spaceViewColumn == null)
				{
					throw new TfValidationException(nameof(id),
						"Found no space view column for specified identifier.");
				}

				var spaceViewColumns = GetSpaceViewColumnsList(spaceViewColumn.SpaceViewId);

				if (spaceViewColumn.Position == spaceViewColumns.Count)
					return;

				var nextSpaceViewColumn =
					spaceViewColumns.SingleOrDefault(x => x.Position == (spaceViewColumn.Position + 1));
				spaceViewColumn.Position = (short)(spaceViewColumn.Position + 1);

				if (nextSpaceViewColumn != null)
					nextSpaceViewColumn.Position = (short)(nextSpaceViewColumn.Position - 1);

				var success = _dboManager.Update<TfSpaceViewColumnDbo>(ConvertModelToDbo(spaceViewColumn));
				if (!success)
					throw new TfDboServiceException("Update<TfSpaceViewColumnDbo> failed");

				if (nextSpaceViewColumn != null)
				{
					success = _dboManager.Update<TfSpaceViewColumnDbo>(ConvertModelToDbo(nextSpaceViewColumn));
					if (!success)
						throw new TfDboServiceException("Update<TfSpaceViewColumnDbo> failed");
				}

				scope.Complete();

				var viewColumns = GetSpaceViewColumnsList(spaceViewColumn.SpaceViewId);
				await _eventBus.PublishAsync(
					key: null,
					payload: new TfSpaceViewColumnUpdatedEventPayload(spaceViewColumn.SpaceViewId, viewColumns));							
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public async Task DeleteSpaceViewColumn(
		Guid id)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var spaceViewColumn = GetSpaceViewColumn(id);


				new TfSpaceViewColumnValidator(this)
					.ValidateDelete(spaceViewColumn)
					.ToValidationException()
					.ThrowIfContainsErrors();

				var columnsAfter = GetSpaceViewColumnsList(spaceViewColumn.SpaceViewId)
					.Where(x => x.Position > spaceViewColumn.Position).ToList();

				//update positions for space view columns after the one being deleted
				foreach (var columnAfter in columnsAfter)
				{
					columnAfter.Position--;
					var successUpdatePosition =
						_dboManager.Update<TfSpaceViewColumnDbo>(ConvertModelToDbo(columnAfter));
					if (!successUpdatePosition)
						throw new TfDboServiceException("Update<TfSpaceViewColumnDbo> failed");
				}

				var success = _dboManager.Delete<TfSpaceViewColumnDbo>(id);

				if (!success)
					throw new TfDboServiceException("Delete<TfSpaceViewColumnDbo> failed");

				scope.Complete();

				var viewColumns = GetSpaceViewColumnsList(spaceViewColumn.SpaceViewId);
				await _eventBus.PublishAsync(
					key: null,
					payload: new TfSpaceViewColumnUpdatedEventPayload(spaceViewColumn.SpaceViewId, viewColumns));					
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public ReadOnlyCollection<ITfSpaceViewColumnTypeAddon> GetCompatibleViewColumnTypesMeta(
		TfSpaceViewColumn viewColumn)
	{
		var list = new List<ITfSpaceViewColumnTypeAddon>();
		var spaceView = GetSpaceView(viewColumn.SpaceViewId);
		var dataset = GetDataset(spaceView!.DatasetId);
		if (dataset is null) return list.AsReadOnly();
		var sampleData = QueryDataset(dataset.Id, page: 1, pageSize: 1);

		var dataAliasTypeDict = new Dictionary<string, TfDatabaseColumnType?>();
		foreach (var alias in viewColumn.DataMapping.Keys)
		{
			if (String.IsNullOrWhiteSpace(viewColumn.DataMapping[alias]))
			{
				dataAliasTypeDict[alias] = null;
				continue;
			}

			var column = sampleData.Columns[viewColumn.DataMapping[alias]!];
			dataAliasTypeDict[alias] = column?.DbType;
		}

		//The logic is the following:
		//1. The aliases must be matched
		//2. Each alias dbtype must be supported by the components corresponding alias map definition
		foreach (var target in _metaService.GetSpaceViewColumnTypesMeta())
		{
			var isSupported = true;
			foreach (var definition in target.DataMappingDefinitions)
			{
				if (!dataAliasTypeDict.ContainsKey(definition.Alias))
				{
					isSupported = false;
					break;
				}

				if (dataAliasTypeDict[definition.Alias] is null)
					continue;

				var defDbSupport = new TfDbTypeSupportLevel(definition);
				if (!defDbSupport.Supports(dataAliasTypeDict[definition.Alias]!.Value))
				{
					isSupported = false;
					break;
				}
			}

			if (isSupported)
				list.Add(target);
		}

		return list.OrderBy(x => x.AddonName).ToList().AsReadOnly();
	}

	public async Task<(bool, List<TfSpaceViewColumn>)> ImportMissingColumnsFromDataset(
		Guid spaceViewId)
	{
		var spaceView = GetSpaceView(spaceViewId) ?? throw new TfServiceException("Space view not found");
		var dataset = GetDataset(spaceView.DatasetId) ?? throw new TfServiceException("Dataset not found");
		var spaceViewColumns = GetSpaceViewColumnsList(spaceViewId);
		var usedColumns = new HashSet<string>();
		foreach (var column in spaceViewColumns)
		{
			foreach (var alias in column.DataMapping.Keys)
			{
				var columnName = column.DataMapping[alias];
				if (!String.IsNullOrWhiteSpace(columnName))
					usedColumns.Add(columnName);
			}
		}

		var columnNamesToBeCreated = dataset.Columns.Where(x => !usedColumns.Contains(x)).ToList();
		foreach (var identity in dataset.Identities)
		{
			foreach (var column in identity.Columns)
			{
				var columnName = $"{identity.DataIdentity}.{column}";
				if(!usedColumns.Contains(columnName))
					columnNamesToBeCreated.Add(columnName);
			}
		}

		if (columnNamesToBeCreated.Count == 0)
			return (false, spaceViewColumns);

		var dt = QueryDataset(datasetId:dataset.Id, page: 1, pageSize: 1);
		List<TfDataColumn> dataColumnsToBeCreate = new();
		foreach (var columnName in columnNamesToBeCreated)
		{
			var column = dt.Columns[columnName];
			if(column is not null)
				dataColumnsToBeCreate.Add(column);
		}

		var viewColumnTypes = _metaService.GetSpaceViewColumnTypesMeta();
		List<TfSpaceViewColumn> viewColumnsToBeCreated = new();
		foreach (var dbColumn in dataColumnsToBeCreate)
		{
			var viewColumnType = dbColumn.DbType.GetColumnTypeForDbType(viewColumnTypes);
			var dataMapping = new Dictionary<string, string?>();
			if (viewColumnType.DataMappingDefinitions.Count > 0)
			{
				dataMapping[viewColumnType.DataMappingDefinitions[0].Alias] = dbColumn.Name;
			}

			var viewColumn = new TfSpaceViewColumn()
			{
				Id = Guid.NewGuid(),
				Icon = null,
				OnlyIcon = false,
				Position = 0,
				QueryName = NavigatorExt.GenerateQueryName(),
				Settings = new(),
				SpaceViewId = spaceViewId,
				Title = NavigatorExt.ProcessForTitle(dbColumn.Name),
				TypeId = viewColumnType.AddonId,
				TypeOptionsJson = "{}",
				DataMapping = dataMapping
			};
			viewColumnsToBeCreated.Add(viewColumn);
		}

		using (var scope = _dbService.CreateTransactionScope())
		{
			foreach (var column in viewColumnsToBeCreated)
			{
				await CreateSpaceViewColumn(column);
			}
			scope.Complete();
		}

		return (true, GetSpaceViewColumnsList(spaceViewId));
	}

	#region << Private >>

	private TfSpaceViewColumn ConvertDboToModel(TfSpaceViewColumnDbo dbo)
	{
		if (dbo == null)
			return null;
		return new TfSpaceViewColumn
		{
			Id = dbo.Id,
			SpaceViewId = dbo.SpaceViewId,
			QueryName = dbo.QueryName,
			Title = dbo.Title,
			Icon = dbo.Icon ?? string.Empty,
			OnlyIcon = dbo.OnlyIcon,
			Position = dbo.Position,
			TypeId = dbo.TypeId,
			DataMapping =
				JsonSerializer.Deserialize<Dictionary<string, string>>(dbo.DataMappingJson) ??
				new Dictionary<string, string>(),
			TypeOptionsJson = dbo.TypeOptionsJson,
			Settings = !String.IsNullOrWhiteSpace(dbo.SettingsJson) && dbo.SettingsJson.StartsWith("{") &&
			           dbo.SettingsJson.EndsWith("}")
				? (JsonSerializer.Deserialize<TfSpaceViewColumnSettings>(dbo.SettingsJson) ??
				   new TfSpaceViewColumnSettings())
				: new TfSpaceViewColumnSettings()
		};
	}

	private TfSpaceViewColumnDbo ConvertModelToDbo(TfSpaceViewColumn model)
	{
		if (model == null)
			return null;

		return new TfSpaceViewColumnDbo
		{
			Id = model.Id,
			SpaceViewId = model.SpaceViewId,
			QueryName = model.QueryName,
			Title = model.Title,
			Icon = model.Icon ?? string.Empty,
			OnlyIcon = model.OnlyIcon,
			Position = (model.Position ?? 1),
			TypeId = model.TypeId,
			DataMappingJson = JsonSerializer.Serialize(model.DataMapping ?? new Dictionary<string, string>()),
			TypeOptionsJson = model.TypeOptionsJson,
			SettingsJson = JsonSerializer.Serialize(model.Settings),
		};
	}

	#region <--- validation --->

	internal class TfSpaceViewColumnValidator
		: AbstractValidator<TfSpaceViewColumn>
	{
		public TfSpaceViewColumnValidator(
			ITfService tfService)
		{
			RuleSet("general", () =>
			{
				RuleFor(column => column.Id)
					.NotEmpty()
					.WithMessage("The column id is required.");

				RuleFor(column => column.QueryName)
					.NotEmpty()
					.WithMessage("The query name is required.");

				RuleFor(column => column.TypeId)
					.NotEmpty()
					.WithMessage("The column type is required.");

				RuleFor(column => column.SpaceViewId)
					.NotEmpty()
					.WithMessage("The space view id is required.");

				RuleFor(column => column.QueryName)
					.Must((column, QueryName) =>
					{
						if (string.IsNullOrWhiteSpace(QueryName))
							return true;

						return QueryName.Length >= TfConstants.DB_MIN_OBJECT_NAME_LENGTH;
					})
					.WithMessage(
						$"The query name must be at least {TfConstants.DB_MIN_OBJECT_NAME_LENGTH} characters long.");

				RuleFor(column => column.QueryName)
					.Must((column, QueryName) =>
					{
						if (string.IsNullOrWhiteSpace(QueryName))
							return true;

						return QueryName.Length <= TfConstants.DB_MAX_OBJECT_NAME_LENGTH;
					})
					.WithMessage(
						$"The length of query name must be less or equal than {TfConstants.DB_MAX_OBJECT_NAME_LENGTH} characters");

				RuleFor(column => column.QueryName)
					.Must((column, QueryName) =>
					{
						if (string.IsNullOrWhiteSpace(QueryName))
							return true;

						//other validation will trigger
						if (QueryName.Length < TfConstants.DB_MIN_OBJECT_NAME_LENGTH)
							return true;

						//other validation will trigger
						if (QueryName.Length > TfConstants.DB_MAX_OBJECT_NAME_LENGTH)
							return true;

						Match match = Regex.Match(QueryName, TfConstants.DB_OBJECT_NAME_VALIDATION_PATTERN);
						return match.Success && match.Value == QueryName.Trim();
					})
					.WithMessage(
						$"The query name can only contains underscores and lowercase alphanumeric characters. It must begin with a letter, " +
						$"not include spaces, not end with an underscore, and not contain two consecutive underscores");
			});

			RuleSet("create", () =>
			{
				RuleFor(column => column.Id)
					.Must((column, id) => { return tfService.GetSpaceViewColumn(id) == null; })
					.WithMessage("There is already existing space view column with specified identifier.");

				RuleFor(column => column.QueryName)
					.Must((column, queryName) =>
					{
						if (string.IsNullOrEmpty(queryName))
							return true;

						var spaceViewColumns = tfService.GetSpaceViewColumnsList(column.SpaceViewId);
						return !spaceViewColumns.Any(x =>
							x.QueryName.ToLowerInvariant().Trim() == queryName.ToLowerInvariant().Trim());
					})
					.WithMessage("There is already existing space view column with same query name.");
			});

			RuleSet("update", () =>
			{
				RuleFor(column => column.Id)
					.Must((column, id) =>
					{
						return tfService.GetSpaceViewColumn(id) != null;
					})
					.WithMessage("There is not existing space view column with specified identifier.");

				RuleFor(column => column.QueryName)
					.Must((column, queryName) =>
					{
						if (string.IsNullOrEmpty(queryName))
							return true;

						var spaceViewColumns = tfService.GetSpaceViewColumnsList(column.SpaceViewId);
						return !spaceViewColumns.Any(x =>
							x.QueryName.ToLowerInvariant().Trim() == queryName.ToLowerInvariant().Trim() &&
							column.Id != x.Id);
					})
					.WithMessage("There is already another space view column with same query name inside same view.");
			});

			RuleSet("delete", () =>
			{
			});
		}

		public ValidationResult ValidateCreate(
			TfSpaceViewColumn spaceViewColumn)
		{
			if (spaceViewColumn == null)
				return new ValidationResult(new[]
				{
					new ValidationFailure("",
						"The space view column is null.")
				});

			return this.Validate(spaceViewColumn, options =>
			{
				options.IncludeRuleSets("general", "create");
			});
		}

		public ValidationResult ValidateUpdate(
			TfSpaceViewColumn spaceViewColumn)
		{
			if (spaceViewColumn == null)
				return new ValidationResult(new[]
				{
					new ValidationFailure("",
						"The space view column is null.")
				});

			return this.Validate(spaceViewColumn, options =>
			{
				options.IncludeRuleSets("general", "update");
			});
		}

		public ValidationResult ValidateDelete(
			TfSpaceViewColumn spaceViewColumn)
		{
			if (spaceViewColumn == null)
				return new ValidationResult(new[]
				{
					new ValidationFailure("",
						"The space view column with specified identifier is not found.")
				});

			return this.Validate(spaceViewColumn, options =>
			{
				options.IncludeRuleSets("delete");
			});
		}
	}

	#endregion

	#endregion
}