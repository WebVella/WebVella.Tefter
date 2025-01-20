using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Wordprocessing;
using FluentResults;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Text;
using WebVella.Tefter.EmailSender.Models;
using WebVella.Tefter.EmailSender.Services;
using WebVella.Tefter.TemplateProcessors.Email.Components;
using WebVella.Tefter.TemplateProcessors.ExcelFile;
using WebVella.Tefter.TemplateProcessors.ExcelFile.Models;

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

			if(string.IsNullOrEmpty(item.HtmlContent) && string.IsNullOrEmpty(item.TextContent))
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

		var blobManager = serviceProvider.GetService<ITfBlobManager>();
		var emailService = serviceProvider.GetService<IEmailService>();
		
		foreach (var item in result.Items) 
		{
			var emailMessage = new CreateEmailMessageModel();
			if (!string.IsNullOrWhiteSpace( item.Sender) )
			{
				emailMessage.Sender = new EmailAddress(item.Sender);
			}
			
			emailMessage.Recipients = new List<EmailAddress>();
			foreach (var recipient in item.Recipients)
			{
				emailMessage.Recipients.Add(new EmailAddress(recipient));
			}

			if (item.CcRecipients != null && item.CcRecipients.Count > 0 )
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

				var bytes = blobManager.GetBlobByteArray(attachment.BlobId.Value,temporary: true).Value;
				var emailAttachment = new CreateEmailAttachmentModel
				{
					Filename = attachment.FileName,
					Buffer = bytes
				};
				emailMessage.Attachments.Add(emailAttachment);
			}

			emailService.CreateEmailMessage(emailMessage);
		}

		return result;
	}

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
				var htmlProcessResult = new TfHtmlTemplateProcessResult();
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

				htmlProcessResult = new TfHtmlTemplateProcessResult();
				htmlProcessResult.TemplateHtml = settings.HtmlContent ?? string.Empty;
				htmlProcessResult.ProcessHtmlTemplate(data);
				emailItem.HtmlContent = htmlProcessResult.ResultHtml;

				#region <--- process attachments --->

				emailItem.Attachments = new List<EmailTemplateResultItemAttachment>();

				foreach (var attItem in settings.AttachmentItems ?? new List<EmailTemplateSettingsAttachmentItem>())
				{
					var excelTemplateResult = templateService.GetTemplate(attItem.TemplateId);

					//we ignore missing template attachments
					if (!excelTemplateResult.IsSuccess || excelTemplateResult.Value == null)
					{
						//emailItem.Attachments.Add(new EmailTemplateResultItemAttachment
						//{
						//	BlobId = null,
						//	DownloadUrl = null,
						//	Errors = new List<ValidationError>()
						//	{
						//		new ValidationError("", "Excel template was not found.")
						//	},
						//	FileName = null
						//});
						continue;
					}

					if (((TfTemplate)excelTemplateResult.Value).ContentProcessorType != typeof(ExcelFileTemplateProcessor))
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
		ITfTemplateService _templateService)
	{
		var result = new List<TfTemplate>();
		var allTemplatesResult = _templateService.GetTemplates();
		if (allTemplatesResult.IsFailed) throw new Exception("GetTemplates failed");
		foreach (var item in allTemplatesResult.Value)
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
