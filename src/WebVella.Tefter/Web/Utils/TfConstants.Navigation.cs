namespace WebVella.Tefter.Web.Utils;

public partial class TfConstants
{
	public static Icon GetIcon(string name, IconSize size = IconSize.Size20, IconVariant variant = IconVariant.Regular)
	{
		try
		{
			return Icons.GetInstance(new IconInfo { Name = name, Size = size, Variant = variant });
		}
		catch
		{
			return Icons.GetInstance(new IconInfo { Name = "ErrorCircle", Size = IconSize.Size20, Variant = IconVariant.Regular }).WithColor(Color.Error);
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

	public static Icon AdminDataProvidersIcon = GetIcon("Connector");
	public static string AdminDataProvidersMenuTitle = "Data providers";

	public static Icon AdminUsersIcon = GetIcon("People");
	public static string AdminUsersMenuTitle = "Users";

	public static Icon AdminAuxColumnsIcon = GetIcon("Column");
	public static string AdminAuxColumnsMenuTitle = "Shared Columns";


	//Action icons
	public static Icon SaveIcon = GetIcon("Save");
	public static Icon CancelIcon = GetIcon("Star");
	public static Icon SearchIcon = GetIcon("Search");
	public static Icon AddIcon = GetIcon("Add");
	public static Icon EditIcon = GetIcon("Edit");
	public static Icon DeleteIcon = GetIcon("Delete");
	public static Icon ViewIcon = GetIcon("Eye");
	public static Icon RectangleIcon = GetIcon("RectangleLandscape",variant:IconVariant.Filled);
	public static Icon ErrorIcon = Icons.GetInstance(new IconInfo { Name = "ErrorCircle", Size = IconSize.Size20, Variant = IconVariant.Filled }).WithColor(Color.Error);
}


