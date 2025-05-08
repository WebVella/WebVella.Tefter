using DocumentFormat.OpenXml.Wordprocessing;
using System.Diagnostics.Eventing.Reader;

namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	Task<TfRecipeResult> ApplyRecipeAsync(ITfRecipeAddon recipe);
}

public partial class TfService : ITfService
{
	public async Task<TfRecipeResult> ApplyRecipeAsync(ITfRecipeAddon recipe)
	{
		var result = new TfRecipeResult();
		var blobIdList = new List<Guid>();
		try
		{
			if (recipe == null) throw new ArgumentNullException(nameof(recipe));
			using (TfDatabaseTransactionScope scope = _dbService.CreateTransactionScope())
			{
				foreach (var step in recipe.Steps)
				{
					result.Steps.Add(await ApplyStep(step, blobIdList));
				}
				if (result.IsSuccessful)
				{
					var installData = new TfInstallData
					{
						RecipeId = recipe.Id,
						RecipeTypeFullName = recipe.GetType().FullName,
						AppliedOn = DateTime.Now
					};
					var setting = new TfSetting
					{
						Key = TfConstants.InstallDataKey,
						Value = JsonSerializer.Serialize(installData)
					};
					SaveSetting(setting);
					scope.Complete();
				}
			}
			return result;
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
		finally
		{
			if (!result.IsSuccessful && blobIdList.Count > 0)
			{
				foreach (var blobId in blobIdList)
				{
					DeleteBlob(blobId);
				}
			}
		}
	}

	public async Task<TfRecipeStepResult> ApplyStep(TfRecipeStepBase stepBase, List<Guid> blobIdList)
	{
		var stepResult = new TfRecipeStepResult()
		{
			StepId = stepBase.StepId,
			Errors = new()
		};
		try
		{
			if (stepBase.GetType() == typeof(TfInfoRecipeStep))
			{
				//nothing to be done
			}
			else if (stepBase.GetType() == typeof(TfGroupRecipeStep))
			{
				var step = (TfGroupRecipeStep)stepBase;
				foreach (var substep in step.Steps)
				{
					var subStepResult = await ApplyStep(substep, blobIdList);
					stepResult.SubSteps.Add(subStepResult);
					stepResult.Errors.AddRange(subStepResult.Errors);
				}
			}
			else if (stepBase.GetType() == typeof(TfCreateRoleRecipeStep))
			{
				var step = (TfCreateRoleRecipeStep)stepBase;
				var result = await CreateRoleAsync(new TfRole
				{
					Id = step.RoleId == Guid.Empty ? Guid.NewGuid() : step.RoleId,
					IsSystem = false,
					Name = step.Name
				});
			}
			else if (stepBase.GetType() == typeof(TfCreateUserRecipeStep))
			{
				var step = (TfCreateUserRecipeStep)stepBase;
				var allRoles = await GetRolesAsync();
				var allRolesFound = true;
				foreach (var roleId in step.Roles)
				{
					if (!allRoles.Any(x => x.Id == roleId))
					{
						allRolesFound = false;
						break;
					}
				}
				if (!allRolesFound)
				{
					var valEx = new TfValidationException();
					valEx.AddValidationError(nameof(TfCreateUserRecipeStep.Roles), "role not found");
					throw valEx;
				}
				var stepRoles = allRoles.Where(x => step.Roles.Contains(x.Id)).ToList();
				var result = await CreateUserAsync(new TfUser
				{
					Id = step.UserId == Guid.Empty ? Guid.NewGuid() : step.UserId,
					CreatedOn = DateTime.Now,
					Email = step.Email,
					Roles = stepRoles.AsReadOnly(),
					Password = step.Password,
					Enabled = true,
					FirstName = step.FirstName,
					LastName = step.LastName,
					Settings = new TfUserSettings()
				});
			}
			else if (stepBase.GetType() == typeof(TfCreateBlobRecipeStep))
			{
				var step = (TfCreateBlobRecipeStep)stepBase;
				var fileLocalPath = step.Assembly.GetFileFromResourceAndUploadLocally(step.EmbeddedResourceName);
				var blobId = step.BlobId == Guid.Empty ? Guid.NewGuid() : step.BlobId;
				CreateBlob(
					blobId: blobId,
					localPath: fileLocalPath,
					temporary: step.IsTemporary
				);
				blobIdList.Add(blobId);
			}
			else if (stepBase.GetType() == typeof(TfCreateRepositoryFileRecipeStep))
			{
				var step = (TfCreateRepositoryFileRecipeStep)stepBase;
				var fileLocalPath = step.Assembly.GetFileFromResourceAndUploadLocally(step.EmbeddedResourceName);
				var repFile = CreateRepositoryFile(
					filename: step.FileName,
					localPath: fileLocalPath,
					createdBy: null
				);
				var blobId = repFile.Id;
				blobIdList.Add(blobId);
			}
			else if (stepBase.GetType() == typeof(TfCreateDataProviderRecipeStep))
			{
				var step = (TfCreateDataProviderRecipeStep)stepBase;
				var dataProvider = CreateDataProvider(new TfDataProviderModel
				{
					Id = step.DataProviderId == Guid.Empty ? Guid.NewGuid() : step.DataProviderId,
					Index = step.DataProviderIndex,
					Name = step.Name,
					ProviderType = step.Type,
					SettingsJson = step.SettingsJson,
					SynchPrimaryKeyColumns = new List<string>(),
					SynchScheduleEnabled = step.SynchScheduleEnabled,
					SynchScheduleMinutes = step.SynchScheduleMinutes
				});
				var dpPrefix = $"dp{dataProvider.Index}_";
				if (step.SynchPrimaryKeyColumns.Count > 0)
				{
					var updateModel = new TfDataProviderModel()
					{
						Id = dataProvider.Id,
						Index = dataProvider.Index,
						Name = dataProvider.Name,
						ProviderType = dataProvider.ProviderType,
						SettingsJson = dataProvider.SettingsJson,
						SynchScheduleEnabled = dataProvider.SynchScheduleEnabled,
						SynchScheduleMinutes = dataProvider.SynchScheduleMinutes,
						SynchPrimaryKeyColumns = new()
					};
					foreach (var columnName in step.SynchPrimaryKeyColumns)
					{
						var fixedColumnName = columnName;
						if (!fixedColumnName.StartsWith(dpPrefix))
							fixedColumnName = dpPrefix + fixedColumnName;

						updateModel.SynchPrimaryKeyColumns.Add(fixedColumnName);
					}

					dataProvider = UpdateDataProvider(updateModel);
				}

				if (step.Columns.Count > 0)
				{
					//Add prefix if needed
					step.Columns.ForEach(x => x.FixProviderPrefix(dpPrefix));
					dataProvider = CreateBulkDataProviderColumn(dataProvider.Id, step.Columns);
					if (step.TriggerDataSynchronization)
					{
						CreateSynchronizationTask(dataProvider.Id, new TfSynchronizationPolicy());
					}
				}

				foreach (var joinKey in step.JoinKeys)
				{
					joinKey.FixProviderPrefix(dpPrefix);
					var keyColumns = new List<TfDataProviderColumn>();
					foreach (var columnName in joinKey.Columns)
					{
						var dpColumn = dataProvider.Columns.FirstOrDefault(x => x.DbName == columnName);
						if (dpColumn is null) continue;
						keyColumns.Add(dpColumn);
					}
					var keySM = new TfDataProviderJoinKey
					{
						Id = joinKey.Id,
						DataProviderId = dataProvider.Id,
						DbName = joinKey.DbName,
						Description = joinKey.Description,
						LastModifiedOn = joinKey.LastModifiedOn,
						Version = joinKey.Version,
						Columns = keyColumns
					};
					var result = CreateDataProviderJoinKey(keySM);
				}

			}
			else if (stepBase.GetType() == typeof(TfCreateSpaceRecipeStep))
			{
				var step = (TfCreateSpaceRecipeStep)stepBase;
				var allRoles = GetRoles();
				var stepRoles = allRoles.Where(x => step.Roles.Contains(x.Id)).ToList();
				var result = CreateSpace(new TfSpace
				{
					Id = step.SpaceId == Guid.Empty ? Guid.NewGuid() : step.SpaceId,
					Name = step.Name,
					Color = (short)(step.Color is null ? TfColor.Emerald500 : step.Color),
					FluentIconName = !String.IsNullOrWhiteSpace(step.FluentIconName) ? step.FluentIconName : "Apps",
					IsPrivate = step.IsPrivate,
					Position = step.Position,
					Roles = stepRoles
				});
			}
			else if (stepBase.GetType() == typeof(TfCreateSpaceDataRecipeStep))
			{
				var step = (TfCreateSpaceDataRecipeStep)stepBase;
				if (step.Filters.Count > 0 || step.SortOrders.Count > 0)
				{
					var dataProvider = GetDataProvider(step.DataProviderId);
					var dpPrefix = $"dp{dataProvider.Index}_";
					step.Filters.ForEach(x => x.FixProviderPrefix(dpPrefix));
					step.SortOrders.ForEach(x => x.FixProviderPrefix(dpPrefix));

				}
				var result = CreateSpaceData(new TfSpaceData
				{
					Id = step.SpaceDataId == Guid.Empty ? Guid.NewGuid() : step.SpaceDataId,
					SpaceId = step.SpaceId,
					DataProviderId = step.DataProviderId,
					Name = step.Name,
					Columns = step.Columns,
					Position = step.Position,
					Filters = step.Filters,
					SortOrders = step.SortOrders,
				});
			}
			else if (stepBase.GetType() == typeof(TfCreateSpaceViewRecipeStep))
			{
				var step = (TfCreateSpaceViewRecipeStep)stepBase;
				var spaceData = GetSpaceData(step.SpaceDataId);
				var dataProvider = GetDataProvider(spaceData.DataProviderId);
				var dpPrefix = $"dp{dataProvider.Index}_";
				if (step.Presets is not null)
				{
					foreach (var preset in step.Presets)
					{
						preset.Filters.ForEach(x => x.FixProviderPrefix(dpPrefix));
						preset.SortOrders.ForEach(x => x.FixProviderPrefix(dpPrefix));
					}
				}
				var spaceView = CreateSpaceView(new TfSpaceView
				{
					Id = step.SpaceViewId == Guid.Empty ? Guid.NewGuid() : step.SpaceViewId,
					SpaceId = step.SpaceId,
					Name = step.Name,
					Position = step.Position,
					SpaceDataId = step.SpaceDataId,
					Type = TfSpaceViewType.DataGrid,
					Presets = step.Presets,
					SettingsJson = step.Settings is not null ? JsonSerializer.Serialize(step.Settings) : "{}",
				});
				;
				foreach (var column in step.Columns)
				{
					column.FixProviderPrefix(dpPrefix);
					var columnResult = CreateSpaceViewColumn(column);
				}
			}
			else if (stepBase.GetType() == typeof(TfCreateSpacePageRecipeStep))
			{
				var step = (TfCreateSpacePageRecipeStep)stepBase;
				var result = CreateSpacePage(new TfSpacePage
				{
					Id = step.SpacePageId == Guid.Empty ? Guid.NewGuid() : step.SpacePageId,
					SpaceId = step.SpaceId,
					Name = step.Name,
					Position = step.Position,
					Type = step.Type,
					ComponentType = step.ComponentType.GetType(),
					ComponentId = step.ComponentId,
					ComponentOptionsJson = step.ComponentOptionsJson,
					ChildPages = step.ChildPages,
					FluentIconName = step.FluentIconName
				});
			}
			else if (stepBase.GetType() == typeof(TfCreateTemplateRecipeStep))
			{
				var step = (TfCreateTemplateRecipeStep)stepBase;
				var result = CreateTemplate(new TfManageTemplateModel
				{
					Id = step.TemplateId == Guid.Empty ? Guid.NewGuid() : step.TemplateId,
					Name = step.Name,
					FluentIconName = step.FluentIconName,
					ContentProcessorType = step.ContentProcessorType.GetType(),
					Description = step.Description,
					IsEnabled = step.IsEnabled,
					IsSelectable = step.IsSelectable,
					SettingsJson = step.SettingsJson,
					SpaceDataList = step.SpaceDataList,
					UserId = null
				});
			}
			else throw new Exception("Unsupported step type");
			stepResult.IsCompleted = true;
			stepResult.IsSuccessful = !stepResult.Errors.Any();
		}
		catch (TfValidationException ex)
		{
			stepResult.IsCompleted = true;
			stepResult.IsSuccessful = false;
			var errors = ex.GetDataAsValidationErrorList();
			if (errors is not null && errors.Count > 0)
			{
				foreach (ValidationError error in errors)
				{
					stepResult.Errors.Add(new TfRecipeStepResultError()
					{
						StepId = stepBase.StepId,
						StepName = stepBase.StepMenuTitle,
						PropName = String.Empty,
						Message = $"{error.PropertyName}: {error.Message}",
					});
				}
			}
			else
			{
				stepResult.Errors.Add(new TfRecipeStepResultError()
				{
					Message = ex.Message,
					StackTrace = ex.StackTrace
				});
			}

		}
		catch (Exception ex)
		{
			stepResult.IsCompleted = true;
			stepResult.IsSuccessful = false;
			stepResult.Errors.Add(new TfRecipeStepResultError()
			{
				Message = ex.Message,
				StackTrace = ex.StackTrace
			});
		}
		return stepResult;
	}
}
