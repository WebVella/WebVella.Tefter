namespace WebVella.Tefter.Web.Utils;

public partial class TfConstants
{
	public static string NotFoundPageUrl { get; set; } = "/404";
	public static string LoginPageUrl { get; set; } = "/login";
	public static string HomePageUrl { get; set; } = "/";
	public static string AdminPageUrl { get; set; } = "/admin";
	public static string AdminUsersPageUrl { get; set; } = "/admin/users";
	public static string AdminUserDetailsPageUrl { get; set; } = "/admin/users/{0}";
	public static string AdminUserAccessPageUrl { get; set; } = "/admin/users/{0}/access";
	public static string AdminUserSavesViewsPageUrl { get; set; } = "/admin/users/{0}/saves";
	public static string AdminDataProvidersPageUrl { get; set; } = "/admin/data-providers";
	public static string AdminDataProviderDetailsPageUrl { get; set; } = "/admin/data-providers/{0}";
	public static string AdminDataProviderSchemaPageUrl { get; set; } = "/admin/data-providers/{0}/schema";
	public static string AdminDataProviderKeysPageUrl { get; set; } = "/admin/data-providers/{0}/keys";
	public static string AdminDataProviderAuxColumnsPageUrl { get; set; } = "/admin/data-providers/{0}/aux";
	public static string AdminDataProviderSynchronizationPageUrl { get; set; } = "/admin/data-providers/{0}/synchronization";
	public static string AdminDataProviderDataPageUrl { get; set; } = "/admin/data-providers/{0}/data";
	public static string SpacePageUrl { get; set; } = "/space/{0}";

}
