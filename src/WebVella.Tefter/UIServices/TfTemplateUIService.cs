namespace WebVella.Tefter.UIServices;

public partial interface ITfTemplateUIService
{
	//Events
	event EventHandler<TfTemplate> TemplateCreated;
	event EventHandler<TfTemplate> TemplateUpdated;
	event EventHandler<TfTemplate> TemplateDeleted;

	//Templates
	List<TfTemplate> GetTemplates(string? search = null, TfTemplateResultType? type = null);
	List<TfTemplate> GetSpaceDataTemplates(Guid spaceDataId, string? search = null);
	TfTemplate GetTemplate(Guid roleId);
	TfTemplate CreateTemplate(TfManageTemplateModel item);
	TfTemplate UpdateTemplate(TfManageTemplateModel item);
	void DeleteTemplate(Guid templateId);

	//Processors
	ReadOnlyCollection<ITfTemplateProcessorAddon> GetProcessors();
	//Data options
	List<TfDataSetAsOption> GetSpaceDataOptionsForTemplate();

	//Settings
	TfTemplate UpdateTemplateSettings(Guid templateId, string settingsJson);
}
public partial class TfTemplateUIService : ITfTemplateUIService
{
	#region << Ctor >>
	private static readonly AsyncLock _asyncLock = new AsyncLock();
	private readonly ITfService _tfService;
	private readonly ITfMetaService _metaService;
	private readonly IStringLocalizer<TfSpaceUIService> LOC;

	public TfTemplateUIService(IServiceProvider serviceProvider)
	{
		_tfService = serviceProvider.GetService<ITfService>() ?? default!;
		_metaService = serviceProvider.GetService<ITfMetaService>() ?? default!;
		LOC = serviceProvider.GetService<IStringLocalizer<TfSpaceUIService>>() ?? default!;
	}
	#endregion

	#region << Events >>
	public event EventHandler<TfTemplate> TemplateUpdated = default!;
	public event EventHandler<TfTemplate> TemplateCreated = default!;
	public event EventHandler<TfTemplate> TemplateDeleted = default!;
	#endregion

	#region << Template >>
	public List<TfTemplate> GetTemplates(string? search = null, TfTemplateResultType? type = null) => _tfService.GetTemplates(search, type);

	public List<TfTemplate> GetSpaceDataTemplates(Guid spaceDataId, string? search = null)
	{
		var templates = _tfService.GetTemplates();
		var result = new List<TfTemplate>();
		foreach (var item in templates)
		{
			if (!item.IsEnabled)
				continue;

			if (!item.IsSelectable)
				continue;

			if (!item.SpaceDataList.Contains(spaceDataId))
				continue;

			if (!TemplateMatchSearch(item, search, null))
				continue;

			result.Add(item);
		}

		return result.OrderBy(x => x.Name).ToList();
	}
	public TfTemplate GetTemplate(Guid id) => _tfService.GetTemplate(id);
	public TfTemplate CreateTemplate(TfManageTemplateModel item)
	{
		var template = _tfService.CreateTemplate(item);
		TemplateCreated?.Invoke(this, template);
		return template;
	}
	public TfTemplate UpdateTemplate(TfManageTemplateModel item)
	{
		var template = _tfService.UpdateTemplate(item);
		TemplateUpdated?.Invoke(this, template);
		return template;
	}
	public void DeleteTemplate(Guid templateId)
	{
		var template = _tfService.GetTemplate(templateId);
		_tfService.DeleteTemplate(templateId);
		TemplateDeleted?.Invoke(this, template);
	}
	#endregion

	#region << Processors >>
	public ReadOnlyCollection<ITfTemplateProcessorAddon> GetProcessors()
		=> _tfService.GetTemplateProcessors();
	#endregion

	#region << Space Data >>
	public List<TfDataSetAsOption> GetSpaceDataOptionsForTemplate()
		=> _tfService.GetSpaceDataOptionsForTemplate();
	#endregion

	#region << Settings >>
	public TfTemplate UpdateTemplateSettings(
		Guid templateId,
		string settingsJson)
	{
		var template = _tfService.UpdateTemplateSettings(
			templateId: templateId,
			settingsJson: settingsJson
		);
		TemplateUpdated?.Invoke(this, template);
		return template;
	}
	#endregion

	private static bool TemplateMatchSearch(
		TfTemplate template,
		string search = null,
		TfTemplateResultType? resultType = null)
	{
		if (resultType is not null && template.ResultType != resultType.Value)
			return false;

		var stringProcessed = search?.Trim().ToLowerInvariant();
		if (String.IsNullOrWhiteSpace(stringProcessed)) return true;
		else if (template.Name.ToLowerInvariant().Contains(stringProcessed))
		{
			return true;
		}
		else if ((template.Description ?? string.Empty).ToLowerInvariant().Contains(stringProcessed))
		{
			return true;
		}

		return false;
	}
}
