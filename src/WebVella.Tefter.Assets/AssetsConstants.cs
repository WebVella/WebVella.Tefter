namespace WebVella.Tefter.Assets;
internal class AssetsConstants
{
	public const string ASSETS_APP_ID_STRING = "5d229b2b-5c78-48fb-b91f-6e853f24aaf2";
	public static Guid ASSETS_APP_ID = new Guid(ASSETS_APP_ID_STRING);
	public static string ASSETS_APP_NAME = "Talk Application";
	public static string ASSETS_APP_DECRIPTION = "Talk Application Description";

	public static string ASSETS_APP_FOLDER_LIST_DATA_KEY = $"{ASSETS_APP_ID}-folder-list";
	public static string ASSETS_APP_SHARED_COLUMNS_LIST_DATA_KEY = $"{ASSETS_APP_ID}-shared-columns-list";
	public static string ASSETS_ADDONS_SPACE_VIEW_COLUMN_TYPES_FOLDER_ASSET_COUNT_ID = "aafd5f8a-95d0-4f6b-8b43-c75a80316504";
	public static string ASSETS_ADDONS_SPACE_VIEW_COLUMN_COMPONENTS_FOLDER_ASSET_COUNT_ID = "6c32d6e7-8758-4916-9685-e0476275a3a2";

}
