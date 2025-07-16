using DocumentFormat.OpenXml.Office.CustomUI;

namespace WebVella.Tefter;

public partial class TfConstants
{
	public static Icon? GetIcon(string? name, IconSize size = IconSize.Size20, IconVariant variant = IconVariant.Regular, string? defaultIcon = null)
	{
		try
		{
			if (String.IsNullOrWhiteSpace(name) && String.IsNullOrWhiteSpace(defaultIcon))
				return null;
			if (String.IsNullOrWhiteSpace(name)) return GetIcon(name: defaultIcon, size: size, variant: variant);
			return IconsExtensions.GetInstance(new IconInfo { Name = name, Size = size, Variant = variant });
		}
		catch
		{
			return IconsExtensions.GetInstance(new IconInfo { Name = "ErrorCircle", Size = IconSize.Size20, Variant = IconVariant.Regular }).WithColor(Color.Error);
		}
	}

	public static Icon HomeIcon = GetIcon("Home") ?? ErrorIcon!;
	public static Icon ApplicationIcon = GetIcon("AppFolder") ?? ErrorIcon!;
	public static Icon SettingsIcon = GetIcon("Settings") ?? ErrorIcon!;
	public static string HomeMenuTitle = "Home";
	public static string AdminMenuTitle = "Administration";
	public static string PagesMenuTitle = "Application Pages";

	public static Icon BookmarkONIcon = GetIcon("Bookmark", variant: IconVariant.Filled) ?? ErrorIcon!;
	public static Icon BookmarkOFFIcon = GetIcon("Bookmark") ?? ErrorIcon!;
	public static Icon BookmarkRemoveIcon = GetIcon("BookmarkOff") ?? ErrorIcon!;
	public static Icon BookmarkEditIcon = GetIcon("BookmarkSearch") ?? ErrorIcon!;

	public static string SpaceIconName = "Folder";
	public static Icon SpaceIcon = GetIcon(SpaceIconName) ?? ErrorIcon!;
	public static Icon SpaceViewIcon = GetIcon("Table") ?? ErrorIcon!;
	public static Icon SpaceDataIcon = GetIcon("Database") ?? ErrorIcon!;


	//Admin
	public static Icon AdminDashboardIcon = GetIcon("Board") ?? ErrorIcon!;
	public static string AdminDashboardMenuTitle = "Dashboard";
	public static string AdminAddonsMenuTitle = "Addons";

	public static Icon AdminDataIcon = GetIcon("Database") ?? ErrorIcon!;
	public static string AdminDataMenuTitle = "Data";

	public static Icon AdminDataProviderIcon = GetIcon("CloudDatabase") ?? ErrorIcon!;
	public static string AdminDataProvidersMenuTitle = "Data Providers";

	public static Icon AdminDataIdentityIcon = GetIcon("KeyMultiple") ?? ErrorIcon!;
	public static string AdminDataIdentitiesMenuTitle = "Data Identities";

	public static Icon AdminUserIcon = GetIcon("Person") ?? ErrorIcon!;
	public static Icon AdminUsersIcon = GetIcon("People") ?? ErrorIcon!;
	public static Icon AdminRoleIcon = GetIcon("Key") ?? ErrorIcon!;
	public static string AdminAccessMenuTitle = "Access";
	public static string AdminUsersMenuTitle = "Users Accounts";
	public static string AdminRolesMenuTitle = "Access Roles";

	public static Icon AdminSharedColumnsIcon = GetIcon("BookDatabase") ?? ErrorIcon!;
	public static string AdminSharedColumnsMenuTitle = "Shared Columns";

	public static Icon AdminFileRepositoryIcon = GetIcon("DocumentDatabase") ?? ErrorIcon!;
	public static string AdminFileRepositoryMenuTitle = "File Repository";

	public static Icon TemplateIcon = GetIcon("CalendarTemplate") ?? ErrorIcon!;
	public static string AdminTemplatesMenuTitle = "Templates";
	public static string AdminTemplateMenuTitle = "Template {0}";

	public static Icon ContentIcon = GetIcon("Folder") ?? ErrorIcon!;
	public static string AdminContentMenuTitle = "Content";


	//Action icons
	public static string PageIconString = "Document";
	public static string FolderIconString = "Folder";
	public static Icon PageIcon = GetIcon(PageIconString) ?? ErrorIcon!;
	public static Icon FolderIcon = GetIcon(FolderIconString) ?? ErrorIcon!;
	public static Icon CloseIcon = GetIcon("Dismiss") ?? ErrorIcon!;
	public static Icon SaveIcon = GetIcon("Save") ?? ErrorIcon!;
	public static Icon CancelIcon = GetIcon("Star") ?? ErrorIcon!;
	public static Icon SearchIcon = GetIcon("Search") ?? ErrorIcon!;
	public static Icon AddIcon = GetIcon("Add") ?? ErrorIcon!;
	public static Icon EditIcon = GetIcon("Edit") ?? ErrorIcon!;
	public static Icon LockIcon = GetIcon("LockClosed") ?? ErrorIcon!;
	public static Icon DeleteIcon = GetIcon("Delete") ?? ErrorIcon!;
	public static Icon ViewIcon = GetIcon("Eye") ?? ErrorIcon!;
	public static Icon HelpIcon = GetIcon("QuestionCircle") ?? ErrorIcon!;
	public static Icon RectangleIcon = GetIcon("RectangleLandscape", variant: IconVariant.Filled) ?? ErrorIcon!;
	public static Icon ErrorIcon = IconsExtensions.GetInstance(new IconInfo { Name = "ErrorCircle", Size = IconSize.Size20, Variant = IconVariant.Filled }).WithColor(Color.Error);

	//Storage keys
	public static string SpaceViewOpenedGroupsLocalStorageKey = "tf-spaceview-opened-groups";
}


