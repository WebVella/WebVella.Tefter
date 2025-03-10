using HtmlAgilityPack;
using MimeKit.Utils;
using System.Text;

namespace WebVella.Tefter.EmailSender.Services;

public partial interface IEmailService
{
	public List<EmailMessage> GetEmailMessages(
		string search = null,
		int? page = null,
		int? pageSize = null);

	public List<EmailMessage> GetEmailMessages(
		Guid tfRowId,
		int? page = null,
		int? pageSize = null);

	public EmailMessage CreateEmailMessage(
		CreateEmailMessageModel emailMessage);

	public EmailMessage GetEmailMessageById(
		Guid id);

	internal List<EmailMessage> GetPendingEmails();

	internal void EmbedRepositoryImages(
	   BodyBuilder builder);

	internal NpgsqlParameter CreateParameter(
		string name,
		object value,
		DbType type);
}

internal partial class EmailService : IEmailService
{
	private readonly ITfDatabaseService _dbService;
	private readonly ITfService _tfService;
	private readonly ISmtpConfigurationService _config;

	public EmailService(
		ITfDatabaseService dbService,
		ITfService tfService,
		ISmtpConfigurationService config)
	{
		_dbService = dbService;
		_tfService = tfService;
		_config = config;
	}

	public EmailMessage GetEmailMessageById(
		Guid id)
	{
		const string SQL = "SELECT * FROM email_message WHERE id = @id";

		var assetIdPar = CreateParameter(
			"id",
			id,
			DbType.Guid);

		var dt = _dbService.ExecuteSqlQueryCommand(SQL, assetIdPar);

		List<EmailMessage> emailMessages = CreateModelListFromDataTable(dt);

		if (emailMessages.Count == 0)
		{
			return null;
		}

		return emailMessages[0];
	}

	public List<EmailMessage> GetEmailMessages(
		string search = null,
		int? page = null,
		int? pageSize = null
		)
	{
		string pagingSql = string.Empty;

		if (page.HasValue && pageSize.HasValue)
		{
			int offset = (page.Value - 1) * pageSize.Value;
			int limit = pageSize.Value;
			pagingSql = $"OFFSET {offset} LIMIT {limit}";
		}

		string SQL = $@"
SELECT * FROM email_message
WHERE ( @search IS NULL OR x_search ILIKE CONCAT ('%', @search, '%') )
ORDER BY created_on DESC {pagingSql}";

		var searchPar = CreateParameter(
			"search",
			search,
			DbType.String);

		var dt = _dbService.ExecuteSqlQueryCommand(SQL, searchPar);

		List<EmailMessage> emailMessages = CreateModelListFromDataTable(dt);

		return emailMessages;
	}

	public List<EmailMessage> GetEmailMessages(
		Guid tfRowId,
		int? page = null,
		int? pageSize = null
		)
	{
		string pagingSql = string.Empty;

		if (page.HasValue && pageSize.HasValue)
		{
			int offset = (page.Value - 1) * pageSize.Value;
			int limit = pageSize.Value;
			pagingSql = $"OFFSET {offset} LIMIT {limit}";
		}

		string SQL = $@"
SELECT * FROM email_message
WHERE ( related_row_ids ILIKE CONCAT ('%', @id, '%') )
ORDER BY created_on DESC {pagingSql}";

		var idParam = CreateParameter(
			"id",
			tfRowId.ToString(),
			DbType.String);

		var dt = _dbService.ExecuteSqlQueryCommand(SQL, idParam);

		List<EmailMessage> emailMessages = CreateModelListFromDataTable(dt);

		return emailMessages;
	}

	public EmailMessage CreateEmailMessage(
		CreateEmailMessageModel emailMessage)
	{


		new EmailMessageValidator(
				_dbService,
				_tfService,
				_config
		)
		.ValidateCreate(emailMessage)
		.ToValidationException()
		.ThrowIfContainsErrors();

		var validAttachments = new List<EmailAttachment>();

		if (emailMessage.Attachments is not null)
		{
			foreach (var att in emailMessage.Attachments)
			{
				var blobId = _tfService.CreateBlob(att.Buffer);

				EmailAttachment attachment = new EmailAttachment
				{
					Filename = att.Filename,
					BlobId = blobId
				};

				validAttachments.Add(attachment);
			}
		}

		var id = Guid.NewGuid();

		EmailAddress sender = emailMessage.Sender;
		if (sender is null)
		{
			sender = new EmailAddress
			{
				Address = _config.DefaultSenderEmail,
				Name = _config.DefaultSenderName
			};
		}

		string replyToEmail = _config.DefaultReplyToEmail;
		if (!string.IsNullOrWhiteSpace(emailMessage.ReplyTo))
		{
			replyToEmail = emailMessage.ReplyTo;
		}


		string htmlContent = null;
		string textContent = null;

		if (string.IsNullOrEmpty(emailMessage.HtmlBody) &&
			string.IsNullOrEmpty(emailMessage.TextBody))
		{
			htmlContent = string.Empty;
			textContent = string.Empty;
		}
		else if (string.IsNullOrEmpty(emailMessage.HtmlBody) &&
			!string.IsNullOrEmpty(emailMessage.TextBody))
		{
			textContent = emailMessage.TextBody;
			htmlContent = TfConverters.ConvertPlainTextToHtml(emailMessage.TextBody);
		}
		else if (!string.IsNullOrEmpty(emailMessage.HtmlBody) &&
			string.IsNullOrEmpty(emailMessage.TextBody))
		{
			htmlContent = emailMessage.HtmlBody;
			textContent = TfConverters.ConvertHtmlToPlainText(emailMessage.HtmlBody);
		}
		else
		{
			htmlContent = emailMessage.HtmlBody;
			textContent = emailMessage.TextBody;
		}

		const string SQL = @"
INSERT INTO email_message(
	id,
	subject,
	content_text,
	content_html,
	sent_on,
	created_on,
	server_error,
	retries_count,
	priority,
	reply_to_email,
	scheduled_on,
	status, sender,
	recipients,
	recipients_cc,
	recipients_bcc,
	attachments,
	x_search,
	user_id,
	related_row_ids
)
VALUES 
(
	@id, 
	@subject, 
	@content_text, 
	@content_html, 
	@sent_on, 
	@created_on, 
	@server_error, 
	@retries_count, 
	@priority, 
	@reply_to_email, 
	@scheduled_on, 
	@status, 
	@sender, 
	@recipients, 
	@recipients_cc, 
	@recipients_bcc, 
	@attachments, 
	@x_search,
	@user_id,
	@related_row_ids
)";

		var dbResult = _dbService.ExecuteSqlNonQueryCommand(
			SQL,
			CreateParameter("id", id, DbType.Guid),
			CreateParameter("sender", JsonSerializer.Serialize(sender), DbType.String),
			CreateParameter("recipients", JsonSerializer.Serialize(emailMessage.Recipients ?? new()), DbType.String),
			CreateParameter("recipients_cc", JsonSerializer.Serialize(emailMessage.RecipientsCc ?? new()), DbType.String),
			CreateParameter("recipients_bcc", JsonSerializer.Serialize(emailMessage.RecipientsBcc ?? new()), DbType.String),
			CreateParameter("attachments", JsonSerializer.Serialize(validAttachments), DbType.String),
			CreateParameter("subject", emailMessage.Subject, DbType.String),
			CreateParameter("content_html", htmlContent, DbType.String),
			CreateParameter("content_text", textContent, DbType.String),
			CreateParameter("priority", (short)emailMessage.Priority, DbType.Int16),
			CreateParameter("status", (short)EmailStatus.Pending, DbType.Int16),
			CreateParameter("created_on", DateTime.Now, DbType.DateTime2),
			CreateParameter("sent_on", null, DbType.DateTime2),
			CreateParameter("scheduled_on", DateTime.Now, DbType.DateTime2),
			CreateParameter("server_error", string.Empty, DbType.String),
			CreateParameter("retries_count", (short)0, DbType.Int16),
			CreateParameter("reply_to_email", replyToEmail, DbType.String),
			CreateParameter("x_search", GenerateSearch(emailMessage), DbType.String),
			CreateParameter("user_id", emailMessage.UserId, DbType.Guid),
			CreateParameter("related_row_ids", JsonSerializer.Serialize(emailMessage.RelatedRowIds ?? new()), DbType.String)
		);

		if (dbResult != 1)
			throw new Exception("Tefter failed to save email to database");

		return GetEmailMessageById(id);
	}

	public List<EmailMessage> GetPendingEmails()
	{
		var dt = _dbService.ExecuteSqlQueryCommand(
			@"SELECT * FROM email_message WHERE status = @status AND scheduled_on IS NOT NULL" +
			" AND scheduled_on < @scheduled_on  ORDER BY priority DESC, scheduled_on ASC  LIMIT 10",
			CreateParameter("status", (short)EmailStatus.Pending, DbType.Int16),
			CreateParameter("scheduled_on", DateTime.Now, DbType.DateTime2));

		return CreateModelListFromDataTable(dt);
	}

	public List<EmailMessage> CreateModelListFromDataTable(
		DataTable dt)
	{
		var result = new List<EmailMessage>();

		if (dt is null)
		{
			return result;
		}

		foreach (DataRow dr in dt.Rows)
		{

			TfUser user = null;

			Guid? userId = dr.Field<Guid?>("user_id");

			if (userId is not null)
			{
				user = _tfService.GetUser(userId.Value);
			}

			result.Add(new EmailMessage
			{
				Id = dr.Field<Guid>("id"),
				Sender = JsonSerializer.Deserialize<EmailAddress>(dr.Field<string>("sender")),
				Recipients = JsonSerializer.Deserialize<List<EmailAddress>>(dr.Field<string>("recipients")),
				RecipientsCc = JsonSerializer.Deserialize<List<EmailAddress>>(dr.Field<string>("recipients_cc")),
				RecipientsBcc = JsonSerializer.Deserialize<List<EmailAddress>>(dr.Field<string>("recipients_bcc")),
				ReplyToEmail = dr.Field<string>("reply_to_email"),
				Subject = dr.Field<string>("subject"),
				ContentHtml = dr.Field<string>("content_html"),
				ContentText = dr.Field<string>("content_text"),
				CreatedOn = dr.Field<DateTime>("created_on"),
				SentOn = dr.Field<DateTime?>("sent_on"),
				ScheduledOn = dr.Field<DateTime?>("scheduled_on"),
				Status = (EmailStatus)dr.Field<short>("status"),
				Priority = (EmailPriority)dr.Field<short>("priority"),
				ServerError = dr.Field<string>("server_error"),
				RetriesCount = dr.Field<short>("retries_count"),
				Attachments = JsonSerializer.Deserialize<List<EmailAttachment>>(dr.Field<string>("attachments")),
				XSearch = dr.Field<string>("x_search"),
				RelatedRowIds = JsonSerializer.Deserialize<List<Guid>>(dr.Field<string>("related_row_ids")),
				User = user
			});

		}
		return result;
	}

	public NpgsqlParameter CreateParameter(
		string name,
		object value,
		DbType type)
	{
		NpgsqlParameter par = new NpgsqlParameter(name, type);
		if (value is null)
			par.Value = DBNull.Value;
		else
			par.Value = value;

		return par;
	}


	#region <--- utility --->

	private static string GenerateSearch(CreateEmailMessageModel emailMessage)
	{
		var recipientsText = string.Join(" ", emailMessage.Recipients.Select(x => $"{x.Name} {x.Address}"));
		var recipientsCcText = string.Join(" ", emailMessage.RecipientsCc.Select(x => $"{x.Name} {x.Address}"));
		var recipientsBccText = string.Join(" ", emailMessage.RecipientsBcc.Select(x => $"{x.Name} {x.Address}"));
		return $"{emailMessage.Sender?.Name} {emailMessage.Sender?.Address} {recipientsText} {recipientsCcText} {recipientsBccText} " +
			   $" {emailMessage.Subject} {emailMessage.HtmlBody} {emailMessage.TextBody}";
	}

	#endregion

	#region <--- validation --->

	internal class EmailMessageValidator : AbstractValidator<CreateEmailMessageModel>
	{
		public readonly ITfDatabaseService _dbService;
		public readonly ITfService _tfService;
		public readonly ISmtpConfigurationService _config;

		public EmailMessageValidator(
			ITfDatabaseService dbService,
			ITfService tfService,
			ISmtpConfigurationService config)
		{
			_dbService = dbService;
			_tfService = tfService;
			_config = config;

			RuleSet("create", () =>
			{
				RuleFor(emailMessage => emailMessage.Subject)
					.NotEmpty()
					.WithMessage("Subject is required");

				RuleFor(emailMessage => emailMessage.Recipients)
					.Must((emailMessage, recipients) =>
					{
						if (recipients is null)
							return false;

						foreach (var recipient in recipients)
						{
							if (recipient == null)
								return false;

							if (string.IsNullOrEmpty(recipient.Address))
								return false;

							if (!recipient.Address.IsEmail())
								return false;

						}
						return true;
					})
					.WithMessage("Invalid recipients");

				RuleFor(emailMessage => emailMessage.RecipientsCc)
					.Must((emailMessage, recipientsCC) =>
					{
						if (recipientsCC is null)
							return true;

						foreach (var recipient in recipientsCC)
						{
							if (recipient == null)
								return false;

							if (string.IsNullOrEmpty(recipient.Address))
								return false;

							if (!recipient.Address.IsEmail())
								return false;

						}
						return true;
					})
					.WithMessage("Invalid CC recipients");

				RuleFor(emailMessage => emailMessage.RecipientsBcc)
					.Must((emailMessage, recipientsBCC) =>
					{
						if (recipientsBCC is null)
							return true;

						foreach (var recipient in recipientsBCC)
						{
							if (recipient == null)
								return false;

							if (string.IsNullOrEmpty(recipient.Address))
								return false;

							if (!recipient.Address.IsEmail())
								return false;

						}
						return true;
					})
					.WithMessage("Invalid BCC recipients");

				RuleFor(emailMessage => emailMessage.ReplyTo)
					.Must((emailMessage, replyTo) =>
					{
						if (!string.IsNullOrWhiteSpace(replyTo))
						{
							string[] replyToEmails = replyTo.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
							foreach (var replyEmail in replyToEmails)
							{
								if (!replyEmail.IsEmail())
									return false;
							}
						}
						return true;
					})
					.WithMessage("Invalid ReplyTo");


				RuleFor(emailMessage => emailMessage.Attachments)
					.Must((emailMessage, attachments) =>
					{
						if (attachments is null)
							return true;

						foreach (var attachment in attachments)
						{
							if (attachment == null)
								return false;

							if (string.IsNullOrWhiteSpace(attachment.Filename))
								return false;
						}
						return true;
					})
					.WithMessage("Invalid attachments");

			});
		}

		public ValidationResult ValidateCreate(
			CreateEmailMessageModel emailMessage)
		{
			if (emailMessage == null)
			{
				return new ValidationResult(new[]
				{
					new ValidationFailure("","The email message id null.")
				});
			}

			return this.Validate(emailMessage, options =>
			{
				options.IncludeRuleSets("create");
			});
		}
	}

	#endregion

	#region <--- content processing --->



	public void EmbedRepositoryImages(
		BodyBuilder builder)
	{
		if (builder == null)
			return;

		if (string.IsNullOrWhiteSpace(builder.HtmlBody))
			return;

		try
		{
			var htmlDoc = new HtmlDocument();
			htmlDoc.Load(new MemoryStream(Encoding.UTF8.GetBytes(builder.HtmlBody)));

			if (htmlDoc.DocumentNode == null)
				return;

			var nodes = htmlDoc.DocumentNode.SelectNodes("//img[@src]");
			if (nodes is not null)
			{
				foreach (HtmlNode node in nodes)
				{
					var src = node.Attributes["src"].Value.Split('?', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();


					if (!string.IsNullOrWhiteSpace(src) &&
						src.ToLowerInvariant().StartsWith("/fs/repository/"))
					{
						try
						{
							Uri uri = new Uri(src);
							src = uri.AbsolutePath;
						}
						catch
						{
						}

						string filename = src.ToLowerInvariant().Replace("/fs/repository/", "");

						var file = _tfService.GetRepositoryFile(filename);
						if (file == null)
							continue;

						var bytes = _tfService.GetRepositoryFileContentAsByteArray(filename);

						var extension = Path.GetExtension(src).ToLowerInvariant();
						new FileExtensionContentTypeProvider().Mappings.TryGetValue(extension, out string mimeType);

						var imagePart = new MimePart(mimeType)
						{
							ContentId = MimeUtils.GenerateMessageId(),
							Content = new MimeContent(new MemoryStream(bytes)),
							ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
							ContentTransferEncoding = ContentEncoding.Base64,
							FileName = Path.GetFileName(src)
						};

						builder.LinkedResources.Add(imagePart);
						node.SetAttributeValue("src", $"cid:{imagePart.ContentId}");
					}
				}
			}

			builder.HtmlBody = htmlDoc.DocumentNode.OuterHtml;
			if (string.IsNullOrWhiteSpace(builder.TextBody) && !string.IsNullOrWhiteSpace(builder.HtmlBody))
				builder.TextBody = TfConverters.ConvertHtmlToPlainText(builder.HtmlBody);
		}
		catch
		{
			return;
		}
	}



	#endregion
}
