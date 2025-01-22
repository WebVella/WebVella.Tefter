namespace WebVella.Tefter.Services;

public partial interface ITfTemplateService
{
	public ITfTemplatePreviewResult GenerateTemplatePreviewResult(
			Guid templateId,
			Guid spaceDataId,
			List<Guid> tfRecordIds);

	public ITfTemplateResult ProcessTemplate(
		Guid templateId,
		Guid spaceDataId,
		List<Guid> tfRecordIds,
		ITfTemplatePreviewResult preview);

	public ITfTemplateResult ProcessTemplate(
		Guid templateId,
		TfDataTable dataTable,
		ITfTemplatePreviewResult preview);

	public void ValidatePreview(
		Guid templateId,
		ITfTemplatePreviewResult preview);
}

internal partial class TfTemplateService : ITfTemplateService
{
	public ITfTemplatePreviewResult GenerateTemplatePreviewResult(
		Guid templateId,
		Guid spaceDataId,
		List<Guid> tfRecordIds)
	{
		var template = GetTemplate(templateId).Value;
		var spaceData = _spaceManager.GetSpaceData(spaceDataId).Value;
		var processor = GetTemplateProcessor(template.ContentProcessorType).Value;

		var dataTable = _dataManager.QuerySpaceData(spaceData.Id, tfRecordIds).Value;

		return processor.GenerateTemplatePreviewResult(template, dataTable, _serviceProvider);
	}

	public ITfTemplateResult ProcessTemplate(
		Guid templateId,
		Guid spaceDataId,
		List<Guid> tfRecordIds,
		ITfTemplatePreviewResult preview)
	{
		var template = GetTemplate(templateId).Value;

		if (template is null)
		{
			throw new Exception("Template is not found.");
		}

		if (!template.SpaceDataList.Contains(spaceDataId))
		{
			throw new Exception("Template does not work for selected space data.");
		}

		var spaceData = _spaceManager.GetSpaceData(spaceDataId).Value;
		var processor = GetTemplateProcessor(template.ContentProcessorType).Value;
		var dataTable = _dataManager.QuerySpaceData(spaceData.Id, tfRecordIds).Value;

		
		return processor.ProcessTemplate(template, dataTable, preview, _serviceProvider);
	}

	public ITfTemplateResult ProcessTemplate(
		Guid templateId,
		TfDataTable dataTable,
		ITfTemplatePreviewResult preview)
	{
		var template = GetTemplate(templateId).Value;

		if (template is null)
		{
			throw new Exception("Template is not found.");
		}

		var processor = GetTemplateProcessor(template.ContentProcessorType).Value;
		return processor.ProcessTemplate(template, dataTable, preview, _serviceProvider);
	}

	public void ValidatePreview(
		Guid templateId,
		ITfTemplatePreviewResult preview)
	{
		var template = GetTemplate(templateId).Value;
		var processor = GetTemplateProcessor(template.ContentProcessorType).Value;
		processor.ValidatePreview(template, preview, _serviceProvider);
	}
}
