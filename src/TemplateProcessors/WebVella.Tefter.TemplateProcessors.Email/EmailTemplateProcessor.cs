using WebVella.Tefter.TemplateProcessors.Email.Components;

namespace WebVella.Tefter.TemplateProcessors.Email;

public class EmailTemplateProcessor : ITfTemplateProcessor
{
	public Guid Id => Constants.EMAIL_CONTENT_PROCESSOR_ID;
	public string Name => "Tefter email template";
	public string Description => "creates emails from a template and data";
	public string FluentIconName => "Mail";

	public TfTemplateResultType ResultType => TfTemplateResultType.Email;
	public Type SettingsComponentType => typeof(SettingsComponent);
	public Type ResultPreviewComponentType => typeof(ResultPreviewComponent);
	public Type ResultComponentType => typeof(ResultComponent);
	public Type HelpComponentType => typeof(HelpComponent);
	public ITfTemplateResult GenerateTemplateResult(
		TfTemplate template,
		TfDataTable data,
		IServiceProvider serviceProvider)
	{
		//TODO 
		return null;
	}

	public List<ValidationError> ProcessTemplateResult(
		TfTemplate template,
		TfDataTable data,
		IServiceProvider serviceProvider)
	{
		//TODO
		return new List<ValidationError>();
	}

	public List<ValidationError> ValidateSettings(
		string settingsJson,
		IServiceProvider serviceProvider)
	{
		var result = new List<ValidationError>();

		if (string.IsNullOrWhiteSpace(settingsJson))
		{
			return result;
		}

		var settings = JsonSerializer.Deserialize<EmailTemplateSettings>(settingsJson);

		if (string.IsNullOrWhiteSpace(settings.Recipients))
		{
			result.Add(new ValidationError(nameof(settings.Recipients), "Recipient(s) is/are required."));
		}

		return result;
	}

	public List<ValidationError> OnCreate(
		TfManageTemplateModel template,
		IServiceProvider serviceProvider)
	{
		return new List<ValidationError>();
	}

	public void OnCreated(
		TfTemplate template,
		IServiceProvider serviceProvider)
	{
	}

	public List<ValidationError> OnUpdate(
		TfManageTemplateModel template,
		IServiceProvider serviceProvider)
	{
		return new List<ValidationError>();
	}

	public void OnUpdated(
		TfTemplate template,
		IServiceProvider serviceProvider)
	{
	}

	public List<ValidationError> OnDelete(
		TfTemplate template,
		IServiceProvider serviceProvider)
	{
		return new List<ValidationError>();
	}

	public void OnDeleted(
		TfTemplate template,
		IServiceProvider serviceProvider)
	{
	}

	public List<TfTemplate> GetTemplateSelectionList(
		Guid? templateId,
		ITfTemplateService _templateService)
	{
		var result = new List<TfTemplate>();
		var allTemplatesResult = _templateService.GetTemplates();
		if(allTemplatesResult.IsFailed) throw new Exception("GetTemplates failed");
		foreach (var item in allTemplatesResult.Value)
		{
			if(item.ResultType != TfTemplateResultType.File) continue;
			if(!item.IsSelectable) continue;
			if(item.Id == templateId) continue;
			result.Add(item);
		}
		result = result.OrderBy(x=> x.Name).ToList();
		return result;
	}
}
