namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	Task<TfRecipeResult> ApplyRecipeAsync(ITfRecipeAddon recipe);
}

public partial class TfService : ITfService
{
	public async Task<TfRecipeResult> ApplyRecipeAsync(ITfRecipeAddon recipe)
	{
		try
		{
			if (recipe == null) throw new ArgumentNullException(nameof(recipe));
			var result = new TfRecipeResult();
			using (TfDatabaseTransactionScope scope = _dbService.CreateTransactionScope())
			{
				foreach (var step in recipe.Steps)
				{
					result.Steps.Add(await ApplyStep(step));
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

	}

	public async Task<TfRecipeStepResult> ApplyStep(TfRecipeStepBase stepBase)
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
					var subStepResult = await ApplyStep(substep);
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
			else if (stepBase.GetType() == typeof(TfCreateRepositoryFileRecipeStep))
			{
				var step = (TfCreateRepositoryFileRecipeStep)stepBase;
				var fileLocalPath = step.Assembly.GetFileFromResourceAndUploadLocally(step.EmbeddedResourceName);
				var result = CreateRepositoryFile(
					filename: step.FileName,
					localPath: fileLocalPath,
					createdBy: null
				);
			}
			else if (stepBase.GetType() == typeof(TfCreateDataProviderRecipeStep))
			{
				var step = (TfCreateDataProviderRecipeStep)stepBase;
				var dataProvider = CreateDataProvider(new TfDataProviderModel
				{
					Id = step.DataProviderId == Guid.Empty ? Guid.NewGuid() : step.DataProviderId,
					Name = step.Name,
					ProviderType = step.Type,
					SettingsJson = step.SettingsJson,
				});
				if (step.Columns.Count > 0)
				{
					//Add prefix if needed
					var dpPrefix = $"dp{dataProvider.Index}_";
					foreach (var column in step.Columns)
					{
						if (!column.DbName.StartsWith(dpPrefix))
							column.DbName = dpPrefix + column.DbName;
					}

					var colResult = CreateBulkDataProviderColumn(dataProvider.Id, step.Columns);
					if (step.ShouldSynchronizeData)
					{
						CreateSynchronizationTask(dataProvider.Id, new TfSynchronizationPolicy());
					}
				}
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
