namespace WebVella.Tefter.UseCases.Recipe;
internal partial class RecipeUseCase
{
	internal virtual Task<TucInstallData> GetInstallDataAsync()
	{
		var setting = _tfService.GetSetting(TfConstants.InstallDataKey);
		if (setting is null)
			return Task.FromResult((TucInstallData)null);
		TfInstallData data = JsonSerializer.Deserialize<TfInstallData>(setting.Value);
		if (data is null)
			return Task.FromResult((TucInstallData)null);
		return Task.FromResult(new TucInstallData(data));
	}

	internal virtual Task SaveInstallDataAsync(TucInstallData data)
	{
		if (data is null) throw new ArgumentNullException(nameof(data));
		var setting = new TfSetting
		{
			Key = TfConstants.InstallDataKey,
			Value = JsonSerializer.Serialize(data.ToModel())
		};
		_tfService.SaveSetting(setting);
		return Task.CompletedTask;
	}
}
