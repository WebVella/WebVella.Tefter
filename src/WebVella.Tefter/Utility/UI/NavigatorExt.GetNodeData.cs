namespace WebVella.Tefter.Utility;
using WebVella.Tefter.Models;
public static partial class NavigatorExt
{
	internal static TfNavigationState GetNodeData(this Uri uri, string navUri)
	{
		TfNavigationState result = new TfNavigationState()
		{
			Uri = navUri
		};
		var nodes = uri.LocalPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
		var dictIndex = 0;
		foreach (var item in nodes)
		{
			result.NodesDict[dictIndex] = item;
			dictIndex++;
		}

		result = result.Front_NodesProcess();
		result = result.Admin_NodesProcess();

		//Query
		var page = GetIntFromQuery(uri, TfConstants.PageQueryName, 1);
		var pageSize = GetIntFromQuery(uri, TfConstants.PageSizeQueryName, null);
		var search = GetStringFromQuery(uri, TfConstants.SearchQueryName, null);
		var searchAside = GetStringFromQuery(uri, TfConstants.AsideSearchQueryName, null);
		List<TfFilterBase> filters = null;
		var filtersString = GetStringFromQuery(uri, TfConstants.FiltersQueryName, null);
		if (!String.IsNullOrWhiteSpace(filtersString)) filters = DeserializeFiltersFromUrl(filtersString, true);
		List<TfSort> sorts = null;
		var sortString = GetStringFromQuery(uri, TfConstants.SortsQueryName, null);
		if (!String.IsNullOrWhiteSpace(sortString)) sorts = DeserializeSortsFromUrl(sortString, true);

		Guid? activeSaveId = GetGuidFromQuery(uri, TfConstants.ActiveSaveQueryName, null);
		bool searchInBookmarks = GetBooleanFromQuery(uri, TfConstants.SearchInBookmarksQueryName, true).Value;
		bool searchInSaves = GetBooleanFromQuery(uri, TfConstants.SearchInSavesQueryName, true).Value;
		bool searchInViews = GetBooleanFromQuery(uri, TfConstants.SearchInViewsQueryName, true).Value;

		Guid? spaceViewPresetId = GetGuidFromQuery(uri, TfConstants.PresetIdQueryName, null);

		result = result with
		{
			Page = page,
			PageSize = pageSize,
			Search = search,
			SearchAside = searchAside,
			Filters = filters,
			Sorts = sorts,
			ActiveSaveId = activeSaveId,
			SearchInBookmarks = searchInBookmarks,
			SearchInSaves = searchInSaves,
			SearchInViews = searchInViews,
			SpaceViewPresetId = spaceViewPresetId,
		};

		return result;
	}

	#region << Public >>
	private static TfNavigationState Front_NodesProcess(this TfNavigationState result)
	{
		if (result.NodesDict.Count == 0)
		{
			result = result.AddRouteNodes(RouteDataNode.Home);
			return result;
		}
		result = result.Front_Space_NodesProcess();
		result = result.Front_Pages_NodesProcess();
		return result;
	}

	#region << Space >>
	private static TfNavigationState Front_Space_NodesProcess(this TfNavigationState result)
	{
		if (result.NodesDict.Count < 1 || result.NodesDict[0] != TfConstants.RouteNameSpace)
			return result;

		result = result.AddRouteNodes(RouteDataNode.Space);
		result = result.Front_Space_SpaceId_NodesProcess();
		return result;
	}

	private static TfNavigationState Front_Space_SpaceId_NodesProcess(this TfNavigationState result)
	{
		if (result.RouteNodes.Count < 1
			|| result.RouteNodes[0] != RouteDataNode.Space)
			return result;

		if (result.NodesDict.Count < 2)
			return result;

		if (Guid.TryParse(result.NodesDict[1], out Guid outGuid))
		{
			result = result.AddRouteNodes(RouteDataNode.SpaceId);
			result = result with { SpaceId = outGuid };

			result = result.Front_Space_SpaceId_Manage_NodesProcess();
			result = result.Front_Space_SpaceId_Page_NodesProcess();
			result = result.Front_Space_SpaceId_SpaceData_NodesProcess();
			result = result.Front_Space_SpaceId_SpaceView_NodesProcess();

		}
		return result;
	}

	#region << Space Manage >>
	private static TfNavigationState Front_Space_SpaceId_Manage_NodesProcess(this TfNavigationState result)
	{
		if (result.RouteNodes.Count < 2
			|| result.RouteNodes[0] != RouteDataNode.Space
			|| result.RouteNodes[1] != RouteDataNode.SpaceId)
			return result;

		if (result.NodesDict.Count < 3)
			return result;

		if (result.NodesDict[2] == TfConstants.RouteNameManage)
		{
			result = result.AddRouteNodes(RouteDataNode.Manage);

			result = result.Front_Space_SpaceId_Manage_Pages_NodesProcess();
			result = result.Front_Space_SpaceId_Manage_Access_NodesProcess();
		}
		return result;
	}

	private static TfNavigationState Front_Space_SpaceId_Manage_Pages_NodesProcess(this TfNavigationState result)
	{
		if (result.RouteNodes.Count < 3
			|| result.RouteNodes[0] != RouteDataNode.Space
			|| result.RouteNodes[1] != RouteDataNode.SpaceId
			|| result.RouteNodes[2] != RouteDataNode.Manage)
			return result;

		if (result.NodesDict.Count < 4)
			return result;

		if (result.NodesDict[3] == TfConstants.RouteNamePages)
		{
			result = result.AddRouteNodes(RouteDataNode.Pages);
		}
		return result;
	}

	private static TfNavigationState Front_Space_SpaceId_Manage_Access_NodesProcess(this TfNavigationState result)
	{
		if (result.RouteNodes.Count < 3
			|| result.RouteNodes[0] != RouteDataNode.Space
			|| result.RouteNodes[1] != RouteDataNode.SpaceId
			|| result.RouteNodes[2] != RouteDataNode.Manage)
			return result;

		if (result.NodesDict.Count < 4)
			return result;

		if (result.NodesDict[3] == TfConstants.RouteNameAccess)
		{
			result = result.AddRouteNodes(RouteDataNode.Access);
		}
		return result;
	}
	#endregion

	#region << Space Page >>
	private static TfNavigationState Front_Space_SpaceId_Page_NodesProcess(this TfNavigationState result)
	{
		if (result.RouteNodes.Count < 2
			|| result.RouteNodes[0] != RouteDataNode.Space
			|| result.RouteNodes[1] != RouteDataNode.SpaceId)
			return result;

		if (result.NodesDict.Count < 3)
			return result;

		if (result.NodesDict[2] == TfConstants.RouteNameSpacePage)
		{
			result = result.AddRouteNodes(RouteDataNode.SpacePage);
			result = result.Front_Space_SpaceId_Page_PageId_NodesProcess();
		}
		return result;
	}

	private static TfNavigationState Front_Space_SpaceId_Page_PageId_NodesProcess(this TfNavigationState result)
	{
		if (result.RouteNodes.Count < 3
			|| result.RouteNodes[0] != RouteDataNode.Space
			|| result.RouteNodes[1] != RouteDataNode.SpaceId
			|| result.RouteNodes[2] != RouteDataNode.SpacePage)
			return result;

		if (result.NodesDict.Count < 4)
			return result;

		if (Guid.TryParse(result.NodesDict[3], out Guid outGuid))
		{
			result = result.AddRouteNodes(RouteDataNode.SpacePageId);
			result = result with { SpacePageId = outGuid };
		}
		return result;
	}
	#endregion

	#region << Space Data >>
	private static TfNavigationState Front_Space_SpaceId_SpaceData_NodesProcess(this TfNavigationState result)
	{
		if (result.RouteNodes.Count < 2
			|| result.RouteNodes[0] != RouteDataNode.Space
			|| result.RouteNodes[1] != RouteDataNode.SpaceId)
			return result;

		if (result.NodesDict.Count < 3)
			return result;

		if (result.NodesDict[2] == TfConstants.RouteNameSpaceData)
		{
			result = result.AddRouteNodes(RouteDataNode.SpaceData);
			result = result.Front_Space_SpaceId_SpaceData_SpaceDataId_NodesProcess();
		}
		return result;
	}

	private static TfNavigationState Front_Space_SpaceId_SpaceData_SpaceDataId_NodesProcess(this TfNavigationState result)
	{
		if (result.RouteNodes.Count < 3
			|| result.RouteNodes[0] != RouteDataNode.Space
			|| result.RouteNodes[1] != RouteDataNode.SpaceId
			|| result.RouteNodes[2] != RouteDataNode.SpaceData)
			return result;

		if (result.NodesDict.Count < 4)
			return result;

		if (Guid.TryParse(result.NodesDict[3], out Guid outGuid))
		{
			result = result.AddRouteNodes(RouteDataNode.SpaceDataId);
			result = result with { SpaceDataId = outGuid };
			result = result.Front_Space_SpaceId_SpaceData_SpaceDataId_Aux_NodesProcess();
			result = result.Front_Space_SpaceId_SpaceData_SpaceDataId_Views_NodesProcess();
			result = result.Front_Space_SpaceId_SpaceData_SpaceDataId_Data_NodesProcess();

		}
		return result;
	}

	private static TfNavigationState Front_Space_SpaceId_SpaceData_SpaceDataId_Aux_NodesProcess(this TfNavigationState result)
	{
		if (result.RouteNodes.Count < 4
			|| result.RouteNodes[0] != RouteDataNode.Space
			|| result.RouteNodes[1] != RouteDataNode.SpaceId
			|| result.RouteNodes[2] != RouteDataNode.SpaceData
			|| result.RouteNodes[3] != RouteDataNode.SpaceDataId)
			return result;

		if (result.NodesDict.Count < 5)
			return result;

		if (result.NodesDict[4] == TfConstants.RouteNameAux)
		{
			result = result.AddRouteNodes(RouteDataNode.Aux);
		}
		return result;
	}

	private static TfNavigationState Front_Space_SpaceId_SpaceData_SpaceDataId_Views_NodesProcess(this TfNavigationState result)
	{
		if (result.RouteNodes.Count < 4
			|| result.RouteNodes[0] != RouteDataNode.Space
			|| result.RouteNodes[1] != RouteDataNode.SpaceId
			|| result.RouteNodes[2] != RouteDataNode.SpaceData
			|| result.RouteNodes[3] != RouteDataNode.SpaceDataId)
			return result;

		if (result.NodesDict.Count < 5)
			return result;

		if (result.NodesDict[4] == TfConstants.RouteNameViews)
		{
			result = result.AddRouteNodes(RouteDataNode.Views);
		}
		return result;
	}
	private static TfNavigationState Front_Space_SpaceId_SpaceData_SpaceDataId_Data_NodesProcess(this TfNavigationState result)
	{
		if (result.RouteNodes.Count < 4
			|| result.RouteNodes[0] != RouteDataNode.Space
			|| result.RouteNodes[1] != RouteDataNode.SpaceId
			|| result.RouteNodes[2] != RouteDataNode.SpaceData
			|| result.RouteNodes[3] != RouteDataNode.SpaceDataId)
			return result;

		if (result.NodesDict.Count < 5)
			return result;

		if (result.NodesDict[4] == TfConstants.RouteNameData)
		{
			result = result.AddRouteNodes(RouteDataNode.Data);
		}
		return result;
	}
	#endregion

	#region << Space View >>
	private static TfNavigationState Front_Space_SpaceId_SpaceView_NodesProcess(this TfNavigationState result)
	{
		if (result.RouteNodes.Count < 2
			|| result.RouteNodes[0] != RouteDataNode.Space
			|| result.RouteNodes[1] != RouteDataNode.SpaceId)
			return result;

		if (result.NodesDict.Count < 3)
			return result;

		if (result.NodesDict[2] == TfConstants.RouteNameSpaceView)
		{
			result = result.AddRouteNodes(RouteDataNode.SpaceView);
			result = result.Front_Space_SpaceId_SpaceView_SpaceViewId_NodesProcess();
		}
		return result;
	}
	private static TfNavigationState Front_Space_SpaceId_SpaceView_SpaceViewId_NodesProcess(this TfNavigationState result)
	{
		if (result.RouteNodes.Count < 3
			|| result.RouteNodes[0] != RouteDataNode.Space
			|| result.RouteNodes[1] != RouteDataNode.SpaceId
			|| result.RouteNodes[2] != RouteDataNode.SpaceView)
			return result;

		if (result.NodesDict.Count < 4)
			return result;

		if (Guid.TryParse(result.NodesDict[3], out Guid outGuid))
		{
			result = result.AddRouteNodes(RouteDataNode.SpaceViewId);
			result = result with { SpaceViewId = outGuid };
			result = result.Front_Space_SpaceId_SpaceView_SpaceViewId_Pages_NodesProcess();

		}
		return result;
	}

	private static TfNavigationState Front_Space_SpaceId_SpaceView_SpaceViewId_Pages_NodesProcess(this TfNavigationState result)
	{
		if (result.RouteNodes.Count < 4
			|| result.RouteNodes[0] != RouteDataNode.Space
			|| result.RouteNodes[1] != RouteDataNode.SpaceId
			|| result.RouteNodes[2] != RouteDataNode.SpaceView
			|| result.RouteNodes[3] != RouteDataNode.SpaceViewId)
			return result;

		if (result.NodesDict.Count < 5)
			return result;

		if (result.NodesDict[4] == TfConstants.RouteNameSpacePages)
		{
			result = result.AddRouteNodes(RouteDataNode.Pages);
		}
		return result;
	}

	#endregion
	#endregion

	#region << Pages >>
	private static TfNavigationState Front_Pages_NodesProcess(this TfNavigationState result)
	{
		if (result.NodesDict.Count < 1 || result.NodesDict[0] != TfConstants.RouteNamePages)
			return result;

		result = result.AddRouteNodes(RouteDataNode.Pages);

		result = result.Front_Pages_PageId_NodesProcess();

		return result;
	}

	private static TfNavigationState Front_Pages_PageId_NodesProcess(this TfNavigationState result)
	{
		if (result.RouteNodes.Count < 1
			|| result.RouteNodes[0] != RouteDataNode.Pages)
			return result;

		if (result.NodesDict.Count < 2)
			return result;

		if (Guid.TryParse(result.NodesDict[1], out Guid outGuid))
		{
			result = result.AddRouteNodes(RouteDataNode.PageId);
			result = result with { PageId = outGuid };

		}
		return result;
	}
	#endregion

	#endregion

	#region << Admin >>

	private static TfNavigationState Admin_NodesProcess(this TfNavigationState result)
	{
		if (result.RouteNodes.Count > 0)
			return result;

		if (result.NodesDict.Count < 1 || result.NodesDict[0] != TfConstants.RouteNameAdmin)
			return result;

		result = result.AddRouteNodes(RouteDataNode.Admin);

		result = result.Admin_Users_NodesProcess();
		result = result.Admin_Roles_NodesProcess();
		result = result.Admin_DataProviders_NodesProcess();
		result = result.Admin_DataIdentities_NodesProcess();
		result = result.Admin_SharedColumns_NodesProcess();
		result = result.Admin_Pages_NodesProcess();
		result = result.Admin_Files_NodesProcess();
		result = result.Admin_Templates_NodesProcess();

		return result;
	}

	#region << Users >>
	private static TfNavigationState Admin_Users_NodesProcess(this TfNavigationState result)
	{
		if (result.RouteNodes.Count < 1 || result.RouteNodes[0] != RouteDataNode.Admin)
			return result;

		if (result.NodesDict.Count < 2 || result.NodesDict[1] != TfConstants.RouteNameUsers)
			return result;

		result = result.AddRouteNodes(RouteDataNode.Users);
		result = result.Admin_Users_UserId_NodesProcess();
		return result;
	}

	private static TfNavigationState Admin_Users_UserId_NodesProcess(this TfNavigationState result)
	{
		if (result.RouteNodes.Count < 2
			|| result.RouteNodes[0] != RouteDataNode.Admin
			|| result.RouteNodes[1] != RouteDataNode.Users)
			return result;

		if (result.NodesDict.Count < 3)
			return result;

		if (Guid.TryParse(result.NodesDict[2], out Guid outGuid))
		{
			result = result.AddRouteNodes(RouteDataNode.UserId);
			result = result with { UserId = outGuid };

			result = result.Admin_Users_UserId_Access_NodesProcess();
			result = result.Admin_Users_UserId_Saves_NodesProcess();
		}
		return result;
	}

	private static TfNavigationState Admin_Users_UserId_Access_NodesProcess(this TfNavigationState result)
	{
		if (result.RouteNodes.Count < 3
			|| result.RouteNodes[0] != RouteDataNode.Admin
			|| result.RouteNodes[1] != RouteDataNode.Users
			|| result.RouteNodes[2] != RouteDataNode.UserId)
			return result;

		if (result.NodesDict.Count < 4)
			return result;

		if (result.NodesDict[3] == TfConstants.RouteNameAccess)
		{
			result = result.AddRouteNodes(RouteDataNode.Access);
		}
		return result;
	}

	private static TfNavigationState Admin_Users_UserId_Saves_NodesProcess(this TfNavigationState result)
	{
		if (result.RouteNodes.Count < 3
			|| result.RouteNodes[0] != RouteDataNode.Admin
			|| result.RouteNodes[1] != RouteDataNode.Users
			|| result.RouteNodes[2] != RouteDataNode.UserId)
			return result;

		if (result.NodesDict.Count < 4)
			return result;

		if (result.NodesDict[3] == TfConstants.RouteNameSaves)
		{
			result = result.AddRouteNodes(RouteDataNode.Saves);
		}
		return result;
	}
	#endregion

	#region << Roles >>
	private static TfNavigationState Admin_Roles_NodesProcess(this TfNavigationState result)
	{
		if (result.RouteNodes.Count < 1 || result.RouteNodes[0] != RouteDataNode.Admin)
			return result;

		if (result.NodesDict.Count < 2 || result.NodesDict[1] != TfConstants.RouteNameRoles)
			return result;

		result = result.AddRouteNodes(RouteDataNode.Roles);
		result = result.Admin_Roles_RoleId_NodesProcess();
		return result;
	}

	private static TfNavigationState Admin_Roles_RoleId_NodesProcess(this TfNavigationState result)
	{
		if (result.RouteNodes.Count < 2
			|| result.RouteNodes[0] != RouteDataNode.Admin
			|| result.RouteNodes[1] != RouteDataNode.Roles)
			return result;

		if (result.NodesDict.Count < 3)
			return result;

		if (Guid.TryParse(result.NodesDict[2], out Guid outGuid))
		{
			result = result.AddRouteNodes(RouteDataNode.RoleId);
			result = result with { RoleId = outGuid };


		}
		return result;
	}

	#endregion

	#region << Data Providers >>
	private static TfNavigationState Admin_DataProviders_NodesProcess(this TfNavigationState result)
	{
		if (result.RouteNodes.Count < 1 || result.RouteNodes[0] != RouteDataNode.Admin)
			return result;

		if (result.NodesDict.Count < 2 || result.NodesDict[1] != TfConstants.RouteNameDataProviders)
			return result;

		result = result.AddRouteNodes(RouteDataNode.DataProviders);
		result = result.Admin_DataProviders_DataProviderId_NodesProcess();
		return result;
	}
	private static TfNavigationState Admin_DataProviders_DataProviderId_NodesProcess(this TfNavigationState result)
	{
		if (result.RouteNodes.Count < 2
			|| result.RouteNodes[0] != RouteDataNode.Admin
			|| result.RouteNodes[1] != RouteDataNode.DataProviders)
			return result;

		if (result.NodesDict.Count < 3)
			return result;

		if (Guid.TryParse(result.NodesDict[2], out Guid outGuid))
		{
			result = result.AddRouteNodes(RouteDataNode.DataProviderId);
			result = result with { DataProviderId = outGuid };
			result = result.Admin_DataProviders_DataProviderId_Schema_NodesProcess();
			result = result.Admin_DataProviders_DataProviderId_Aux_NodesProcess();
			result = result.Admin_DataProviders_DataProviderId_Synchronization_NodesProcess();
			result = result.Admin_DataProviders_DataProviderId_Data_NodesProcess();

		}
		return result;
	}
	private static TfNavigationState Admin_DataProviders_DataProviderId_Schema_NodesProcess(this TfNavigationState result)
	{
		if (result.RouteNodes.Count < 3
			|| result.RouteNodes[0] != RouteDataNode.Admin
			|| result.RouteNodes[1] != RouteDataNode.DataProviders
			|| result.RouteNodes[2] != RouteDataNode.DataProviderId)
			return result;

		if (result.NodesDict.Count < 4)
			return result;

		if (result.NodesDict[3] == TfConstants.RouteNameSchema)
		{
			result = result.AddRouteNodes(RouteDataNode.Schema);
		}
		return result;
	}
	private static TfNavigationState Admin_DataProviders_DataProviderId_Aux_NodesProcess(this TfNavigationState result)
	{
		if (result.RouteNodes.Count < 3
			|| result.RouteNodes[0] != RouteDataNode.Admin
			|| result.RouteNodes[1] != RouteDataNode.DataProviders
			|| result.RouteNodes[2] != RouteDataNode.DataProviderId)
			return result;

		if (result.NodesDict.Count < 4)
			return result;

		if (result.NodesDict[3] == TfConstants.RouteNameAux)
		{
			result = result.AddRouteNodes(RouteDataNode.Aux);
		}
		return result;
	}
	private static TfNavigationState Admin_DataProviders_DataProviderId_Synchronization_NodesProcess(this TfNavigationState result)
	{
		if (result.RouteNodes.Count < 3
			|| result.RouteNodes[0] != RouteDataNode.Admin
			|| result.RouteNodes[1] != RouteDataNode.DataProviders
			|| result.RouteNodes[2] != RouteDataNode.DataProviderId)
			return result;

		if (result.NodesDict.Count < 4)
			return result;

		if (result.NodesDict[3] == TfConstants.RouteNameSynchronization)
		{
			result = result.AddRouteNodes(RouteDataNode.Synchronization);
		}
		return result;
	}
	private static TfNavigationState Admin_DataProviders_DataProviderId_Data_NodesProcess(this TfNavigationState result)
	{
		if (result.RouteNodes.Count < 3
			|| result.RouteNodes[0] != RouteDataNode.Admin
			|| result.RouteNodes[1] != RouteDataNode.DataProviders
			|| result.RouteNodes[2] != RouteDataNode.DataProviderId)
			return result;

		if (result.NodesDict.Count < 4)
			return result;

		if (result.NodesDict[3] == TfConstants.RouteNameData)
		{
			result = result.AddRouteNodes(RouteDataNode.Data);
		}
		return result;
	}
	#endregion

	#region << Data Identities >>
	private static TfNavigationState Admin_DataIdentities_NodesProcess(this TfNavigationState result)
	{
		if (result.RouteNodes.Count < 1 || result.RouteNodes[0] != RouteDataNode.Admin)
			return result;

		if (result.NodesDict.Count < 2 || result.NodesDict[1] != TfConstants.RouteNameDataIdentities)
			return result;

		result = result.AddRouteNodes(RouteDataNode.DataIdentities);
		result = result.Admin_DataIdentities_DataIdentityId_NodesProcess();
		return result;
	}
	private static TfNavigationState Admin_DataIdentities_DataIdentityId_NodesProcess(this TfNavigationState result)
	{
		if (result.RouteNodes.Count < 2
			|| result.RouteNodes[0] != RouteDataNode.Admin
			|| result.RouteNodes[1] != RouteDataNode.DataIdentities)
			return result;

		if (result.NodesDict.Count < 3)
			return result;

		result = result.AddRouteNodes(RouteDataNode.DataIdentityId);
		result = result with { DataIdentityId = result.NodesDict[2] };

		return result;
	}
	#endregion

	#region << Shared Columns >>
	private static TfNavigationState Admin_SharedColumns_NodesProcess(this TfNavigationState result)
	{
		if (result.RouteNodes.Count < 1 || result.RouteNodes[0] != RouteDataNode.Admin)
			return result;

		if (result.NodesDict.Count < 2 || result.NodesDict[1] != TfConstants.RouteNameSharedColumns)
			return result;

		result = result.AddRouteNodes(RouteDataNode.SharedColumns);
		result = result.Admin_SharedColumns_SharedColumnId_NodesProcess();
		return result;
	}
	private static TfNavigationState Admin_SharedColumns_SharedColumnId_NodesProcess(this TfNavigationState result)
	{
		if (result.RouteNodes.Count < 2
			|| result.RouteNodes[0] != RouteDataNode.Admin
			|| result.RouteNodes[1] != RouteDataNode.SharedColumns)
			return result;

		if (result.NodesDict.Count < 3)
			return result;
		if (Guid.TryParse(result.NodesDict[2], out Guid outGuid))
		{
			result = result.AddRouteNodes(RouteDataNode.SharedColumnId);
			result = result with { SharedColumnId = outGuid };

		}
		return result;
	}
	#endregion

	#region << Pages >>
	private static TfNavigationState Admin_Pages_NodesProcess(this TfNavigationState result)
	{
		if (result.RouteNodes.Count < 1 || result.RouteNodes[0] != RouteDataNode.Admin)
			return result;

		if (result.NodesDict.Count < 2 || result.NodesDict[1] != TfConstants.RouteNamePages)
			return result;

		result = result.AddRouteNodes(RouteDataNode.Pages);

		result = result.Admin_Page_PageId_NodesProcess();

		return result;
	}
	private static TfNavigationState Admin_Page_PageId_NodesProcess(this TfNavigationState result)
	{
		if (result.RouteNodes.Count < 2
			|| result.RouteNodes[0] != RouteDataNode.Admin
			|| result.RouteNodes[1] != RouteDataNode.Pages)
			return result;

		if (result.NodesDict.Count < 3)
			return result;
		if (Guid.TryParse(result.NodesDict[2], out Guid outGuid))
		{
			result = result.AddRouteNodes(RouteDataNode.PageId);
			result = result with { PageId = outGuid };

		}
		return result;
	}
	#endregion

	#region << Files >>
	private static TfNavigationState Admin_Files_NodesProcess(this TfNavigationState result)
	{
		if (result.RouteNodes.Count < 1 || result.RouteNodes[0] != RouteDataNode.Admin)
			return result;

		if (result.NodesDict.Count < 2 || result.NodesDict[1] != TfConstants.RouteNameFileRepository)
			return result;

		result = result.AddRouteNodes(RouteDataNode.FileRepository);

		return result;
	}
	#endregion

	#region << Templates >>
	private static TfNavigationState Admin_Templates_NodesProcess(this TfNavigationState result)
	{
		if (result.RouteNodes.Count < 1 || result.RouteNodes[0] != RouteDataNode.Admin)
			return result;

		if (result.NodesDict.Count < 2 || result.NodesDict[1] != TfConstants.RouteNameTemplates)
			return result;

		result = result.AddRouteNodes(RouteDataNode.Templates);
		result = result.Admin_Templates_TemplateTypeId_NodesProcess();
		return result;
	}

	private static TfNavigationState Admin_Templates_TemplateTypeId_NodesProcess(this TfNavigationState result)
	{
		if (result.RouteNodes.Count < 2
			|| result.RouteNodes[0] != RouteDataNode.Admin
			|| result.RouteNodes[1] != RouteDataNode.Templates)
			return result;

		if (result.NodesDict.Count < 3)
			return result;

		if (Enum.TryParse<TfTemplateResultType>(result.NodesDict[2], out TfTemplateResultType outEnum))
		{
			result = result.AddRouteNodes(RouteDataNode.TemplateTypeId);
			result = result with { TemplateResultType = outEnum };
			result = result.Admin_Templates_TemplateTypeId_TemplateId_NodesProcess();
		}
		return result;
	}

	private static TfNavigationState Admin_Templates_TemplateTypeId_TemplateId_NodesProcess(this TfNavigationState result)
	{
		if (result.RouteNodes.Count < 3
			|| result.RouteNodes[0] != RouteDataNode.Admin
			|| result.RouteNodes[1] != RouteDataNode.Templates
			|| result.RouteNodes[2] != RouteDataNode.TemplateTypeId)
			return result;

		if (result.NodesDict.Count < 4)
			return result;

		if (Guid.TryParse(result.NodesDict[3], out Guid outGuid))
		{
			result = result.AddRouteNodes(RouteDataNode.TemplateId);
			result = result with { TemplateId = outGuid };
		}
		return result;
	}

	#endregion

	#endregion
}
