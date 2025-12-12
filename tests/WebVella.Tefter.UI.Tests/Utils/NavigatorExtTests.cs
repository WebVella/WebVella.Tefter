using System;

namespace WebVella.Tefter.UI.Tests.Utils;

public class NavigatorExtTests
{
    [Fact]
    public void GetNodeDataTests()
    {
        //Given
        var baseUrl = "http://localhost";
        Uri uri = new Uri(baseUrl);
        TfNavigationState result = new();
        Guid spaceId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        Guid roleId = Guid.NewGuid();
        Guid providerId = Guid.NewGuid();
        Guid sharedColumnId = Guid.NewGuid();
        string identityId = Guid.NewGuid().ToString();
        Guid viewId = Guid.NewGuid();
        Guid dataId = Guid.NewGuid();
        Guid pageId = Guid.NewGuid();
        Guid templateId = Guid.NewGuid();
        string manageOption = "addon-manage-option";
        //When
        uri = new Uri($"{baseUrl}");
        result = NavigatorExt.GetNodeData(uri);
        result.NodesDict.Should().NotBeNull();
        result.NodesDict.Count.Should().Be(0);
        result.RouteNodes.Count.Should().Be(1);
        result.RouteNodes[0].Should().Be(RouteDataNode.Home);

        #region << NoSpace >>
        uri = new Uri($"{baseUrl}{string.Format(TfConstants.HomeNoSpaceUrl)}");
        result = NavigatorExt.GetNodeData(uri);
        result.NodesDict.Should().NotBeNull();
        result.NodesDict.Count.Should().Be(1);
        result.RouteNodes.Count.Should().Be(1);
        result.RouteNodes[0].Should().Be(RouteDataNode.NoSpace);
        #endregion

        #region << Home page >>
        uri = new Uri($"{baseUrl}{string.Format(TfConstants.HomePagesSingleUrl, pageId)}");
        result = NavigatorExt.GetNodeData(uri);
        result.NodesDict.Should().NotBeNull();
        result.NodesDict.Count.Should().Be(3);
        result.RouteNodes[0].Should().Be(RouteDataNode.Home);
        result.RouteNodes[1].Should().Be(RouteDataNode.Pages);
        result.RouteNodes[2].Should().Be(RouteDataNode.PageId);
        result.PageId.Should().Be(pageId);


        #endregion

        #region << Admin Users >>
        uri = new Uri($"{baseUrl}{string.Format(TfConstants.AdminUsersPageUrl)}");
        result = NavigatorExt.GetNodeData(uri);
        result.NodesDict.Should().NotBeNull();
        result.NodesDict.Count.Should().Be(2);
        result.RouteNodes.Count.Should().Be(2);
        result.RouteNodes[0].Should().Be(RouteDataNode.Admin);
        result.RouteNodes[1].Should().Be(RouteDataNode.Users);

        uri = new Uri($"{baseUrl}{string.Format(TfConstants.AdminRolesPageUrl)}");
        result = NavigatorExt.GetNodeData(uri);
        result.NodesDict.Should().NotBeNull();
        result.NodesDict.Count.Should().Be(2);
        result.RouteNodes.Count.Should().Be(2);
        result.RouteNodes[0].Should().Be(RouteDataNode.Admin);
        result.RouteNodes[1].Should().Be(RouteDataNode.Roles);

        uri = new Uri($"{baseUrl}{string.Format(TfConstants.AdminUserDetailsPageUrl, userId)}");
        result = NavigatorExt.GetNodeData(uri);
        result.NodesDict.Should().NotBeNull();
        result.NodesDict.Count.Should().Be(3);
        result.RouteNodes.Count.Should().Be(3);
        result.RouteNodes[0].Should().Be(RouteDataNode.Admin);
        result.RouteNodes[1].Should().Be(RouteDataNode.Users);
        result.RouteNodes[2].Should().Be(RouteDataNode.UserId);
        result.UserId.Should().Be(userId);

        uri = new Uri($"{baseUrl}{string.Format(TfConstants.AdminRoleDetailsPageUrl, roleId)}");
        result = NavigatorExt.GetNodeData(uri);
        result.NodesDict.Should().NotBeNull();
        result.NodesDict.Count.Should().Be(3);
        result.RouteNodes.Count.Should().Be(3);
        result.RouteNodes[0].Should().Be(RouteDataNode.Admin);
        result.RouteNodes[1].Should().Be(RouteDataNode.Roles);
        result.RouteNodes[2].Should().Be(RouteDataNode.RoleId);
        result.RoleId.Should().Be(roleId);

        uri = new Uri($"{baseUrl}{string.Format(TfConstants.AdminUserAccessPageUrl, userId)}");
        result = NavigatorExt.GetNodeData(uri);
        result.NodesDict.Should().NotBeNull();
        result.NodesDict.Count.Should().Be(4);
        result.RouteNodes.Count.Should().Be(4);
        result.RouteNodes[0].Should().Be(RouteDataNode.Admin);
        result.RouteNodes[1].Should().Be(RouteDataNode.Users);
        result.RouteNodes[2].Should().Be(RouteDataNode.UserId);
        result.RouteNodes[3].Should().Be(RouteDataNode.Access);
        result.UserId.Should().Be(userId);

        uri = new Uri($"{baseUrl}{string.Format(TfConstants.AdminUserSavesViewsPageUrl, userId)}");
        result = NavigatorExt.GetNodeData(uri);
        result.NodesDict.Should().NotBeNull();
        result.RouteNodes.Count.Should().Be(4);
        result.RouteNodes[0].Should().Be(RouteDataNode.Admin);
        result.RouteNodes[1].Should().Be(RouteDataNode.Users);
        result.RouteNodes[2].Should().Be(RouteDataNode.UserId);
        result.RouteNodes[3].Should().Be(RouteDataNode.Saves);
        result.UserId.Should().Be(userId);
        #endregion

        #region << Admin Data providers >>
        uri = new Uri($"{baseUrl}{string.Format(TfConstants.AdminDataProvidersPageUrl)}");
        result = NavigatorExt.GetNodeData(uri);
        result.NodesDict.Should().NotBeNull();
        result.NodesDict.Count.Should().Be(2);
        result.RouteNodes.Count.Should().Be(2);
        result.RouteNodes[0].Should().Be(RouteDataNode.Admin);
        result.RouteNodes[1].Should().Be(RouteDataNode.DataProviders);

        uri = new Uri($"{baseUrl}{string.Format(TfConstants.AdminDataProviderDetailsPageUrl, providerId)}");
        result = NavigatorExt.GetNodeData(uri);
        result.NodesDict.Should().NotBeNull();
        result.NodesDict.Count.Should().Be(3);
        result.RouteNodes.Count.Should().Be(3);
        result.RouteNodes[0].Should().Be(RouteDataNode.Admin);
        result.RouteNodes[1].Should().Be(RouteDataNode.DataProviders);
        result.RouteNodes[2].Should().Be(RouteDataNode.DataProviderId);
        result.DataProviderId.Should().Be(providerId);

        uri = new Uri($"{baseUrl}{string.Format(TfConstants.AdminDataIdentitiesPageUrl)}");
        result = NavigatorExt.GetNodeData(uri);
        result.NodesDict.Should().NotBeNull();
        result.NodesDict.Count.Should().Be(2);
        result.RouteNodes.Count.Should().Be(2);
        result.RouteNodes[0].Should().Be(RouteDataNode.Admin);
        result.RouteNodes[1].Should().Be(RouteDataNode.DataIdentities);

        uri = new Uri($"{baseUrl}{string.Format(TfConstants.AdminDataIdentityDetailsPageUrl, identityId)}");
        result = NavigatorExt.GetNodeData(uri);
        result.NodesDict.Should().NotBeNull();
        result.NodesDict.Count.Should().Be(3);
        result.RouteNodes.Count.Should().Be(3);
        result.RouteNodes[0].Should().Be(RouteDataNode.Admin);
        result.RouteNodes[1].Should().Be(RouteDataNode.DataIdentities);
        result.RouteNodes[2].Should().Be(RouteDataNode.DataIdentityId);
        result.DataIdentityId.Should().Be(identityId);

        uri = new Uri($"{baseUrl}{string.Format(TfConstants.AdminDataProviderSchemaPageUrl, providerId)}");
        result = NavigatorExt.GetNodeData(uri);
        result.NodesDict.Should().NotBeNull();
        result.NodesDict.Count.Should().Be(4);
        result.RouteNodes.Count.Should().Be(4);
        result.RouteNodes[0].Should().Be(RouteDataNode.Admin);
        result.RouteNodes[1].Should().Be(RouteDataNode.DataProviders);
        result.RouteNodes[2].Should().Be(RouteDataNode.DataProviderId);
        result.RouteNodes[3].Should().Be(RouteDataNode.Schema);
        result.DataProviderId.Should().Be(providerId);

        uri = new Uri($"{baseUrl}{string.Format(TfConstants.AdminDataProviderAuxPageUrl, providerId)}");
        result = NavigatorExt.GetNodeData(uri);
        result.NodesDict.Should().NotBeNull();
        result.NodesDict.Count.Should().Be(4);
        result.RouteNodes.Count.Should().Be(4);
        result.RouteNodes[0].Should().Be(RouteDataNode.Admin);
        result.RouteNodes[1].Should().Be(RouteDataNode.DataProviders);
        result.RouteNodes[2].Should().Be(RouteDataNode.DataProviderId);
        result.RouteNodes[3].Should().Be(RouteDataNode.Aux);
        result.DataProviderId.Should().Be(providerId);

        uri = new Uri($"{baseUrl}{string.Format(TfConstants.AdminDataProviderSynchronizationPageUrl, providerId)}");
        result = NavigatorExt.GetNodeData(uri);
        result.NodesDict.Should().NotBeNull();
        result.NodesDict.Count.Should().Be(4);
        result.RouteNodes.Count.Should().Be(4);
        result.RouteNodes[0].Should().Be(RouteDataNode.Admin);
        result.RouteNodes[1].Should().Be(RouteDataNode.DataProviders);
        result.RouteNodes[2].Should().Be(RouteDataNode.DataProviderId);
        result.RouteNodes[3].Should().Be(RouteDataNode.Synchronization);
        result.DataProviderId.Should().Be(providerId);

        uri = new Uri($"{baseUrl}{string.Format(TfConstants.AdminDataProviderDataPageUrl, providerId)}");
        result = NavigatorExt.GetNodeData(uri);
        result.NodesDict.Should().NotBeNull();
        result.NodesDict.Count.Should().Be(4);
        result.RouteNodes.Count.Should().Be(4);
        result.RouteNodes[0].Should().Be(RouteDataNode.Admin);
        result.RouteNodes[1].Should().Be(RouteDataNode.DataProviders);
        result.RouteNodes[2].Should().Be(RouteDataNode.DataProviderId);
        result.RouteNodes[3].Should().Be(RouteDataNode.Data);
        result.DataProviderId.Should().Be(providerId);
        #endregion

        #region << Admin SharedColumns >>
        uri = new Uri($"{baseUrl}{string.Format(TfConstants.AdminSharedColumnsPageUrl)}");
        result = NavigatorExt.GetNodeData(uri);
        result.NodesDict.Should().NotBeNull();
        result.NodesDict.Count.Should().Be(2);
        result.RouteNodes.Count.Should().Be(2);
        result.RouteNodes[0].Should().Be(RouteDataNode.Admin);
        result.RouteNodes[1].Should().Be(RouteDataNode.SharedColumns);

        uri = new Uri($"{baseUrl}{string.Format(TfConstants.AdminSharedColumnDetailsPageUrl, sharedColumnId)}");
        result = NavigatorExt.GetNodeData(uri);
        result.NodesDict.Should().NotBeNull();
        result.NodesDict.Count.Should().Be(3);
        result.RouteNodes.Count.Should().Be(3);
        result.RouteNodes[0].Should().Be(RouteDataNode.Admin);
        result.RouteNodes[1].Should().Be(RouteDataNode.SharedColumns);
        result.RouteNodes[2].Should().Be(RouteDataNode.SharedColumnId);
        result.SharedColumnId.Should().Be(sharedColumnId);
        #endregion

        #region << Admin FileRepository >>
        uri = new Uri($"{baseUrl}{string.Format(TfConstants.AdminFileRepositoryPageUrl)}");
        result = NavigatorExt.GetNodeData(uri);
        result.NodesDict.Should().NotBeNull();
        result.NodesDict.Count.Should().Be(2);
        result.RouteNodes.Count.Should().Be(2);
        result.RouteNodes[0].Should().Be(RouteDataNode.Admin);
        result.RouteNodes[1].Should().Be(RouteDataNode.FileRepository);
        #endregion

        #region << Admin Templates >>
        uri = new Uri($"{baseUrl}{string.Format(TfConstants.AdminTemplatesPageUrl)}");
        result = NavigatorExt.GetNodeData(uri);
        result.NodesDict.Should().NotBeNull();
        result.NodesDict.Count.Should().Be(2);
        result.RouteNodes.Count.Should().Be(2);
        result.RouteNodes[0].Should().Be(RouteDataNode.Admin);
        result.RouteNodes[1].Should().Be(RouteDataNode.Templates);

        uri = new Uri($"{baseUrl}{string.Format(TfConstants.AdminTemplateDetailsPageUrl, templateId)}");
        result = NavigatorExt.GetNodeData(uri);
        result.NodesDict.Should().NotBeNull();
        result.NodesDict.Count.Should().Be(3);
        result.RouteNodes.Count.Should().Be(3);
        result.RouteNodes[0].Should().Be(RouteDataNode.Admin);
        result.RouteNodes[1].Should().Be(RouteDataNode.Templates);
        result.RouteNodes[2].Should().Be(RouteDataNode.TemplateId);
        result.TemplateId.Should().Be(templateId);
        #endregion

        #region << Space >>
        uri = new Uri($"{baseUrl}{string.Format(TfConstants.SpaceManagePageUrl, spaceId)}");
        result = NavigatorExt.GetNodeData(uri);
        result.NodesDict.Should().NotBeNull();
        result.NodesDict.Count.Should().Be(3);
        result.RouteNodes[0].Should().Be(RouteDataNode.Space);
        result.RouteNodes[1].Should().Be(RouteDataNode.SpaceId);
        result.RouteNodes[2].Should().Be(RouteDataNode.Manage);
        result.SpaceId.Should().Be(spaceId);
        uri = new Uri($"{baseUrl}{string.Format(TfConstants.SpaceManagePagesPageUrl, spaceId)}");
        result = NavigatorExt.GetNodeData(uri);
        result.NodesDict.Should().NotBeNull();
        result.NodesDict.Count.Should().Be(4);
        result.RouteNodes[0].Should().Be(RouteDataNode.Space);
        result.RouteNodes[1].Should().Be(RouteDataNode.SpaceId);
        result.RouteNodes[2].Should().Be(RouteDataNode.Manage);
        result.RouteNodes[3].Should().Be(RouteDataNode.Pages);
        result.SpaceId.Should().Be(spaceId);
        uri = new Uri($"{baseUrl}{string.Format(TfConstants.SpaceManageAccessPageUrl, spaceId)}");
        result = NavigatorExt.GetNodeData(uri);
        result.NodesDict.Should().NotBeNull();
        result.NodesDict.Count.Should().Be(4);
        result.RouteNodes[0].Should().Be(RouteDataNode.Space);
        result.RouteNodes[1].Should().Be(RouteDataNode.SpaceId);
        result.RouteNodes[2].Should().Be(RouteDataNode.Manage);
        result.RouteNodes[3].Should().Be(RouteDataNode.Access);
        result.SpaceId.Should().Be(spaceId);
        #endregion

        #region << Space page >>
        uri = new Uri($"{baseUrl}{string.Format(TfConstants.SpacePagePageUrl, spaceId, pageId)}");
        result = NavigatorExt.GetNodeData(uri);
        result.NodesDict.Should().NotBeNull();
        result.NodesDict.Count.Should().Be(4);
        result.RouteNodes[0].Should().Be(RouteDataNode.Space);
        result.RouteNodes[1].Should().Be(RouteDataNode.SpaceId);
        result.RouteNodes[2].Should().Be(RouteDataNode.SpacePage);
        result.RouteNodes[3].Should().Be(RouteDataNode.SpacePageId);
        result.SpaceId.Should().Be(spaceId);
        result.SpacePageId.Should().Be(pageId);

        uri = new Uri($"{baseUrl}{string.Format(TfConstants.SpacePagePageManageUrl, spaceId, pageId)}");
        result = NavigatorExt.GetNodeData(uri);
        result.NodesDict.Should().NotBeNull();
        result.NodesDict.Count.Should().Be(5);
        result.RouteNodes[0].Should().Be(RouteDataNode.Space);
        result.RouteNodes[1].Should().Be(RouteDataNode.SpaceId);
        result.RouteNodes[2].Should().Be(RouteDataNode.SpacePage);
        result.RouteNodes[3].Should().Be(RouteDataNode.SpacePageId);
        result.RouteNodes[4].Should().Be(RouteDataNode.Manage);
        result.SpaceId.Should().Be(spaceId);
        result.SpacePageId.Should().Be(pageId);

        uri = new Uri($"{baseUrl}{string.Format(TfConstants.SpacePagePageManageTabUrl, spaceId, pageId, manageOption)}");
        result = NavigatorExt.GetNodeData(uri);
        result.NodesDict.Should().NotBeNull();
        result.NodesDict.Count.Should().Be(5);
        result.RouteNodes[0].Should().Be(RouteDataNode.Space);
        result.RouteNodes[1].Should().Be(RouteDataNode.SpaceId);
        result.RouteNodes[2].Should().Be(RouteDataNode.SpacePage);
        result.RouteNodes[3].Should().Be(RouteDataNode.SpacePageId);
        result.RouteNodes[4].Should().Be(RouteDataNode.ManageTab);
        result.SpaceId.Should().Be(spaceId);
        result.SpacePageId.Should().Be(pageId);
        result.ManageTab.Should().Be(manageOption);

        #endregion
    }

    [Fact]
    public void GetQueryDataTests()
    {
        //Given
        var baseUrl = "http://localhost";
        Uri uri = new Uri(baseUrl);
        TfNavigationState result = new();

        int page = 3;
        int pageSize = 12;
        string search = "$~@32~/";

        var filterBoolColumnName = Guid.NewGuid().ToString().Split("-", StringSplitOptions.RemoveEmptyEntries)[0];
        var filterBoolMethod = TfFilterBooleanComparisonMethod.NotEqual;
        bool filterBoolValue = false;
        var filterDateOnlyColumnName = Guid.NewGuid().ToString().Split("-", StringSplitOptions.RemoveEmptyEntries)[0];
        //var filterDateOnlyMethod = TucFilterDateTimeComparisonMethod.Greater;
        DateOnly? filterDateOnlyValue = DateOnly.FromDateTime(DateTime.Now);
        var filterTextColumnName = Guid.NewGuid().ToString().Split("-", StringSplitOptions.RemoveEmptyEntries)[0];
        var filterTextMethod = TfFilterTextComparisonMethod.Contains;
        var filterTextValue = "//$~(&&)";
        var filterBool = new TfFilterBoolean
        {
            ColumnName = filterBoolColumnName,
            ComparisonMethod = filterBoolMethod,
            Value = filterBoolValue.ToString(),
        };
        var filterText = new TfFilterText
        {
            ColumnName = filterTextColumnName,
            ComparisonMethod = filterTextMethod,
            Value = filterTextValue,
        };
        var filterOr = new TfFilterOr(filterBool, filterText)
        {
            Id = Guid.NewGuid(),
        };
        var filterAnd = new TfFilterAnd(filterOr);
        var filters = new List<TfFilterQuery> { filterAnd.ToQuery() };

        var sortColumnName1 = Guid.NewGuid().ToString().Split("-", StringSplitOptions.RemoveEmptyEntries)[0];
        var sortColumnOrder1 = TfSortDirection.ASC;
        var sortColumnName2 = Guid.NewGuid().ToString().Split("-", StringSplitOptions.RemoveEmptyEntries)[0];
        var sortColumnOrder2 = TfSortDirection.DESC;
        var sort1 = new TfSort
        {
            ColumnName = sortColumnName1,
            Direction = sortColumnOrder1,
        };
        var sort2 = new TfSort
        {
            ColumnName = sortColumnName2,
            Direction = sortColumnOrder2,
        };
        var sorts = new List<TfSortQuery>() { sort1.ToQuery(), sort2.ToQuery() };
        //When
        uri = new Uri($"{baseUrl}?{TfConstants.PageQueryName}={NavigatorExt.ProcessQueryValueForUrl(page.ToString())}" +
        $"&{TfConstants.PageSizeQueryName}={NavigatorExt.ProcessQueryValueForUrl(pageSize.ToString())}" +
        $"&{TfConstants.SearchQueryName}={NavigatorExt.ProcessQueryValueForUrl(search)}" +
        $"&{TfConstants.FiltersQueryName}={NavigatorExt.SerializeFiltersForUrl(filters)}" +
        $"&{TfConstants.SortsQueryName}={NavigatorExt.SerializeSortsForUrl(sorts)}");
        result = NavigatorExt.GetNodeData(uri);
        #region << page >>
        {
            result.Page.Should().Be(page);
        }
        #endregion
        #region << pageSize >>
        {
            result.PageSize.Should().Be(pageSize);
        }
        #endregion
        #region << pageSize >>
        {
            result.Search.Should().Be(search);
        }
        #endregion
        #region << filters >>
        {
            result.Filters.Should().NotBeNull();
            result.Filters.Count.Should().Be(1);
            var lvl1 = result.Filters[0];
            lvl1.Should().BeOfType<TfFilterQuery>();
            var lvl1Filter = (TfFilterQuery)lvl1;
            lvl1Filter.QueryName.Should().Be(filterAnd.ToQuery().QueryName);
            lvl1Filter.Items.Should().NotBeNull();
            lvl1Filter.Items.Count.Should().Be(1);
            var lvl2 = lvl1Filter.Items[0];
            lvl2.Should().BeOfType<TfFilterQuery>();
            var lvl2Filter = (TfFilterQuery)lvl2;
            lvl2Filter.QueryName.Should().Be(filterOr.ToQuery().QueryName);
            lvl2Filter.Items.Should().NotBeNull();
            lvl2Filter.Items.Count.Should().Be(2);

            var lvl31 = lvl2Filter.Items[0];
            lvl31.Should().BeOfType<TfFilterQuery>();
            var lvl31Filter = (TfFilterQuery)lvl31;
            lvl31Filter.QueryName.Should().Be(filterBoolColumnName);
            lvl31Filter.Method.Should().Be((int)filterBoolMethod);
            lvl31Filter.Value.Should().Be(filterBoolValue.ToString());

            var lvl33 = lvl2Filter.Items[1];
            lvl33.Should().BeOfType<TfFilterQuery>();
            var lvl33Filter = (TfFilterQuery)lvl33;
            lvl33Filter.QueryName.Should().Be(filterTextColumnName);
            lvl33Filter.Method.Should().Be((int)filterTextMethod);
            lvl33Filter.Value.Should().Be(filterTextValue);

        }
        #endregion
        #region << sorts >>
        {
            result.Sorts.Should().NotBeNull();
            result.Sorts.Count.Should().Be(2);
            result.Sorts[0].Name.Should().Be(sortColumnName1);
            result.Sorts[0].Direction.Should().Be((int)sortColumnOrder1);
            result.Sorts[1].Name.Should().Be(sortColumnName2);
            result.Sorts[1].Direction.Should().Be((int)sortColumnOrder2);

        }
        #endregion
    }

    [Fact]
    public void ProcessForTitleTests()
    {
        string? input = null;
        string? result = null;

        result = NavigatorExt.ProcessForTitle(input);
        result.Should().BeNull();

        input = "test.dp4_column";
        result = NavigatorExt.ProcessForTitle(input);
        result.Should().Be("column");

        input = "dp4_column_1";
        result = NavigatorExt.ProcessForTitle(input);
        result.Should().Be("column 1");

        input = "test.dp4_column_1";
        result = NavigatorExt.ProcessForTitle(input);
        result.Should().Be("column 1");
    }


    // [Fact]
    // public void SpaceViewFilterUrlSerializationTests()
    // {
    //
    // 	var test = new List<TfFilterBase>();
    // 	string queryValue = null;
    // 	var result = new List<TfFilterBase>();
    // 	var columnId = Guid.NewGuid();
    // 	var columnName = Guid.NewGuid().ToString().Split("-", StringSplitOptions.RemoveEmptyEntries)[0];
    //
    // 	#region << TfFilterAnd >>
    // 	{
    // 		test = new List<TfFilterBase> {
    // 			new TfFilterAnd(){
    // 				Id = columnId,
    // 				//ColumnName = columnName,
    // 				Filters = new()
    // 			}
    // 		};
    // 		queryValue = NavigatorExt.SerializeFiltersForUrl(test);
    // 		result = NavigatorExt.DeserializeFiltersFromUrl(queryValue);
    // 		result.Should().NotBeNull();
    // 		result.Count.Should().Be(1);
    // 		result[0].Should().BeOfType<TfFilterAnd>();
    // 		//result[0].ColumnName.Should().Be(columnName);
    // 		var casted = (TfFilterAnd)result[0];
    // 		casted.Filters.Should().NotBeNull();
    // 		casted.Filters.Count.Should().Be(0);
    // 	}
    // 	#endregion
    //
    // 	#region << TfFilterOr >>
    // 	{
    // 		test = new List<TfFilterBase> {
    // 			new TfFilterOr(){
    // 				Id = columnId,
    // 				//ColumnName = columnName,
    // 				Filters = new()
    // 			}
    // 		};
    // 		queryValue = NavigatorExt.SerializeFiltersForUrl(test);
    // 		result = NavigatorExt.DeserializeFiltersFromUrl(queryValue);
    // 		result.Should().NotBeNull();
    // 		result.Count.Should().Be(1);
    // 		result[0].Should().BeOfType<TfFilterOr>();
    // 		//result[0].ColumnName.Should().Be(columnName);
    // 		var casted = (TfFilterOr)result[0];
    // 		casted.Filters.Should().NotBeNull();
    // 		casted.Filters.Count.Should().Be(0);
    // 	}
    // 	#endregion
    //
    // 	#region << TucFilterBoolean >>
    // 	{
    // 		var method = TucFilterBooleanComparisonMethod.NotEqual;
    // 		bool? value = false;
    // 		test = new List<TfFilterBase> {
    // 			new TucFilterBoolean(){
    // 				Id = columnId,
    // 				ColumnName = columnName,
    // 				ComparisonMethod = method,
    // 				Value = value.ToString()
    // 			}
    // 		};
    // 		queryValue = NavigatorExt.SerializeFiltersForUrl(test);
    // 		result = NavigatorExt.DeserializeFiltersFromUrl(queryValue);
    // 		result.Should().NotBeNull();
    // 		result.Count.Should().Be(1);
    // 		result[0].Should().BeOfType<TucFilterBoolean>();
    // 		result[0].ColumnName.Should().Be(columnName);
    // 		var casted = (TucFilterBoolean)result[0];
    // 		casted.ComparisonMethod.Should().Be(method);
    // 		casted.Value.Should().NotBeNull();
    // 		casted.Value.Should().Be(value.ToString());
    // 	}
    // 	#endregion
    //
    // 	#region << TucFilterDateTime >>
    // 	{
    // 		var method = TucFilterDateTimeComparisonMethod.Lower;
    // 		DateTime? value = DateTime.Now;
    // 		test = new List<TfFilterBase> {
    // 			new TucFilterDateTime(){
    // 				Id = columnId,
    // 				ColumnName = columnName,
    // 				ComparisonMethod = method,
    // 				Value = value?.ToString(TfConstants.DateTimeFormat)
    // 			}
    // 		};
    // 		queryValue = NavigatorExt.SerializeFiltersForUrl(test);
    // 		result = NavigatorExt.DeserializeFiltersFromUrl(queryValue);
    // 		result.Should().NotBeNull();
    // 		result.Count.Should().Be(1);
    // 		result[0].Should().BeOfType<TucFilterDateTime>();
    // 		result[0].ColumnName.Should().Be(columnName);
    // 		var casted = (TucFilterDateTime)result[0];
    // 		casted.ComparisonMethod.Should().Be(method);
    // 		casted.Value.Should().NotBeNull();
    // 		if(casted.Value is not null)
    // 			casted.Value.Should().Be(value.Value.ToString(TfConstants.DateTimeFormat));
    // 	}
    // 	#endregion
    //
    // 	#region << TucFilterGuid >>
    // 	{
    // 		var method = TucFilterGuidComparisonMethod.IsNotEmpty;
    // 		Guid? value = Guid.NewGuid();
    // 		test = new List<TfFilterBase> {
    // 			new TucFilterGuid(){
    // 				Id = columnId,
    // 				ColumnName = columnName,
    // 				ComparisonMethod = method,
    // 				Value = value.ToString()
    // 			}
    // 		};
    // 		queryValue = NavigatorExt.SerializeFiltersForUrl(test);
    // 		result = NavigatorExt.DeserializeFiltersFromUrl(queryValue);
    // 		result.Should().NotBeNull();
    // 		result.Count.Should().Be(1);
    // 		result[0].Should().BeOfType<TucFilterGuid>();
    // 		result[0].ColumnName.Should().Be(columnName);
    // 		var casted = (TucFilterGuid)result[0];
    // 		casted.ComparisonMethod.Should().Be(method);
    // 		casted.Value.Should().NotBeNull();
    // 		casted.Value.Should().Be(value.ToString());
    // 	}
    // 	#endregion
    //
    // 	#region << TucFilterNumeric >>
    // 	{
    // 		var method = TucFilterNumericComparisonMethod.Lower;
    // 		decimal? value = (decimal)0.235;
    // 		test = new List<TfFilterBase> {
    // 			new TucFilterNumeric(){
    // 				Id = columnId,
    // 				ColumnName = columnName,
    // 				ComparisonMethod = method,
    // 				Value = value.ToString()
    // 			}
    // 		};
    // 		queryValue = NavigatorExt.SerializeFiltersForUrl(test);
    // 		result = NavigatorExt.DeserializeFiltersFromUrl(queryValue);
    // 		result.Should().NotBeNull();
    // 		result.Count.Should().Be(1);
    // 		result[0].Should().BeOfType<TucFilterNumeric>();
    // 		result[0].ColumnName.Should().Be(columnName);
    // 		var casted = (TucFilterNumeric)result[0];
    // 		casted.ComparisonMethod.Should().Be(method);
    // 		casted.Value.Should().NotBeNull();
    // 		casted.Value.Should().Be(value.ToString());
    // 	}
    // 	#endregion
    //
    // 	#region << TucFilterText >>
    // 	{
    // 		var method = TfFilterTextComparisonMethod.Contains;
    // 		string value = Guid.NewGuid().ToString();
    // 		test = new List<TfFilterBase> {
    // 			new TucFilterText(){
    // 				Id = columnId,
    // 				ColumnName = columnName,
    // 				ComparisonMethod = method,
    // 				Value = value
    // 			}
    // 		};
    // 		queryValue = NavigatorExt.SerializeFiltersForUrl(test);
    // 		result = NavigatorExt.DeserializeFiltersFromUrl(queryValue);
    // 		result.Should().NotBeNull();
    // 		result.Count.Should().Be(1);
    // 		result[0].Should().BeOfType<TucFilterText>();
    // 		result[0].ColumnName.Should().Be(columnName);
    // 		var casted = (TucFilterText)result[0];
    // 		casted.ComparisonMethod.Should().Be(method);
    // 		casted.Value.Should().NotBeNull();
    // 		casted.Value.Should().Be(value);
    //
    // 		//test with special symbols
    // 		value = "//$~(&&)";
    // 		test = new List<TfFilterBase> {
    // 			new TucFilterText(){
    // 				Id = columnId,
    // 				ColumnName = columnName,
    // 				ComparisonMethod = method,
    // 				Value = value
    // 			}
    // 		};
    // 		queryValue = NavigatorExt.SerializeFiltersForUrl(test);
    // 		result = NavigatorExt.DeserializeFiltersFromUrl(queryValue);
    // 		result.Should().NotBeNull();
    // 		result.Count.Should().Be(1);
    // 		result[0].Should().BeOfType<TucFilterText>();
    // 		result[0].ColumnName.Should().Be(columnName);
    // 		casted = (TucFilterText)result[0];
    // 		casted.ComparisonMethod.Should().Be(method);
    // 		casted.Value.Should().NotBeNull();
    // 		casted.Value.Should().Be(value);
    // 	}
    // 	#endregion
    //
    // 	#region << With child filters >>
    // 	{
    // 		var boolColumnName = Guid.NewGuid().ToString().Split("-", StringSplitOptions.RemoveEmptyEntries)[0];
    // 		var boolMethod = TucFilterBooleanComparisonMethod.NotEqual;
    // 		bool? boolValue = false;
    //
    // 		var textColumnName = Guid.NewGuid().ToString().Split("-", StringSplitOptions.RemoveEmptyEntries)[0];
    // 		var textMethod = TfFilterTextComparisonMethod.Contains;
    // 		var textValue = "//$~(&&)";
    //
    // 		test = new List<TfFilterBase> {
    // 			new TfFilterAnd(){
    // 				Id = columnId,
    // 				Filters = new List<TfFilterBase>{
    // 					new TfFilterOr(){
    // 						Id = Guid.NewGuid(),
    // 						Filters = new List<TfFilterBase>{
    // 							new TucFilterBoolean{
    // 								ColumnName = boolColumnName,
    // 								ComparisonMethod = boolMethod,
    // 								Value = boolValue.ToString(),
    // 							},
    // 							new TucFilterText{
    // 								ColumnName = textColumnName,
    // 								ComparisonMethod = textMethod,
    // 								Value = textValue,
    // 							},
    // 						}
    // 					}
    // 				}
    // 			}
    // 		};
    // 		queryValue = NavigatorExt.SerializeFiltersForUrl(test);
    // 		result = NavigatorExt.DeserializeFiltersFromUrl(queryValue);
    // 		result.Should().NotBeNull();
    // 		result.Count.Should().Be(1);
    // 		result[0].Should().BeOfType<TfFilterAnd>();
    // 		var lvl1Casted = (TfFilterAnd)result[0];
    // 		lvl1Casted.Filters.Should().NotBeNull();
    // 		lvl1Casted.Filters.Count.Should().Be(1);
    //
    // 		lvl1Casted.Filters[0].Should().BeOfType<TfFilterOr>();
    // 		var lvl2Casted = (TfFilterOr)lvl1Casted.Filters[0];
    // 		lvl2Casted.Filters.Should().NotBeNull();
    // 		lvl2Casted.Filters.Count.Should().Be(2);
    //
    // 		lvl2Casted.Filters[0].Should().BeOfType<TucFilterBoolean>();
    // 		lvl2Casted.Filters[1].Should().BeOfType<TucFilterText>();
    //
    // 		var lvl31Casted = (TucFilterBoolean)lvl2Casted.Filters[0];
    // 		var lvl33Casted = (TucFilterText)lvl2Casted.Filters[1];
    //
    // 		lvl31Casted.ColumnName.Should().Be(boolColumnName);
    // 		lvl31Casted.ComparisonMethod.Should().Be(boolMethod);
    // 		lvl31Casted.Value.Should().Be(boolValue.ToString());
    //
    // 		lvl33Casted.ColumnName.Should().Be(textColumnName);
    // 		lvl33Casted.ComparisonMethod.Should().Be(textMethod);
    // 		lvl33Casted.Value.Should().Be(textValue);
    //
    // 	}
    // 	#endregion
    // }
    //
    // [Fact]
    // public void SpaceViewSortUrlSerializationTests()
    // {
    // 	var test = new List<TucSort>();
    // 	var result = new List<TucSort>();
    // 	var columnName1 = Guid.NewGuid().ToString().Split("-", StringSplitOptions.RemoveEmptyEntries)[0];
    // 	var columnOrder1 = TucSortDirection.ASC;
    // 	var columnName2 = Guid.NewGuid().ToString().Split("-", StringSplitOptions.RemoveEmptyEntries)[0];
    // 	var columnOrder2 = TucSortDirection.DESC;
    // 	string queryValue = null;
    //
    // 	#region << One >>
    // 	{
    // 		test = new List<TucSort>(){
    // 			new TucSort{
    // 				ColumnName = columnName1,
    // 				Direction = columnOrder1,
    // 			}
    // 		};
    // 		queryValue = NavigatorExt.SerializeSortsForUrl(test);
    // 		result = NavigatorExt.DeserializeSortsFromUrl(queryValue);
    // 		result.Should().NotBeNull();
    // 		result.Count.Should().Be(1);
    // 		result[0].Should().BeOfType<TucSort>();
    // 		result[0].ColumnName.Should().Be(columnName1);
    // 		result[0].Direction.Should().Be(columnOrder1);
    // 	}
    // 	#endregion
    //
    // 	#region << Many >>
    // 	{
    // 		test = new List<TucSort>(){
    // 			new TucSort{
    // 				ColumnName = columnName1,
    // 				Direction = columnOrder1,
    // 			},
    // 			new TucSort{
    // 				ColumnName = columnName2,
    // 				Direction = columnOrder2,
    // 			},
    // 		};
    // 		queryValue = NavigatorExt.SerializeSortsForUrl(test);
    // 		result = NavigatorExt.DeserializeSortsFromUrl(queryValue);
    // 		result.Should().NotBeNull();
    // 		result.Count.Should().Be(2);
    // 		result[0].Should().BeOfType<TucSort>();
    // 		result[0].ColumnName.Should().Be(columnName1);
    // 		result[0].Direction.Should().Be(columnOrder1);
    // 		result[1].Should().BeOfType<TucSort>();
    // 		result[1].ColumnName.Should().Be(columnName2);
    // 		result[1].Direction.Should().Be(columnOrder2);
    // 	}
    // 	#endregion
    //
    //
    // }
}
