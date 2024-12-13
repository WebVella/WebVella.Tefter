using WebVella.Tefter.Templates.Models;

namespace WebVella.Tefter.Templates.ContentProcessors;

public class TfEmailContentProcessor : ITemplateProcessor
{
	public string Name => "Default tefter email content processor";

	public string Description => "Default tefter email content processor";

	public TemplateResultType ResultType => TemplateResultType.Email;

	public Type SettingsComponentType => null;

	public Type ResultViewComponentType => null;

	public ITemplateResult GenerateContent(
		Template template,
		TfDataTable data)
	{
		return null;
	}

	public List<string> GetUsedColumns(
		string settingsJson,
		ITemplatesService templateService )
	{
		var settings = JsonSerializer.Deserialize<TemplateEmailSettings>(settingsJson);

		List<string> usedColumns = new List<string>();

		if (!string.IsNullOrWhiteSpace(settings.GroupBy))
			usedColumns.Add(settings.GroupBy);

		var senderTags = TfTemplateUtility.GetTagsFromTemplate(settings.Sender ?? string.Empty);
		var recipientsTags = TfTemplateUtility.GetTagsFromTemplate(settings.Recipients ?? string.Empty);
		var ccRecipientsTags = TfTemplateUtility.GetTagsFromTemplate(settings.CcRecipients ?? string.Empty);
		var bccRecipientsTags = TfTemplateUtility.GetTagsFromTemplate(settings.BccRecipients ?? string.Empty);
		var subjectTags = TfTemplateUtility.GetTagsFromTemplate(settings.Subject ?? string.Empty);
		var textContentTags = TfTemplateUtility.GetTagsFromTemplate(settings.TextContent ?? string.Empty);
		var htmlContentTags = TfTemplateUtility.GetTagsFromTemplate(settings.HtmlContent ?? string.Empty);

		List<TfTemplateTag> tags = new List<TfTemplateTag>();
		tags.AddRange(senderTags);
		tags.AddRange(recipientsTags);
		tags.AddRange(ccRecipientsTags);
		tags.AddRange(bccRecipientsTags);
		tags.AddRange(subjectTags);
		tags.AddRange(textContentTags);
		tags.AddRange(htmlContentTags);

		foreach (var tag in tags)
		{
			if (tag.Type == TfTemplateTagType.Data)
			{
				if (!usedColumns.Contains(tag.Name))
					usedColumns.Add(tag.Name);
			}
		}

		var attachmentColumns = GetUsedColumnsFromAttachments(settings, templateService);
		foreach( var column in  attachmentColumns)
		{
			if (!usedColumns.Contains(column))
				usedColumns.Add(column);
		}

		return usedColumns;
	}

	public List<Template> GetUsedTemplates(
		string settingsJson,
		ITemplatesService templateService)
	{
		List<Template> usedTemplates = new List<Template>();

		var settings = JsonSerializer.Deserialize<TemplateEmailSettings>(settingsJson);

		if (settings.AttachmentItems == null || settings.AttachmentItems.Count == 0)
		{
			return usedTemplates;
		}

		foreach (var item in settings.AttachmentItems)
		{

			if (!usedTemplates.Any(x=>x.Id == item.TemplateId))
			{
				var templateResult = templateService.GetTemplate(item.TemplateId);

				if (!templateResult.IsSuccess || templateResult.Value == null)
					continue;

				usedTemplates.Add(templateResult.Value);
			}
		}

		return usedTemplates;
	}

	private List<string> GetUsedColumnsFromAttachments(
		TemplateEmailSettings settings,
		ITemplatesService templateService )
	{
		var usedColumns = new List<string>();

		if (settings == null ||
			settings.AttachmentItems == null ||
			settings.AttachmentItems.Count == 0)
		{
			return usedColumns;
		}

		Queue<QueueItem> queue = new Queue<QueueItem>();
		
		foreach (var item in settings.AttachmentItems)
		{
			var templateResult = templateService.GetTemplate(item.TemplateId);

			if (!templateResult.IsSuccess || templateResult.Value == null)
				continue;

			queue.Enqueue(new QueueItem
			{
				Template = templateResult.Value,
				Depth = 0
			});
		}

		while(queue.Count > 0)
		{
			var queueItem = queue.Dequeue();

			if (queueItem.Depth > TfTemplatesConstants.MAX_CALCULATION_DEPTH)
			{
				throw new Exception("Calculation of used templates failed, " +
					"because it goes to deep recursive.");
			}

			var processorResult = templateService.GetProcessor(queueItem.Template.ContentProcessorType);

			if (!processorResult.IsSuccess || processorResult.Value == null)
				continue;

			var processor = processorResult.Value;	

			var childTemplates = processor.GetUsedTemplates(
				queueItem.Template.SettingsJson, 
				templateService);

			foreach(var childTemplate in childTemplates)
			{
				var templateResult = templateService.GetTemplate(childTemplate.Id);

				if (!templateResult.IsSuccess || templateResult.Value == null)
					continue;

				queue.Enqueue(new QueueItem
				{
					Template = childTemplate,
					Depth = (short)(queueItem.Depth + 1)
				});
			}
		}

		return usedColumns;
	}

	class QueueItem
	{
		public Template Template { get; set; }
		public short Depth { get; set; }
	}

	public List<ValidationError> ValidateSettings(
		string settingsJson,
		ITemplatesService templateService)
	{
		throw new NotImplementedException();
	}

	public string OnCreateTemplate(
		string settingsJson,
		ITemplatesService templateService)
	{
		return settingsJson;
	}

	public string OnUpdateTemplate(
		Guid templateId,
		string newSettingsJson,
		ITemplatesService templateService)
	{
		//do nothing
		return newSettingsJson;

	}

	public void OnDeleteTemplate(
		Guid templateId,
		ITemplatesService templateService)
	{
		//do nothing
	}
}
