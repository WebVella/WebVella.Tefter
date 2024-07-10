namespace WebVella.Tefter.UseCases.DataProviderAdmin;
public partial class DataProviderAdminUseCase
{

	internal List<TucDataProviderTypeInfo> AllProviderTypes { get; set; } = new();
	internal TucDataProviderForm ProviderForm { get; set; } = new();
	internal Task InitForProviderManageDialog()
	{
		ProviderForm = new TucDataProviderForm();
		var serviceResult = _dataProviderManager.GetProviderTypes();
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetProviderTypes failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return Task.CompletedTask;
		}
		if (serviceResult.Value is not null)
		{
			AllProviderTypes = serviceResult.Value.Select(t => new TucDataProviderTypeInfo(t)).ToList();
		}
		return Task.CompletedTask;
	}

	internal Result<TucDataProvider> CreateDataProviderWithForm()
	{
		var providerTypesResult = _dataProviderManager.GetProviderTypes();
		if (providerTypesResult.IsFailed) return Result.Fail(new Error("GetProviderTypes failed").CausedBy(providerTypesResult.Errors));
		var submitForm = ProviderForm.ToModel(providerTypesResult.Value);
		var createResult = _dataProviderManager.CreateDataProvider(submitForm);
		if (createResult.IsFailed) return Result.Fail(new Error("CreateDataProvider failed").CausedBy(createResult.Errors));
		if (createResult.Value is null) return Result.Fail(new Error("CreateDataProvider returned null object").CausedBy(createResult.Errors));
		return Result.Ok(new TucDataProvider(createResult.Value));
	}

	internal Result<TucDataProvider> UpdateDataProviderWithForm()
	{
		var providerTypesResult = _dataProviderManager.GetProviderTypes();
		if (providerTypesResult.IsFailed) return Result.Fail(new Error("GetProviderTypes failed").CausedBy(providerTypesResult.Errors));
		var submitForm = ProviderForm.ToModel(providerTypesResult.Value);
		var updateResult = _dataProviderManager.UpdateDataProvider(submitForm);
		if (updateResult.IsFailed) return Result.Fail(new Error("UpdateDataProvider failed").CausedBy(updateResult.Errors));
		if (updateResult.Value is null) return Result.Fail(new Error("UpdateDataProvider returned null object").CausedBy(updateResult.Errors));
		return Result.Ok(new TucDataProvider(updateResult.Value));
	}
}
