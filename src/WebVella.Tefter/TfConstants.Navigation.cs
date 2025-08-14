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

	public static string HomeMenuTitle = "Home";
	public static string AdminMenuTitle = "Administration";
	public static string PagesMenuTitle = "Application Pages";
	public static string SpaceDataMenuTitle = "Space data";
	public static string SpaceViewMenuTitle = "Space view";

	//Admin
	public static string AdminDashboardMenuTitle = "Dashboard";
	public static string AdminAddonsMenuTitle = "Addons";
	public static string AdminDataMenuTitle = "Data";
	public static string AdminDataProvidersMenuTitle = "Data Providers";
	public static string AdminDataIdentitiesMenuTitle = "Data Identities";
	public static string AdminAccessMenuTitle = "Access";
	public static string AdminUsersMenuTitle = "Users Accounts";
	public static string AdminRolesMenuTitle = "Access Roles";
	public static string AdminSharedColumnsMenuTitle = "Shared Columns";
	public static string AdminFileRepositoryMenuTitle = "File Repository";
	public static string AdminTemplatesMenuTitle = "Templates";
	public static string AdminTemplateMenuTitle = "Template {0}";
	public static string AdminContentMenuTitle = "Content";


	//Action icons
	public static Icon ErrorIcon = IconsExtensions.GetInstance(new IconInfo { Name = "ErrorCircle", Size = IconSize.Size20, Variant = IconVariant.Filled }).WithColor(Color.Error);

	//Storage keys
	public static string SpaceViewOpenedGroupsLocalStorageKey = "tf-spaceview-opened-groups";
}


