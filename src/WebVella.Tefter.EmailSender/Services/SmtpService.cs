namespace WebVella.Tefter.EmailSender.Services;

internal partial interface ISmtpService
{
	public ValueTask ProcessSmtpQueue();
}

internal partial class SmtpService : ISmtpService
{

	private const int MAX_RETRIES = 3;
	private const int RETRY_WAIT_MINS = 60;
	private static bool queueProcessingInProgress = false;
	private static object queueLockObj = new object();

	private readonly IEmailService _emailService;
	public readonly ITfDatabaseService _dbService;
	private readonly ISmtpConfigurationService _config;
	private readonly ITfService _tfService;

	public SmtpService(
		IEmailService emailService,
		ITfDatabaseService dbService,
		ISmtpConfigurationService config,
		ITfService tfService)
	{
		_dbService = dbService;
		_config = config;
		_tfService = tfService;
		_emailService = emailService;
	}

	
	public async ValueTask ProcessSmtpQueue()
	{
		lock (queueLockObj)
		{
			if (queueProcessingInProgress)
				return;

			queueProcessingInProgress = true;
		}

		try
		{
			List<EmailMessage> pendingEmailMessages = new List<EmailMessage>();
			do
			{
				pendingEmailMessages = _emailService.GetPendingEmails();

				foreach (var emailMessage in pendingEmailMessages)
				{
					await SendEmailAsync(emailMessage);
				}
			} while (pendingEmailMessages.Count > 0);
		}
		finally
		{
			lock (queueLockObj)
			{
				queueProcessingInProgress = false;
			}
		}
	}

	private async ValueTask SendEmailAsync(
		EmailMessage emailMessage)
	{
		try
		{
			var message = new MimeMessage();
			message.From.Add(!string.IsNullOrWhiteSpace(emailMessage.Sender?.Name)
				? new MailboxAddress(emailMessage.Sender?.Name, emailMessage.Sender?.Address)
				: new MailboxAddress(emailMessage.Sender?.Address, emailMessage.Sender?.Address));

			foreach (var recipient in emailMessage.Recipients)
			{
				message.To.Add(!string.IsNullOrWhiteSpace(recipient.Name)
					? new MailboxAddress(recipient.Name, recipient.Address)
					: new MailboxAddress(recipient.Address, recipient.Address));
			}

			if (emailMessage.RecipientsCc is { Count: > 0 })
			{
				foreach (var recipient in emailMessage.RecipientsCc)
				{
					message.Cc.Add(!string.IsNullOrWhiteSpace(recipient.Name)
						? new MailboxAddress(recipient.Name, recipient.Address)
						: new MailboxAddress(recipient.Address, recipient.Address));
				}
			}

			if (emailMessage.RecipientsBcc is { Count: > 0 })
			{
				foreach (var recipient in emailMessage.RecipientsBcc)
				{
					message.Bcc.Add(!string.IsNullOrWhiteSpace(recipient.Name)
						? new MailboxAddress(recipient.Name, recipient.Address)
						: new MailboxAddress(recipient.Address, recipient.Address));
				}
			}

			if (!string.IsNullOrWhiteSpace(emailMessage.ReplyToEmail))
			{
				string[] replyToEmails = emailMessage.ReplyToEmail.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
				foreach (var replyEmail in replyToEmails)
					message.ReplyTo.Add(new MailboxAddress(replyEmail, replyEmail));
			}
			else
				message.ReplyTo.Add(new MailboxAddress(emailMessage.Sender?.Address, emailMessage.Sender?.Address));

			message.Subject = emailMessage.Subject;
			var bodyBuilder = new BodyBuilder
			{
				HtmlBody = emailMessage.ContentHtml??string.Empty,
				TextBody = emailMessage.ContentText??string.Empty
			};

			if (emailMessage.Attachments != null && emailMessage.Attachments.Count > 0)
			{
				foreach (var att in emailMessage.Attachments)
				{
					var bytes = _tfService.GetBlobByteArray(att.BlobId);

					var extension = Path.GetExtension(att.Filename).ToLowerInvariant();
					new FileExtensionContentTypeProvider().Mappings.TryGetValue(extension, out string mimeType);

					var attachment = new MimePart(mimeType)
					{
						Content = new MimeContent(new MemoryStream(bytes)),
						ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
						ContentTransferEncoding = ContentEncoding.Base64,
						FileName = att.Filename
					};

					bodyBuilder.Attachments.Add(attachment);
				}
			}

			_emailService.EmbedRepositoryImages(bodyBuilder);

			message.Body = bodyBuilder.ToMessageBody();

			using (var client = new SmtpClient()) //when need advaced log new ProtocolLogger("smtp.log")
			{
				//accept all SSL certificates (in case the server supports STARTTLS)
				client.ServerCertificateValidationCallback = (s, c, h, e) => true;

				client.Connect(_config.Server, _config.Port, SecureSocketOptions.Auto);

				if (!string.IsNullOrWhiteSpace(_config.Username))
					client.Authenticate(_config.Username, _config.Password);

				await client.SendAsync(message);
				client.Disconnect(true);
			}

			emailMessage.SentOn = DateTime.Now;
			emailMessage.Status = EmailStatus.Sent;
			emailMessage.ScheduledOn = null;
			emailMessage.ServerError = null;
		}
		catch (Exception ex)
		{
			emailMessage.SentOn = null;
			emailMessage.ServerError = ex.Message;
			emailMessage.RetriesCount++;
			if (emailMessage.RetriesCount > MAX_RETRIES)
			{
				emailMessage.ScheduledOn = null;
				emailMessage.Status = EmailStatus.Aborted;
			}
			else
			{
				emailMessage.ScheduledOn = DateTime.Now.AddMinutes(emailMessage.RetriesCount * RETRY_WAIT_MINS);
				emailMessage.Status = EmailStatus.Pending;
			}
		}
		finally
		{
			const string SQL = @"UPDATE email_message SET 
				sent_on = @sent_on,
				status = @status,
				scheduled_on = @scheduled_on,
				server_error = @server_error,
				retries_count = @retries_count
			WHERE id = @id";

			var dbResult = _dbService.ExecuteSqlNonQueryCommand(
				SQL,
				_emailService.CreateParameter("id", emailMessage.Id, DbType.Guid),
				_emailService.CreateParameter("status", (short)emailMessage.Status, DbType.Int16),
				_emailService.CreateParameter("sent_on", emailMessage.SentOn, DbType.DateTime2),
				_emailService.CreateParameter("scheduled_on", emailMessage.ScheduledOn, DbType.DateTime2),
				_emailService.CreateParameter("server_error", emailMessage.ServerError, DbType.String),
				_emailService.CreateParameter("retries_count", (short)emailMessage.RetriesCount, DbType.Int16));

			if (dbResult != 1)
			{
				throw new Exception("Failed to update email message in database");
			}
		}
	}
}
