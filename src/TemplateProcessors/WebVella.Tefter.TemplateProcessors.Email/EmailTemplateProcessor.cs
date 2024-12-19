namespace WebVella.Tefter.TemplateProcessors.Email;

public class EmailTemplateProcessor : ITfTemplateProcessor
{
	public Guid Id => Constants.EMAIL_CONTENT_PROCESSOR_ID;
	public string Name => "Tefter email template";
	public string Description => "creates emails from a template and data";
	public string FluentIconName => "Mail";

	public TfTemplateResultType ResultType => TfTemplateResultType.Email;

	public Type SettingsComponentType => null;

	public Type ResultViewComponentType => null;

	public List<string> GetUsedColumns(
		string settingsJson,
		IServiceProvider serviceProvider)
	{
		if (string.IsNullOrWhiteSpace(settingsJson))
		{
			return new List<string>();
		}

		var settings = JsonSerializer.Deserialize<EmailTemplateSettings>(settingsJson);

		List<string> usedColumns = new List<string>();

		if (settings.GroupBy != null && settings.GroupBy.Count > 0 )
		{
			usedColumns.AddRange(settings.GroupBy);
		}

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

		var attachmentColumns = GetUsedColumnsFromAttachments(settings, serviceProvider);
		foreach (var column in attachmentColumns)
		{
			if (!usedColumns.Contains(column))
				usedColumns.Add(column);
		}

		return usedColumns;
	}

	public List<TfTemplate> GetUsedTemplates(
		string settingsJson,
		IServiceProvider serviceProvider)
	{
		
		List<TfTemplate> usedTemplates = new List<TfTemplate>();

		if (string.IsNullOrEmpty(settingsJson))
		{
			return usedTemplates;
		}

		var templateService = serviceProvider.GetService<ITfTemplateService>();

		var settings = JsonSerializer.Deserialize<EmailTemplateSettings>(settingsJson);

		if (settings.AttachmentItems == null || settings.AttachmentItems.Count == 0)
		{
			return usedTemplates;
		}

		foreach (var item in settings.AttachmentItems)
		{

			if (!usedTemplates.Any(x => x.Id == item.TemplateId))
			{
				var templateResult = templateService.GetTemplate(item.TemplateId);

				if (!templateResult.IsSuccess || templateResult.Value == null)
					continue;

				usedTemplates.Add(templateResult.Value);
			}
		}

		return usedTemplates;
	}

	public ITfTemplateResult GenerateTemplateResult(
		TfTemplate template,
		TfDataTable data,
		IServiceProvider serviceProvider)
	{
		//TODO 
		return null;
	}

	public void ProcessTemplateResult(
		TfTemplate template,
		TfDataTable data,
		IServiceProvider serviceProvider)
	{
		//TODO
	}

	public List<ValidationError> ValidateSettings(
		string settingsJson,
		IServiceProvider serviceProvider)
	{
		//TODO
		return new List<ValidationError>();
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
		

	private List<string> GetUsedColumnsFromAttachments(
		EmailTemplateSettings settings,
		IServiceProvider serviceProvider)
	{
		var usedColumns = new List<string>();

		if (settings == null ||
			settings.AttachmentItems == null ||
			settings.AttachmentItems.Count == 0)
		{
			return usedColumns;
		}

		var templateService = serviceProvider.GetService<ITfTemplateService>();

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

		while (queue.Count > 0)
		{
			var queueItem = queue.Dequeue();

			if (queueItem.Depth > Constants.MAX_CALCULATION_DEPTH)
			{
				throw new Exception("Calculation of used templates failed, " +
					"because it goes to deep recursive.");
			}

			var processorResult = templateService.GetTemplateProcessor(queueItem.Template.ContentProcessorType);

			if (!processorResult.IsSuccess || processorResult.Value == null)
				continue;

			var processor = processorResult.Value;

			var childTemplates = processor.GetUsedTemplates(
				queueItem.Template.SettingsJson,
				serviceProvider);

			foreach (var childTemplate in childTemplates)
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
		public TfTemplate Template { get; set; }
		public short Depth { get; set; }
	}
}
