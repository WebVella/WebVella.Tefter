namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	public List<TfSetting> GetSettings();

	public TfSetting GetSetting(
		string name);

	public void SaveSetting(
		TfSetting setting);

	public void DeleteSetting(
		string name);
}

public partial class TfService : ITfService
{
	public List<TfSetting> GetSettings()
	{
		try
		{
			return _dboManager.GetList<TfSetting>();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfSetting GetSetting(
		string name)
	{
		try
		{
			return _dboManager.Get<TfSetting>(name, nameof(TfSetting.Key));
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void SaveSetting(
		TfSetting setting)
	{
		try
		{
			new TfSettingValidator(this)
				.ValidateSave(setting)
				.ToValidationException()
				.ThrowIfContainsErrors();

			var existingSetting = GetSetting(setting.Key);
			if (existingSetting is not null)
			{
				var success = _dboManager.Update<TfSetting>(nameof(setting.Key),setting);
				if (!success)
					throw new TfDboServiceException("Update<TfSetting> failed.");
			}
			else
			{
				var success = _dboManager.Insert<TfSetting>(setting);
				if (!success)
					throw new TfDboServiceException("Insert<TfSetting> failed.");
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}

	}

	public void DeleteSetting(
		string name)
	{
		try
		{
			var existingSetting = GetSetting(name);

			new TfSettingValidator(this)
				.ValidateDelete(existingSetting)
				.ToValidationException()
				.ThrowIfContainsErrors();

			var success = _dboManager.Delete<TfSetting, string>(nameof(TfSetting.Key), existingSetting.Key);
			if (!success)
				throw new TfDboServiceException("Delete<TfSetting> failed.");
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}



	#region <--- validation --->

	internal class TfSettingValidator : AbstractValidator<TfSetting>
	{
		public TfSettingValidator(
			ITfService tfService)
		{
		}

		public ValidationResult ValidateSave(
			TfSetting setting)
		{
			if (setting == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The setting is null.") });

			if (string.IsNullOrWhiteSpace(setting.Key))
				return new ValidationResult(new[] { new ValidationFailure("",
					"The setting name is required.") });

			return new ValidationResult { Errors = new List<ValidationFailure>() };
		}


		public ValidationResult ValidateDelete(
			TfSetting setting)
		{
			if (setting == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The bookmark with specified identifier is not found.") });

			return new ValidationResult { Errors = new List<ValidationFailure>() };
		}
	}

	#endregion
}
