namespace WebVella.Tefter.Talk;
internal class Constants
{
	public const string TALK_APP_ID_STRING = "27a7703a-8fe8-4363-aee1-64a219d7520e";
	public static Guid TALK_APP_ID = new Guid(TALK_APP_ID_STRING);
	public static string TALK_APP_NAME = "Talk Application";
	public static string TALK_APP_DECRIPTION = "Talk Application Description";
	public static string TALK_APP_CHANNEL_LIST_DATA_KEY = $"{TALK_APP_ID}-channels-list";
	public static string TALK_APP_SHARED_COLUMNS_LIST_DATA_KEY = $"{TALK_APP_ID}-shared-columns-list";
}
