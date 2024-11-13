namespace WebVella.Tefter.EmailSender.Services;

internal static class EmailUtility
{
	internal static List<EmailMessage> CreateModelListFromDataTable(DataTable dt )
	{
		var result = new List<EmailMessage>();

		if (dt is null)
		{
			return result;
		}

		foreach (DataRow dr in dt.Rows)
		{
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
				RetriesCount= dr.Field<short>("retries_count"),
				Attachments = JsonSerializer.Deserialize<List<EmailAttachment>>(dr.Field<string>("attachments")),
				XSearch = dr.Field<string>("x_search")
			});
			
		}
		return result;
	}
}
