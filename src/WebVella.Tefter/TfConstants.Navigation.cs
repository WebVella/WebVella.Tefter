﻿using DocumentFormat.OpenXml.Office.CustomUI;

namespace WebVella.Tefter;

public partial class TfConstants
{
	public static Icon GetIcon(string name, IconSize size = IconSize.Size20, IconVariant variant = IconVariant.Regular)
	{
		try
		{
			return IconsExtensions.GetInstance(new IconInfo { Name = name, Size = size, Variant = variant });
		}
		catch
		{
			return IconsExtensions.GetInstance(new IconInfo { Name = "ErrorCircle", Size = IconSize.Size20, Variant = IconVariant.Regular }).WithColor(Color.Error);
		}
	}

	public static Icon HomeIcon = GetIcon("Home");
	public static Icon ApplicationIcon = GetIcon("AppFolder");
	public static string HomeMenuTitle = "Home";
	public static string PagesMenuTitle = "Application Pages";

	public static Icon BookmarkONIcon = GetIcon("Bookmark", variant: IconVariant.Filled);
	public static Icon BookmarkOFFIcon = GetIcon("Bookmark");
	public static Icon BookmarkRemoveIcon = GetIcon("BookmarkOff");
	public static Icon BookmarkEditIcon = GetIcon("BookmarkSearch");

	public static Icon SpaceIcon = GetIcon("Folder");
	public static Icon SpaceViewIcon = GetIcon("Table");
	public static Icon SpaceDataIcon = GetIcon("Database");


	//Admin
	public static Icon AdminDashboardIcon = GetIcon("Board");
	public static string AdminDashboardMenuTitle = "Dashboard";
	public static string AdminAddonsMenuTitle = "Addons";

	public static Icon AdminDataIcon = GetIcon("Database");
	public static string AdminLocalDataMenuTitle = "Local Data";

	public static Icon AdminDataProviderIcon = GetIcon("CloudDatabase");
	public static string AdminDataProvidersMenuTitle = "Data Providers";

	public static Icon AdminDataIdentityIcon = GetIcon("KeyMultiple");
	public static string AdminDataIdentitiesMenuTitle = "Data Identities";

	public static Icon AdminUsersIcon = GetIcon("People");
	public static Icon AdminRoleIcon = GetIcon("Key");
	public static string AdminAccessMenuTitle = "Access";
	public static string AdminUsersMenuTitle = "Users";

	public static Icon AdminSharedColumnsIcon = GetIcon("BookDatabase");
	public static string AdminSharedColumnsMenuTitle = "Shared Columns";

	public static Icon AdminFileRepositoryIcon = GetIcon("DocumentDatabase");
	public static string AdminFileRepositoryMenuTitle = "File Repository";

	public static Icon TemplateIcon = GetIcon("CalendarTemplate");
	public static string AdminTemplatesMenuTitle = "Templates";

	public static Icon ContentIcon = GetIcon("Folder");
	public static string AdminLocalContentMenuTitle = "Local Content";


	//Action icons
	public static string PageIconString = "Document";
	public static string FolderIconString = "Folder";
	public static Icon PageIcon = GetIcon(PageIconString);
	public static Icon FolderIcon = GetIcon(FolderIconString);
	public static Icon CloseIcon = GetIcon("Dismiss");
	public static Icon SaveIcon = GetIcon("Save");
	public static Icon CancelIcon = GetIcon("Star");
	public static Icon SearchIcon = GetIcon("Search");
	public static Icon AddIcon = GetIcon("Add");
	public static Icon EditIcon = GetIcon("Edit");
	public static Icon LockIcon = GetIcon("LockClosed");
	public static Icon DeleteIcon = GetIcon("Delete");
	public static Icon ViewIcon = GetIcon("Eye");
	public static Icon HelpIcon = GetIcon("QuestionCircle");
	public static Icon RectangleIcon = GetIcon("RectangleLandscape",variant:IconVariant.Filled);
	public static Icon ErrorIcon = IconsExtensions.GetInstance(new IconInfo { Name = "ErrorCircle", Size = IconSize.Size20, Variant = IconVariant.Filled }).WithColor(Color.Error);

	//Storage keys
	public static string SpaceViewOpenedGroupsLocalStorageKey = "tf-spaceview-opened-groups";
}


