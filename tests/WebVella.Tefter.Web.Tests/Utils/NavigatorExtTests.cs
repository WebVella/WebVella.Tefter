using FluentAssertions;
using System;
using WebVella.Tefter.Web.Store;
using WebVella.Tefter.Web.Utils;

namespace WebVella.Tefter.Web.Tests.Utils;
public class NavigatorExtTests
{
	[Fact]
	public void GetNodeDataTests()
	{
		//Given
		var baseUrl = "http://localhost";
		Uri uri = new Uri(baseUrl);
		TfRouteState result = new();
		Guid spaceId = Guid.NewGuid();
		Guid userId = Guid.NewGuid();
		Guid providerId = Guid.NewGuid();
		Guid viewId = Guid.NewGuid();
		Guid dataId = Guid.NewGuid();

		//When
		uri = new Uri($"{baseUrl}");
		result = NavigatorExt.GetNodeData(uri);
		result.NodesDict.Should().NotBeNull();
		result.NodesDict.Count.Should().Be(0);
		result.FirstNode.Should().Be(RouteDataFirstNode.Home);
		result.SecondNode.Should().Be(default);
		result.ThirdNode.Should().Be(default);

		uri = new Uri($"{baseUrl}{String.Format(TfConstants.AdminPageUrl)}");
		result = NavigatorExt.GetNodeData(uri);
		result.NodesDict.Should().NotBeNull();
		result.NodesDict.Count.Should().Be(1);
		result.FirstNode.Should().Be(RouteDataFirstNode.Admin);
		result.SecondNode.Should().Be(RouteDataSecondNode.Dashboard);
		result.ThirdNode.Should().Be(default);

		#region << Admin Users >>
		uri = new Uri($"{baseUrl}{String.Format(TfConstants.AdminUsersPageUrl)}");
		result = NavigatorExt.GetNodeData(uri);
		result.NodesDict.Should().NotBeNull();
		result.NodesDict.Count.Should().Be(2);
		result.FirstNode.Should().Be(RouteDataFirstNode.Admin);
		result.SecondNode.Should().Be(RouteDataSecondNode.Users);
		result.ThirdNode.Should().Be(default);

		uri = new Uri($"{baseUrl}{String.Format(TfConstants.AdminUserDetailsPageUrl, userId)}");
		result = NavigatorExt.GetNodeData(uri);
		result.NodesDict.Should().NotBeNull();
		result.NodesDict.Count.Should().Be(3);
		result.FirstNode.Should().Be(RouteDataFirstNode.Admin);
		result.SecondNode.Should().Be(RouteDataSecondNode.Users);
		result.ThirdNode.Should().Be(RouteDataThirdNode.Details);
		result.UserId.Should().Be(userId);

		uri = new Uri($"{baseUrl}{String.Format(TfConstants.AdminUserAccessPageUrl, userId)}");
		result = NavigatorExt.GetNodeData(uri);
		result.NodesDict.Should().NotBeNull();
		result.NodesDict.Count.Should().Be(4);
		result.FirstNode.Should().Be(RouteDataFirstNode.Admin);
		result.SecondNode.Should().Be(RouteDataSecondNode.Users);
		result.ThirdNode.Should().Be(RouteDataThirdNode.Access);
		result.UserId.Should().Be(userId);

		uri = new Uri($"{baseUrl}{String.Format(TfConstants.AdminUserSavesViewsPageUrl, userId)}");
		result = NavigatorExt.GetNodeData(uri);
		result.NodesDict.Should().NotBeNull();
		result.NodesDict.Count.Should().Be(4);
		result.FirstNode.Should().Be(RouteDataFirstNode.Admin);
		result.SecondNode.Should().Be(RouteDataSecondNode.Users);
		result.ThirdNode.Should().Be(RouteDataThirdNode.Saves);
		result.UserId.Should().Be(userId);
		#endregion

		#region << Admin Data providers >>
		uri = new Uri($"{baseUrl}{String.Format(TfConstants.AdminDataProvidersPageUrl)}");
		result = NavigatorExt.GetNodeData(uri);
		result.NodesDict.Should().NotBeNull();
		result.NodesDict.Count.Should().Be(2);
		result.FirstNode.Should().Be(RouteDataFirstNode.Admin);
		result.SecondNode.Should().Be(RouteDataSecondNode.DataProviders);
		result.ThirdNode.Should().Be(default);

		uri = new Uri($"{baseUrl}{String.Format(TfConstants.AdminDataProviderDetailsPageUrl, providerId)}");
		result = NavigatorExt.GetNodeData(uri);
		result.NodesDict.Should().NotBeNull();
		result.NodesDict.Count.Should().Be(3);
		result.FirstNode.Should().Be(RouteDataFirstNode.Admin);
		result.SecondNode.Should().Be(RouteDataSecondNode.DataProviders);
		result.ThirdNode.Should().Be(RouteDataThirdNode.Details);
		result.DataProviderId.Should().Be(providerId);

		uri = new Uri($"{baseUrl}{String.Format(TfConstants.AdminDataProviderSchemaPageUrl, providerId)}");
		result = NavigatorExt.GetNodeData(uri);
		result.NodesDict.Should().NotBeNull();
		result.NodesDict.Count.Should().Be(4);
		result.FirstNode.Should().Be(RouteDataFirstNode.Admin);
		result.SecondNode.Should().Be(RouteDataSecondNode.DataProviders);
		result.ThirdNode.Should().Be(RouteDataThirdNode.Schema);
		result.DataProviderId.Should().Be(providerId);

		uri = new Uri($"{baseUrl}{String.Format(TfConstants.AdminDataProviderKeysPageUrl, providerId)}");
		result = NavigatorExt.GetNodeData(uri);
		result.NodesDict.Should().NotBeNull();
		result.NodesDict.Count.Should().Be(4);
		result.FirstNode.Should().Be(RouteDataFirstNode.Admin);
		result.SecondNode.Should().Be(RouteDataSecondNode.DataProviders);
		result.ThirdNode.Should().Be(RouteDataThirdNode.SharedKeys);
		result.DataProviderId.Should().Be(providerId);

		uri = new Uri($"{baseUrl}{String.Format(TfConstants.AdminDataProviderAuxColumnsPageUrl, providerId)}");
		result = NavigatorExt.GetNodeData(uri);
		result.NodesDict.Should().NotBeNull();
		result.NodesDict.Count.Should().Be(4);
		result.FirstNode.Should().Be(RouteDataFirstNode.Admin);
		result.SecondNode.Should().Be(RouteDataSecondNode.DataProviders);
		result.ThirdNode.Should().Be(RouteDataThirdNode.AuxColumns);
		result.DataProviderId.Should().Be(providerId);

		uri = new Uri($"{baseUrl}{String.Format(TfConstants.AdminDataProviderSynchronizationPageUrl, providerId)}");
		result = NavigatorExt.GetNodeData(uri);
		result.NodesDict.Should().NotBeNull();
		result.NodesDict.Count.Should().Be(4);
		result.FirstNode.Should().Be(RouteDataFirstNode.Admin);
		result.SecondNode.Should().Be(RouteDataSecondNode.DataProviders);
		result.ThirdNode.Should().Be(RouteDataThirdNode.Synchronization);
		result.DataProviderId.Should().Be(providerId);

		uri = new Uri($"{baseUrl}{String.Format(TfConstants.AdminDataProviderDataPageUrl, providerId)}");
		result = NavigatorExt.GetNodeData(uri);
		result.NodesDict.Should().NotBeNull();
		result.NodesDict.Count.Should().Be(4);
		result.FirstNode.Should().Be(RouteDataFirstNode.Admin);
		result.SecondNode.Should().Be(RouteDataSecondNode.DataProviders);
		result.ThirdNode.Should().Be(RouteDataThirdNode.Data);
		result.DataProviderId.Should().Be(providerId);
		#endregion

		#region << Admin SharedColumns >>
		uri = new Uri($"{baseUrl}{String.Format(TfConstants.AdminSharedColumnsPageUrl)}");
		result = NavigatorExt.GetNodeData(uri);
		result.NodesDict.Should().NotBeNull();
		result.NodesDict.Count.Should().Be(2);
		result.FirstNode.Should().Be(RouteDataFirstNode.Admin);
		result.SecondNode.Should().Be(RouteDataSecondNode.SharedColumns);
		result.ThirdNode.Should().Be(default);
		#endregion

		#region << Dashboard >>
		uri = new Uri($"{baseUrl}/");
		result = NavigatorExt.GetNodeData(uri);
		result.NodesDict.Should().NotBeNull();
		result.NodesDict.Count.Should().Be(0);
		result.FirstNode.Should().Be(RouteDataFirstNode.Home);
		result.SecondNode.Should().Be(RouteDataSecondNode.Dashboard);
		result.ThirdNode.Should().Be(default);
		#endregion

		#region << Space view >>
		uri = new Uri($"{baseUrl}{String.Format(TfConstants.SpaceViewPageUrl, spaceId, viewId)}");
		result = NavigatorExt.GetNodeData(uri);
		result.NodesDict.Should().NotBeNull();
		result.NodesDict.Count.Should().Be(4);
		result.FirstNode.Should().Be(RouteDataFirstNode.Space);
		result.SecondNode.Should().Be(RouteDataSecondNode.SpaceView);
		result.ThirdNode.Should().Be(RouteDataThirdNode.Details);
		result.SpaceId.Should().Be(spaceId);
		result.SpaceViewId.Should().Be(viewId);

		uri = new Uri($"{baseUrl}{String.Format(TfConstants.SpaceViewManagePageUrl, spaceId, viewId)}");
		result = NavigatorExt.GetNodeData(uri);
		result.NodesDict.Should().NotBeNull();
		result.NodesDict.Count.Should().Be(5);
		result.FirstNode.Should().Be(RouteDataFirstNode.Space);
		result.SecondNode.Should().Be(RouteDataSecondNode.SpaceView);
		result.ThirdNode.Should().Be(RouteDataThirdNode.Manage);
		result.SpaceId.Should().Be(spaceId);
		result.SpaceViewId.Should().Be(viewId);
		#endregion

		#region << Space data >>

		uri = new Uri($"{baseUrl}{String.Format(TfConstants.SpaceDataPageUrl, spaceId, dataId)}");
		result = NavigatorExt.GetNodeData(uri);
		result.NodesDict.Should().NotBeNull();
		result.NodesDict.Count.Should().Be(4);
		result.FirstNode.Should().Be(RouteDataFirstNode.Space);
		result.SecondNode.Should().Be(RouteDataSecondNode.SpaceData);
		result.ThirdNode.Should().Be(RouteDataThirdNode.Details);
		result.SpaceId.Should().Be(spaceId);
		result.SpaceDataId.Should().Be(dataId);

		uri = new Uri($"{baseUrl}{String.Format(TfConstants.SpaceDataViewsPageUrl, spaceId, dataId)}");
		result = NavigatorExt.GetNodeData(uri);
		result.NodesDict.Should().NotBeNull();
		result.NodesDict.Count.Should().Be(5);
		result.FirstNode.Should().Be(RouteDataFirstNode.Space);
		result.SecondNode.Should().Be(RouteDataSecondNode.SpaceData);
		result.ThirdNode.Should().Be(RouteDataThirdNode.Views);
		result.SpaceId.Should().Be(spaceId);
		result.SpaceDataId.Should().Be(dataId);

		#endregion

	}

}
