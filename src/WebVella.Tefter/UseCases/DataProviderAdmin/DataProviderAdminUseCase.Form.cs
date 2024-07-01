namespace WebVella.Tefter.UseCases.DataProviderAdmin;
public partial class DataProviderAdminUseCase
{

	internal List<TucDataProviderTypeInfo> AllProviderTypes { get; set; } = new();
	internal TucDataProviderForm Form
	{
		get
		{
			return form;
		}
		set
		{
			form = value;
			ConsoleExt.WriteLine(System.Text.Json.JsonSerializer.Serialize(form));
		}
	}


	private TucDataProviderForm form;
	internal void InitForm()
	{
		Form = new TucDataProviderForm();
		var serviceResult = _dataProviderManager.GetProviderTypes();
		if (serviceResult.IsFailed) throw new Exception("GetProviderTypes failed");
		if (serviceResult.Value is not null)
		{
			AllProviderTypes = serviceResult.Value.Select(t => new TucDataProviderTypeInfo(t)).ToList();
		}
	}

	internal Result<TucDataProvider> CreateDataProviderWithForm()
	{
		var providerTypesResult = _dataProviderManager.GetProviderTypes();
		if (providerTypesResult.IsFailed) return Result.Fail(new Error("GetProviderTypes failed").CausedBy(providerTypesResult.Errors));
		var submitForm = Form.ToModel(providerTypesResult.Value);
		var createResult = _dataProviderManager.CreateDataProvider(submitForm);
		if (createResult.IsFailed) return Result.Fail(new Error("CreateDataProvider failed").CausedBy(createResult.Errors));
		if (createResult.Value is null) return Result.Fail(new Error("CreateDataProvider returned null object").CausedBy(createResult.Errors));
		return Result.Ok(new TucDataProvider(createResult.Value));
	}

	internal Result<TucDataProvider> UpdateDataProviderWithForm()
	{
		var providerTypesResult = _dataProviderManager.GetProviderTypes();
		if (providerTypesResult.IsFailed) return Result.Fail(new Error("GetProviderTypes failed").CausedBy(providerTypesResult.Errors));
		var submitForm = Form.ToModel(providerTypesResult.Value);
		var updateResult = _dataProviderManager.UpdateDataProvider(submitForm);
		if (updateResult.IsFailed) return Result.Fail(new Error("UpdateDataProvider failed").CausedBy(updateResult.Errors));
		if (updateResult.Value is null) return Result.Fail(new Error("UpdateDataProvider returned null object").CausedBy(updateResult.Errors));
		return Result.Ok(new TucDataProvider(updateResult.Value));
	}
}
