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

	public static Icon DashboardIcon = GetIcon("Board");
	public static string DashboardMenuTitle = "Dashboard";

	public static Icon FastAccessIcon = GetIcon("VideoPersonStar");
	public static string FastAccesMenuTitle = "Fast Access";

	public static Icon BookmarkONIcon = GetIcon("Star", variant: IconVariant.Filled);
	public static Icon BookmarkOFFIcon = GetIcon("Star");

	public static Icon SpaceIcon = GetIcon("VideoPersonStar");
	public static Icon SpaceViewIcon = GetIcon("Grid");
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
}


