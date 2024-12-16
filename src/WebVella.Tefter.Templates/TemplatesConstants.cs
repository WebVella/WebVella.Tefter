namespace WebVella.Tefter.Templates;

internal class TemplatesConstants
{
	public const string TEMPLATES_APP_ID_STRING = "e80a79d5-0b66-4cbb-9607-0a62a325f284";
	public static Guid TEMPLATES_APP_ID = new Guid(TEMPLATES_APP_ID_STRING);
	public static string TEMPLATES_APP_NAME = "Templates Application";
	public static string TEMPLATES_APP_DECRIPTION = "Templates Application Description";

	internal static short MAX_CALCULATION_DEPTH= 10;

	public static Guid EMAIL_CONTENT_PROCESSOR_ID = new Guid("12c7c882-0be3-4692-add0-469098d36032");
	public static Guid EXCEL_FILE_CONTENT_PROCESSOR_ID = new Guid("a655fffd-1a02-4e90-9e05-50595916f97a");
	public static Guid TEXT_FILE_CONTENT_PROCESSOR_ID = new Guid("1f1139b2-a92c-4ae3-9dc9-318404ab6b04");
	public static Guid TEXT_CONTENT_PROCESSOR_ID = new Guid("174c885f-dd5e-4666-900a-adfafca633cc");

	public static string TEMPLATE_APP_TEMPLATES_LIST_DATA_KEY = $"{TEMPLATES_APP_ID_STRING}-templates-list";
	public static string TEMPLATE_APP_PROCESSORS_LIST_DATA_KEY = $"{TEMPLATES_APP_ID_STRING}-processors-list";

}
