using System.Text;
using WebVella.DocumentTemplates.Engines.Email;
using WebVella.DocumentTemplates.Engines.Email.Models;
using WebVella.Tefter.EmailSender.Models;
using WebVella.Tefter.EmailSender.Services;
using WebVella.Tefter.Exceptions;
using WebVella.Tefter.TemplateProcessors.Email.Components;
using WebVella.Tefter.TemplateProcessors.ExcelFile;
using WebVella.Tefter.TemplateProcessors.ExcelFile.Models;
using WebVella.Tefter.TemplateProcessors.TextFile;
using WebVella.Tefter.TemplateProcessors.TextFile.Models;

namespace WebVella.Tefter.TemplateProcessors.Email;

public class EmailTemplateProcessor : ITfTemplateProcessor
{
	public Guid Id => Constants.EMAIL_CONTENT_PROCESSOR_ID;
	public string Name => "Tefter email template";
	public string Description => "creates emails from a template and data";
	public string FluentIconName => "Mail";
	public TfTemplateResultType ResultType => TfTemplateResultType.Email;

	public void ValidatePreview(
		TfTemplate template,
		ITfTemplatePreviewResult preview,
		IServiceProvider serviceProvider)
	{
		var previewResult = (EmailTemplatePreviewResult)preview;
		foreach (var item in previewResult.Items)
		{
			item.Errors.Clear();

			if (!string.IsNullOrWhiteSpace(item.Sender) && !item.Sender.IsEmail())
			{
				item.Errors.Add(new ValidationError("Sender", "Sender is not a valid email address."));
			}

			if (item.Recipients == null || item.Recipients.Count == 0)
			{
				item.Errors.Add(new ValidationError("Recipients", "Recipients are not specified."));
			}
			else
			{
				if (item.Recipients.Any(x => !x.IsEmail()))
				{
					item.Errors.Add(new ValidationError("Recipients", "Invalid recipients."));
				}
			}

			if (item.CcRecipients != null && item.CcRecipients.Count > 0 && item.Recipients.Any(x => !x.IsEmail()))
			{
				item.Errors.Add(new ValidationError("CcRecipients", "Copy recipients contains invalid email address."));
			}

			if (item.BccRecipients != null && item.BccRecipients.Count > 0 && item.BccRecipients.Any(x => !x.IsEmail()))
			{
				item.Errors.Add(new ValidationError("BccRecipients", "Blind-copy recipients contains invalid email address."));
			}

			if (string.IsNullOrEmpty(item.HtmlContent) && string.IsNullOrEmpty(item.TextContent))
			{
				item.Errors.Add(new ValidationError("HtmlContent", "Email body content is empty."));
			}
		}
	}

	public ITfTemplatePreviewResult GenerateTemplatePreviewResult(
		TfTemplate template,
		TfDataTable dataTable,
		IServiceProvider serviceProvider)
	{
		var result = GenerateResultInternal(template, dataTable, serviceProvider);
		EmailTemplatePreviewResult previewResult = new EmailTemplatePreviewResult
		{
			Items = ((EmailTemplateResult)result).Items,
			Errors = result.Errors,
		};
		ValidatePreview(template, previewResult, serviceProvider);
		return previewResult;
	}

	public ITfTemplateResult ProcessTemplate(
		TfTemplate template,
		TfDataTable dataTable,
		ITfTemplatePreviewResult preview,
		IServiceProvider serviceProvider)
	{
		EmailTemplateResult result = new EmailTemplateResult
		{
			Items = ((EmailTemplatePreviewResult)preview).Items,
			Errors = preview.Errors,
		};

		var tfService = serviceProvider.GetService<ITfService>();
		var emailService = serviceProvider.GetService<IEmailService>();

		foreach (var item in result.Items)
		{
			var emailMessage = new CreateEmailMessageModel();
			if (!string.IsNullOrWhiteSpace(item.Sender))
			{
				emailMessage.Sender = new EmailAddress(item.Sender);
			}

			emailMessage.Recipients = new List<EmailAddress>();
			foreach (var recipient in item.Recipients)
			{
				emailMessage.Recipients.Add(new EmailAddress(recipient));
			}

			if (item.CcRecipients != null && item.CcRecipients.Count > 0)
			{
				emailMessage.RecipientsCc = new List<EmailAddress>();
				foreach (var recipient in item.CcRecipients)
				{
					emailMessage.RecipientsCc.Add(new EmailAddress(recipient));
				}
			}

			if (item.BccRecipients != null && item.BccRecipients.Count > 0)
			{
				emailMessage.RecipientsBcc = new List<EmailAddress>();
				foreach (var recipient in item.BccRecipients)
				{
					emailMessage.RecipientsBcc.Add(new EmailAddress(recipient));
				}
			}

			emailMessage.Subject = item.Subject;
			emailMessage.TextBody = item.TextContent;
			emailMessage.HtmlBody = item.HtmlContent;
			emailMessage.RelatedRowIds = item.RelatedRowIds;

			emailMessage.Attachments = new List<CreateEmailAttachmentModel>();
			foreach (var attachment in item.Attachments)
			{
				if (attachment.BlobId == null)
					continue;

				if (attachment.Errors != null && attachment.Errors.Count > 0)
					continue;

				var bytes = tfService.GetBlobByteArray(attachment.BlobId.Value, temporary: true);
				var emailAttachment = new CreateEmailAttachmentModel
				{
					Filename = attachment.FileName,
					Buffer = bytes
				};
				emailMessage.Attachments.Add(emailAttachment);
			}

			try
			{
				var resultEmail = emailService.CreateEmailMessage(emailMessage);
			}
			catch (Exception ex)
			{
				if (ex is TfValidationException)
				{
					var valEx = ex as TfValidationException;

					var data = valEx.GetDataAsUsableDictionary();
					foreach (var propertyName in data.Keys)
					{
						var errors = data[propertyName];
						foreach (var errorMessage in errors)
						{
							if (String.IsNullOrWhiteSpace(propertyName))
								result.Errors.Add(new ValidationError("", errorMessage));
							else
								result.Errors.Add(new ValidationError(propertyName, errorMessage));
						}
					}

					if (data.Keys.Count == 0 && !string.IsNullOrWhiteSpace(valEx.Message))
						result.Errors.Add(new ValidationError("", ex.Message));
				}
				else
					result.Errors.Add(new ValidationError("", ex.Message));

			}
		}

		return result;
	}
	/*
	private ITfTemplateResult GenerateResultInternal(
		TfTemplate template,
		TfDataTable dataTable,
		IServiceProvider serviceProvider)
	{
		var result = new EmailTemplateResult();

		var templateService = serviceProvider.GetService<ITfTemplateService>();
		var dataManager = serviceProvider.GetService<ITfDataManager>();

		if (string.IsNullOrWhiteSpace(template.SettingsJson))
		{
			result.Errors.Add(new ValidationError("", "Template settings are not set."));
			return result;
		}

		var settings = JsonSerializer.Deserialize<EmailTemplateSettings>(template.SettingsJson);

		var groupedData = GroupDataTable(settings.GroupBy, dataTable);

		foreach (var key in groupedData.Keys)
		{
			var data = groupedData[key];

			var emailItem = new EmailTemplateResultItem();

			try
			{
				foreach (TfDataRow row in data.Rows)
				{
					emailItem.RelatedRowIds.Add((Guid)row["tf_id"]);
				}

				var textProcessResult = new TfTextTemplateProcessResult();
				textProcessResult.TemplateText = settings.Sender ?? string.Empty;
				textProcessResult.ProcessTextTemplate(data);
				emailItem.Sender = textProcessResult.ResultText;

				textProcessResult = new TfTextTemplateProcessResult();
				textProcessResult.TemplateText = settings.Recipients ?? string.Empty;
				textProcessResult.ProcessTextTemplate(data);
				emailItem.Recipients = textProcessResult.ResultText.Split(";", StringSplitOptions.RemoveEmptyEntries).ToList();

				textProcessResult = new TfTextTemplateProcessResult();
				textProcessResult.TemplateText = settings.CcRecipients ?? string.Empty;
				textProcessResult.ProcessTextTemplate(data);
				emailItem.CcRecipients = textProcessResult.ResultText.Split(";", StringSplitOptions.RemoveEmptyEntries).ToList();

				textProcessResult = new TfTextTemplateProcessResult();
				textProcessResult.TemplateText = settings.BccRecipients ?? string.Empty;
				textProcessResult.ProcessTextTemplate(data);
				emailItem.BccRecipients = textProcessResult.ResultText.Split(";", StringSplitOptions.RemoveEmptyEntries).ToList();

				textProcessResult = new TfTextTemplateProcessResult();
				textProcessResult.TemplateText = settings.Subject ?? string.Empty;
				textProcessResult.ProcessTextTemplate(data);
				emailItem.Subject = textProcessResult.ResultText;

				textProcessResult = new TfTextTemplateProcessResult();
				textProcessResult.TemplateText = settings.TextContent ?? string.Empty;
				textProcessResult.ProcessTextTemplate(data);
				emailItem.TextContent = textProcessResult.ResultText;

				var htmlProcessResult = new TfHtmlTemplateProcessResult();
				htmlProcessResult.TemplateHtml = settings.HtmlContent ?? string.Empty;
				htmlProcessResult.ProcessHtmlTemplate(data);
				emailItem.HtmlContent = htmlProcessResult.ResultHtml;

				#region <--- process attachments --->

				emailItem.Attachments = new List<EmailTemplateResultItemAttachment>();

				foreach (var attItem in settings.AttachmentItems ?? new List<EmailTemplateSettingsAttachmentItem>())
				{
					var excelTemplate = templateService.GetTemplate(attItem.TemplateId);

					//we ignore missing template attachments
					if (excelTemplate == null)
						continue;

					if (((TfTemplate)excelTemplate).ContentProcessorType != typeof(ExcelFileTemplateProcessor))
					{
						emailItem.Attachments.Add(new EmailTemplateResultItemAttachment
						{
							BlobId = null,
							DownloadUrl = null,
							Errors = new List<ValidationError>()
							{
								new ValidationError("", "Template is not an excel file template.")
							},
							FileName = null
						});
						continue;
					}

					var processedExcelResult = (ExcelFileTemplateResult)
						templateService.ProcessTemplate(attItem.TemplateId, groupedData[key], null);

					if (processedExcelResult.Errors.Count > 0)
					{
						var emailAttachemntItem = new EmailTemplateResultItemAttachment
						{
							BlobId = null,
							DownloadUrl = null,
							Errors = new List<ValidationError>(),
							FileName = null
						};
						emailAttachemntItem.Errors.AddRange(processedExcelResult.Errors);
						emailItem.Attachments.Add(emailAttachemntItem);
						continue;
					}

					emailItem.Attachments.Add(new EmailTemplateResultItemAttachment
					{
						BlobId = processedExcelResult.ZipBlobId,
						DownloadUrl = processedExcelResult.ZipDownloadUrl,
						Errors = new(),
						FileName = processedExcelResult.ZipFilename
					});
				}

				#endregion

				result.Items.Add(emailItem);
			}
			catch (Exception ex)
			{
				emailItem.Errors.Add(new ValidationError("", $"Unexpected error occurred. {ex.Message} {ex.StackTrace}"));
			}
		}

		return result;
	}
	*/
	private ITfTemplateResult GenerateResultInternal(
		TfTemplate template,
		TfDataTable dataTable,
		IServiceProvider serviceProvider)
	{
		var result = new EmailTemplateResult();

		var _tfService = serviceProvider.GetService<ITfService>();

		if (string.IsNullOrWhiteSpace(template.SettingsJson))
		{
			result.Errors.Add(new ValidationError("", "Template settings are not set."));
			return result;
		}

		var settings = JsonSerializer.Deserialize<EmailTemplateSettings>(template.SettingsJson);

		WvEmail emailTmpl = new WvEmail
		{
			Sender = settings.Sender ?? string.Empty,
			Recipients = settings.Recipients ?? string.Empty,
			CcRecipients = settings.CcRecipients ?? string.Empty,
			BccRecipients = settings.BccRecipients ?? string.Empty,
			Subject = settings.Subject ?? string.Empty,
			TextContent = settings.TextContent ?? string.Empty,
			HtmlContent = settings.HtmlContent ?? string.Empty
		};

		emailTmpl.AttachmentItems = new List<WvEmailAttachment>();
		foreach (var attItem in settings.AttachmentItems ?? new List<EmailTemplateSettingsAttachmentItem>())
		{
			var emailAtt = new WvEmailAttachment();

			var attTemplate = _tfService.GetTemplate(attItem.TemplateId);

			//we ignore missing template attachments
			if (attTemplate == null)
				continue;

			if (((TfTemplate)attTemplate).ContentProcessorType == typeof(ExcelFileTemplateProcessor))
			{
				var attTemplaceSettings = JsonSerializer.Deserialize<ExcelFileTemplateSettings>(attTemplate.SettingsJson);
				var bytes = _tfService.GetBlobByteArray(attTemplaceSettings.TemplateFileBlobId.Value);

				emailAtt.Type = WvEmailAttachmentType.SpreadsheetFile;
				emailAtt.Template = new MemoryStream(bytes);
				emailAtt.Filename = attTemplaceSettings.FileName;
				emailAtt.GroupDataByColumns = attTemplaceSettings.GroupBy;
				emailTmpl.AttachmentItems.Add(emailAtt);
			}
			else if (((TfTemplate)attTemplate).ContentProcessorType == typeof(TextFileTemplateProcessor))
			{
				var attTemplaceSettings = JsonSerializer.Deserialize<TextFileTemplateSettings>(attTemplate.SettingsJson);
				var bytes = _tfService.GetBlobByteArray(attTemplaceSettings.TemplateFileBlobId.Value);

				emailAtt.Type = WvEmailAttachmentType.TextFile;
				emailAtt.Template = new MemoryStream(bytes);
				emailAtt.Filename = attTemplaceSettings.FileName;
				emailAtt.GroupDataByColumns = attTemplaceSettings.GroupBy;
				emailTmpl.AttachmentItems.Add(emailAtt);
			}
			else
			{
				//ignore other type templates
			}
		}

		WvEmailTemplate tmpl = new WvEmailTemplate
		{
			Template = emailTmpl,
			GroupDataByColumns = settings.GroupBy
		};


		var tmplResult = tmpl.Process(dataTable.ToDataTable());


		foreach (var resultItem in tmplResult.ResultItems)
		{
			if (resultItem is null)
				continue;

			var emailItem = new EmailTemplateResultItem();

			
			try
			{
				foreach( var ctx in resultItem.Contexts )
				{
					foreach(var errorMessage in ctx.Errors )
						emailItem.Errors.Add( new ValidationError("", errorMessage) );
				}

				foreach (DataRow row in resultItem.DataTable.Rows)
					emailItem.RelatedRowIds.Add((Guid)row["tf_id"]);

				emailItem.Sender = resultItem.Result.Sender ?? string.Empty;
				emailItem.Recipients = (resultItem.Result.Recipients ?? string.Empty).Split(";", StringSplitOptions.RemoveEmptyEntries).ToList();
				emailItem.CcRecipients = (resultItem.Result.CcRecipients ?? string.Empty).Split(";", StringSplitOptions.RemoveEmptyEntries).ToList();
				emailItem.BccRecipients = (resultItem.Result.BccRecipients ?? string.Empty).Split(";", StringSplitOptions.RemoveEmptyEntries).ToList();
				emailItem.Subject = resultItem.Result.Subject ?? string.Empty;
				emailItem.TextContent = resultItem.Result.TextContent ?? string.Empty;
				emailItem.HtmlContent = resultItem.Result.HtmlContent ?? string.Empty;

				emailItem.Attachments = new List<EmailTemplateResultItemAttachment>();

				foreach (var attachmentItem in resultItem.Result.AttachmentItems)
				{
					if (attachmentItem is null)
						continue;

					var emailAtt = new EmailTemplateResultItemAttachment();

					foreach (var ctx in attachmentItem.Contexts)
					{
						foreach (var errorMessage in ctx.Errors)
							emailAtt.Errors.Add(new ValidationError("", errorMessage));
					}

					emailAtt.FileName = attachmentItem.Filename;
					emailAtt.DownloadUrl = null;
					if (attachmentItem.Template is not null)
					{
						emailAtt.BlobId = _tfService.CreateBlob(attachmentItem.Template.GetBuffer(), temporary: true);
						emailAtt.DownloadUrl = $"/fs/blob/{emailAtt.BlobId}/{emailAtt.FileName}";
					}
					emailItem.Attachments.Add(emailAtt);
				}

				result.Items.Add(emailItem);
			}
			catch (Exception ex)
			{
				emailItem.Errors.Add(new ValidationError("", $"Unexpected error occurred. {ex.Message} {ex.StackTrace}"));
			}
		}

		return result;
	}

	private Dictionary<string, TfDataTable> GroupDataTable(
		List<string> groupColumns,
		TfDataTable dataTable)
	{
		var result = new Dictionary<string, TfDataTable>();
		if (groupColumns is null || groupColumns.Count == 0)
		{
			result.Add(string.Empty, dataTable);
		}
		else
		{
			foreach (TfDataRow row in dataTable.Rows)
			{
				var sbKey = new StringBuilder();

				foreach (var column in groupColumns)
				{
					sbKey.Append($"{row[column]}$$$|||$$$");
				}

				var key = sbKey.ToString();

				if (!result.ContainsKey(key))
				{
					result.Add(key, dataTable.NewTable());
				}

				result[key].Rows.Add(row);
			}
		}

		return result;
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
		ITfService _tfService)
	{
		var result = new List<TfTemplate>();
		var allTemplates = _tfService.GetTemplates();
		foreach (var item in allTemplates)
		{
			if (item.ResultType != TfTemplateResultType.File) continue;
			if (!item.IsSelectable) continue;
			if (item.Id == templateId) continue;
			result.Add(item);
		}
		result = result.OrderBy(x => x.Name).ToList();
		return result;
	}
}
