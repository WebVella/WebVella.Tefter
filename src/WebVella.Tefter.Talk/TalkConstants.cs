﻿namespace WebVella.Tefter.Talk;
internal class TalkConstants
{
	public const string TALK_APP_ID_STRING = "27a7703a-8fe8-4363-aee1-64a219d7520e";
	public static Guid TALK_APP_ID = new Guid(TALK_APP_ID_STRING);
	public static string TALK_APP_NAME = "Talk Application";
	public static string TALK_APP_DECRIPTION = "Talk Application Description";
	public static string TALK_APP_CHANNEL_LIST_DATA_KEY = $"{TALK_APP_ID}-channels-list";
	public static string TALK_APP_SHARED_COLUMNS_LIST_DATA_KEY = $"{TALK_APP_ID}-shared-columns-list";

	public static string TALK_APP_SPACE_VIEW_COLUMN_TYPE_COMMENTS_COUNT = $"60ab4197-be46-4ebd-a6de-02e8d25450d3";
	public static string TALK_APP_SPACE_VIEW_COLUMN_COMPONENT_COMMENTS_COUNT = $"5f3855f1-4819-488f-b24a-d4a81448e4f0";
}
