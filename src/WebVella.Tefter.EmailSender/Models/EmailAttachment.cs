namespace WebVella.Tefter.EmailSender.Models;

public class EmailAttachment
{
	public string Filename { get; set; }
	public Guid BlobId { get; set; }
}

public class CreateEmailAttachmentModel
{
	public string Filename { get; set; }
	public byte[] Buffer { get; set; }
}