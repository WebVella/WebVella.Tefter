namespace WebVella.Tefter.Web.Utils;

public partial class TfConstants
{
	public static string PageQueryName = "pg";
	public static string PageSizeQueryName = "pgs";
	public static string SearchQueryName = "sch";
	public static string FiltersQueryName = "flt";
	public static string SortsQueryName = "srt";
	public static string ActiveSaveQueryName = "svid";

	public static string SearchInBookmarksQueryName = "sbkm";
	public static string SearchInSavesQueryName = "ssvs";
	public static string SearchInViewsQueryName = "svws";
	public static string SearchInSpacesQueryName = "ssps";


	public static string NotFoundPageUrl = "/404";
	public static string LoginPageUrl  = "/login";
	public static string HomePageUrl  = "/";

	public static string RouteNameAdmin  = "admin";
	public static string RouteNameUsers  = "users";
	public static string RouteNameAccess  = "access";
	public static string RouteNameSaves  = "saves";
	public static string RouteNameDataProviders  = "data-providers";
	public static string RouteNameSchema  = "schema";
	public static string RouteNameKeys  = "keys";
	public static string RouteNameAux  = "aux";
	public static string RouteNameSynchronization  = "synchronization";
	public static string RouteNameData  = "data";
	public static string RouteNameSharedColumns  = "aux-columns";
	public static string AdminPageUrl  = $"/{RouteNameAdmin}";
	public static string AdminUsersPageUrl  = $"/{RouteNameAdmin}/{RouteNameUsers}";
	public static string AdminUserDetailsPageUrl  = $"/{RouteNameAdmin}/{RouteNameUsers}/{{0}}";
	public static string AdminUserAccessPageUrl  = $"/{RouteNameAdmin}/{RouteNameUsers}/{{0}}/{RouteNameAccess}";
	public static string AdminUserSavesViewsPageUrl  = $"/{RouteNameAdmin}/{RouteNameUsers}/{{0}}/{RouteNameSaves}";

	public static string AdminDataProvidersPageUrl  = $"/{RouteNameAdmin}/{RouteNameDataProviders}";
	public static string AdminDataProviderDetailsPageUrl  = $"/{RouteNameAdmin}/{RouteNameDataProviders}/{{0}}";
	public static string AdminDataProviderSchemaPageUrl  = $"/{RouteNameAdmin}/{RouteNameDataProviders}/{{0}}/{RouteNameSchema}";
	public static string AdminDataProviderKeysPageUrl  = $"/{RouteNameAdmin}/{RouteNameDataProviders}/{{0}}/{RouteNameKeys}";
	public static string AdminDataProviderAuxColumnsPageUrl  = $"/{RouteNameAdmin}/{RouteNameDataProviders}/{{0}}/{RouteNameAux}";
	public static string AdminDataProviderSynchronizationPageUrl  = $"/{RouteNameAdmin}/{RouteNameDataProviders}/{{0}}/{RouteNameSynchronization}";
	public static string AdminDataProviderDataPageUrl  = $"/{RouteNameAdmin}/{RouteNameDataProviders}/{{0}}/{RouteNameData}";
	public static string AdminSharedColumnsPageUrl  = $"/{RouteNameAdmin}/{RouteNameSharedColumns}";

	public static string RouteNameSpace  = "space";
	public static string RouteNameSpaceView  = "view";
	public static string RouteNameSpaceData  = "data";
	public static string RouteNameManage  = "manage";
	public static string RouteNameViews  = "views";
	public static string SpacePageUrl  = $"/{RouteNameSpace}/{{0}}";
	public static string SpaceViewPageUrl  = $"/{RouteNameSpace}/{{0}}/{RouteNameSpaceView}/{{1}}";
	public static string SpaceViewManagePageUrl  = $"/{RouteNameSpace}/{{0}}/{RouteNameSpaceView}/{{1}}/{RouteNameManage}";
	public static string SpaceDataPageUrl  = $"/{RouteNameSpace}/{{0}}/{RouteNameSpaceData}/{{1}}";
	public static string SpaceDataViewsPageUrl  = $"/{RouteNameSpace}/{{0}}/{RouteNameSpaceData}/{{1}}/{RouteNameViews}";

}
