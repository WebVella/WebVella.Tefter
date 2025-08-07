namespace WebVella.Tefter;

public partial class TfConstants
{
	public static string NoDefaultRedirectQueryName = "ndr";
	public static string PageQueryName = "pg";
	public static string PageSizeQueryName = "pgs";
	public static string SearchQueryName = "sch";
	public static string AsideSearchQueryName = "asch";
	public static string FiltersQueryName = "flt";
	public static string SortsQueryName = "srt";
	public static string ActiveSaveQueryName = "svid";
	public static string TabQueryName = "tab";
	public static string PresetIdQueryName = "preset";

	public static string SearchInBookmarksQueryName = "sbkm";
	public static string SearchInSavesQueryName = "ssvs";
	public static string SearchInViewsQueryName = "svws";


	public static string NotFoundPageUrl = "/404";
	public static string LoginPageUrl = "/login";
	public static string HomePageUrl = "/";

	public static string RouteNameAdmin = "admin";
	public static string RouteNameUsers = "users";
	public static string RouteNameRoles = "roles";
	public static string RouteNameAccess = "access";
	public static string RouteNameSaves = "saves";
	public static string RouteNameDataProviders = "data-providers";
	public static string RouteNameSchema = "schema";
	public static string RouteNameKeys = "keys";
	public static string RouteNameAux = "aux";
	public static string RouteNameSynchronization = "synchronization";
	public static string RouteNameData = "data";
	public static string RouteNamePages = "pages";
	public static string RouteNameColumns = "columns";
	public static string RouteNameFilters = "filters";
	public static string RouteNameSharedColumns = "shared-columns";
	public static string RouteNameDataIdentities = "data-identities";
	public static string RouteNameFileRepository = "file-repository";
	public static string RouteNameTemplates = "templates";
	public static string AdminDashboardUrl = $"/{RouteNameAdmin}";
	public static string AdminPagesUrl = $"/{RouteNameAdmin}/{RouteNamePages}";
	public static string AdminPagesSingleUrl = $"/{RouteNameAdmin}/{RouteNamePages}/{{0}}";
	public static string AdminUsersPageUrl = $"/{RouteNameAdmin}/{RouteNameUsers}";
	public static string AdminUserDetailsPageUrl = $"/{RouteNameAdmin}/{RouteNameUsers}/{{0}}";
	public static string AdminUserAccessPageUrl = $"/{RouteNameAdmin}/{RouteNameUsers}/{{0}}/{RouteNameAccess}";
	public static string AdminUserSavesViewsPageUrl = $"/{RouteNameAdmin}/{RouteNameUsers}/{{0}}/{RouteNameSaves}";
	public static string AdminRolesPageUrl = $"/{RouteNameAdmin}/{RouteNameRoles}";
	public static string AdminRoleDetailsPageUrl = $"/{RouteNameAdmin}/{RouteNameRoles}/{{0}}";

	public static string AdminDataProvidersPageUrl = $"/{RouteNameAdmin}/{RouteNameDataProviders}";
	public static string AdminDataProviderDetailsPageUrl = $"/{RouteNameAdmin}/{RouteNameDataProviders}/{{0}}";
	public static string AdminDataIdentityDetailsPageUrl = $"/{RouteNameAdmin}/{RouteNameDataIdentities}/{{0}}";
	public static string AdminDataProviderSchemaPageUrl = $"/{RouteNameAdmin}/{RouteNameDataProviders}/{{0}}/{RouteNameSchema}";
	public static string AdminDataProviderAuxPageUrl = $"/{RouteNameAdmin}/{RouteNameDataProviders}/{{0}}/{RouteNameAux}";
	public static string AdminDataProviderSynchronizationPageUrl = $"/{RouteNameAdmin}/{RouteNameDataProviders}/{{0}}/{RouteNameSynchronization}";
	public static string AdminDataProviderDataPageUrl = $"/{RouteNameAdmin}/{RouteNameDataProviders}/{{0}}/{RouteNameData}";
	public static string AdminSharedColumnsPageUrl = $"/{RouteNameAdmin}/{RouteNameSharedColumns}";
	public static string AdminSharedColumnDetailsPageUrl = $"/{RouteNameAdmin}/{RouteNameSharedColumns}/{{0}}";
	public static string AdminDataIdentitiesPageUrl = $"/{RouteNameAdmin}/{RouteNameDataIdentities}";
	public static string AdminFileRepositoryPageUrl = $"/{RouteNameAdmin}/{RouteNameFileRepository}";
	public static string AdminTemplatesPageUrl = $"/{RouteNameAdmin}/{RouteNameTemplates}";
	public static string AdminTemplatesTypePageUrl = $"/{RouteNameAdmin}/{RouteNameTemplates}/{{0}}";
	public static string AdminTemplatesTypeWithSearchPageUrl = $"/{RouteNameAdmin}/{RouteNameTemplates}/{{0}}?{TfConstants.SearchQueryName}={{1}}";
	public static string AdminTemplatesTemplatePageUrl = $"/{RouteNameAdmin}/{RouteNameTemplates}/{{0}}/{{1}}";
	public static string BlobDownloadUrl = $"/fs/blob/{{0}}/{{1}}";

	public static string RouteNameSpace = "space";
	public static string RouteNameSpacePage = "page";
	public static string RouteNameSpaceView = "view";
	public static string RouteNameSpaceData = "data";
	public static string RouteNameManage = "manage";
	public static string RouteNameViews = "views";
	public static string PagesPageUrl = $"/{RouteNamePages}";
	public static string PagesSinglePageUrl = $"/{RouteNamePages}/{{0}}";
	public static string SpacePageUrl = $"/{RouteNameSpace}/{{0}}";
	public static string SpacePagePageUrl = $"/{RouteNameSpace}/{{0}}/{RouteNameSpacePage}/{{1}}";
	public static string SpaceManagePageUrl = $"/{RouteNameSpace}/{{0}}/{RouteNameManage}";
	public static string SpaceManagePagesPageUrl = $"/{RouteNameSpace}/{{0}}/{RouteNameManage}/{RouteNamePages}";
	public static string SpaceManageAccessPageUrl = $"/{RouteNameSpace}/{{0}}/{RouteNameManage}/{RouteNameAccess}";
	public static string SpaceViewPageUrl = $"/{RouteNameSpace}/{{0}}/{RouteNameSpaceView}/{{1}}";
	public static string SpaceViewPagesPageUrl = $"/{RouteNameSpace}/{{0}}/{RouteNameSpaceView}/{{1}}/{RouteNamePages}";
	public static string SpaceViewColumnsPageUrl = $"/{RouteNameSpace}/{{0}}/{RouteNameSpaceView}/{{1}}/{RouteNameColumns}";
	public static string SpaceViewFiltersPageUrl = $"/{RouteNameSpace}/{{0}}/{RouteNameSpaceView}/{{1}}/{RouteNameFilters}";
	public static string SpaceDataPageUrl = $"/{RouteNameSpace}/{{0}}/{RouteNameSpaceData}/{{1}}";
	public static string SpaceDataViewsPageUrl = $"/{RouteNameSpace}/{{0}}/{RouteNameSpaceData}/{{1}}/{RouteNameViews}";
	public static string SpaceDataConnectedDataPageUrl = $"/{RouteNameSpace}/{{0}}/{RouteNameSpaceData}/{{1}}/{RouteNameAux}";
	public static string SpaceDataDataPageUrl = $"/{RouteNameSpace}/{{0}}/{RouteNameSpaceData}/{{1}}/{RouteNameData}";
	public static string NoAccessPage = $"/403";
	public static string InstallPage = $"/tf-install";
	public static string InstallDetailsPage = $"/tf-install/{{0}}";

	public static List<string> SupportedUriFirstNodes = new List<string>() {
		String.Empty,RouteNameAdmin, RouteNamePages,RouteNameSpace
	};

}
