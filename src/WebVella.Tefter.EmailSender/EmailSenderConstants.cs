namespace WebVella.Tefter.EmailSender;
internal class EmailSenderConstants
{
	public const string APP_ID_STRING = "0c847f54-28c0-4314-9151-bb9226d42033";
	public static Guid  APP_ID = new Guid(APP_ID_STRING);
	public static string APP_NAME = "Email Sender Application";
	public static string APP_DECRIPTION = "Email Sender Application Description";
	public static string APP_EMAIL_LIST_DATA_KEY = $"{APP_ID_STRING}-email-list";
}
