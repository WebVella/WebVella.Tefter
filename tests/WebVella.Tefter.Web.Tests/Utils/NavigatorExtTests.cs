using FluentAssertions;
using System;
using System.Collections.Generic;
using WebVella.Tefter.Web.Models;
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
		TucRouteState result = new();
		Guid spaceId = Guid.NewGuid();
		Guid userId = Guid.NewGuid();
		Guid providerId = Guid.NewGuid();
		Guid viewId = Guid.NewGuid();
		Guid dataId = Guid.NewGuid();
		Guid nodeId = Guid.NewGuid();

		//When
		uri = new Uri($"{baseUrl}");
		result = NavigatorExt.GetNodeData(uri);
		result.NodesDict.Should().NotBeNull();
		result.NodesDict.Count.Should().Be(0);
		result.FirstNode.Should().Be(RouteDataFirstNode.Home);
		result.SecondNode.Should().Be(default);
		result.ThirdNode.Should().Be(default);

		uri = new Uri($"{baseUrl}{String.Format(TfConstants.AdminDashboardUrl)}");
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

		#region << Space node >>
		uri = new Uri($"{baseUrl}{String.Format(TfConstants.SpaceNodePageUrl, spaceId, nodeId)}");
		result = NavigatorExt.GetNodeData(uri);
		result.NodesDict.Should().NotBeNull();
		result.NodesDict.Count.Should().Be(4);
		result.FirstNode.Should().Be(RouteDataFirstNode.Space);
		result.SecondNode.Should().Be(RouteDataSecondNode.SpacePage);
		result.ThirdNode.Should().Be(RouteDataThirdNode.Details);
		result.SpaceId.Should().Be(spaceId);
		result.SpaceNodeId.Should().Be(nodeId);
		#endregion
	}

	[Fact]
	public void GetQueryDataTests()
	{
		//Given
		var baseUrl = "http://localhost";
		Uri uri = new Uri(baseUrl);
		TucRouteState result = new();

		int page = 3;
		int pageSize = 12;
		string search = "$~@32~/";

		var filterBoolColumnName = Guid.NewGuid().ToString().Split("-", StringSplitOptions.RemoveEmptyEntries)[0];
		var filterBoolMethod = TucFilterBooleanComparisonMethod.NotEqual;
		bool? filterBoolValue = false;
		var filterDateOnlyColumnName = Guid.NewGuid().ToString().Split("-", StringSplitOptions.RemoveEmptyEntries)[0];
		var filterDateOnlyMethod = TucFilterDateTimeComparisonMethod.Greater;
		DateOnly? filterDateOnlyValue = DateOnly.FromDateTime(DateTime.Now);
		var filterTextColumnName = Guid.NewGuid().ToString().Split("-", StringSplitOptions.RemoveEmptyEntries)[0];
		var filterTextMethod = TucFilterTextComparisonMethod.Contains;
		var filterTextValue = "//$~(&&)";
		var filters = new List<TucFilterBase> {
				new TucFilterAnd(){
					Filters = new List<TucFilterBase>{
						new TucFilterOr(){
							Id = Guid.NewGuid(),
							Filters = new List<TucFilterBase>{
								new TucFilterBoolean{
									ColumnName = filterBoolColumnName,
									ComparisonMethod = filterBoolMethod,
									Value = filterBoolValue.ToString(),
								},
								new TucFilterText{
									ColumnName = filterTextColumnName,
									ComparisonMethod = filterTextMethod,
									Value = filterTextValue,
								},
							}
						}
					}
				}
			};

		var sortColumnName1 = Guid.NewGuid().ToString().Split("-", StringSplitOptions.RemoveEmptyEntries)[0];
		var sortColumnOrder1 = TucSortDirection.ASC;
		var sortColumnName2 = Guid.NewGuid().ToString().Split("-", StringSplitOptions.RemoveEmptyEntries)[0];
		var sortColumnOrder2 = TucSortDirection.DESC;
		var sorts = new List<TucSort>(){
				new TucSort{
					DbName = sortColumnName1,
					Direction = sortColumnOrder1,
				},
				new TucSort{
					DbName = sortColumnName2,
					Direction = sortColumnOrder2,
				},
			};

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
			lvl1.Should().BeOfType<TucFilterAnd>();
			var lvl1Filter = (TucFilterAnd)lvl1;
			lvl1Filter.Filters.Should().NotBeNull();
			lvl1Filter.Filters.Count.Should().Be(1);
			var lvl2 = lvl1Filter.Filters[0];
			lvl2.Should().BeOfType<TucFilterOr>();
			var lvl2Filter = (TucFilterOr)lvl2;
			lvl2Filter.Filters.Should().NotBeNull();
			lvl2Filter.Filters.Count.Should().Be(2);

			var lvl31 = lvl2Filter.Filters[0];
			lvl31.Should().BeOfType<TucFilterBoolean>();
			var lvl31Filter = (TucFilterBoolean)lvl31;
			lvl31Filter.ColumnName.Should().Be(filterBoolColumnName);
			lvl31Filter.ComparisonMethod.Should().Be(filterBoolMethod);
			lvl31Filter.Value.Should().Be(filterBoolValue.ToString());

			var lvl33 = lvl2Filter.Filters[1];
			lvl33.Should().BeOfType<TucFilterText>();
			var lvl33Filter = (TucFilterText)lvl33;
			lvl33Filter.ColumnName.Should().Be(filterTextColumnName);
			lvl33Filter.ComparisonMethod.Should().Be(filterTextMethod);
			lvl33Filter.Value.Should().Be(filterTextValue);

		}
		#endregion
		#region << sorts >>
		{
			result.Sorts.Should().NotBeNull();
			result.Sorts.Count.Should().Be(2);
			result.Sorts[0].DbName.Should().Be(sortColumnName1);
			result.Sorts[0].Direction.Should().Be(sortColumnOrder1);
			result.Sorts[1].DbName.Should().Be(sortColumnName2);
			result.Sorts[1].Direction.Should().Be(sortColumnOrder2);

		}
		#endregion
	}


	[Fact]
	public void SpaceViewFilterUrlSerializationTests()
	{

		var test = new List<TucFilterBase>();
		string queryValue = null;
		var result = new List<TucFilterBase>();
		var columnId = Guid.NewGuid();
		var columnName = Guid.NewGuid().ToString().Split("-", StringSplitOptions.RemoveEmptyEntries)[0];

		#region << TucFilterAnd >>
		{
			test = new List<TucFilterBase> {
				new TucFilterAnd(){
					Id = columnId,
					//ColumnName = columnName,
					Filters = new()
				}
			};
			queryValue = NavigatorExt.SerializeFiltersForUrl(test);
			result = NavigatorExt.DeserializeFiltersFromUrl(queryValue);
			result.Should().NotBeNull();
			result.Count.Should().Be(1);
			result[0].Should().BeOfType<TucFilterAnd>();
			//result[0].ColumnName.Should().Be(columnName);
			var casted = (TucFilterAnd)result[0];
			casted.Filters.Should().NotBeNull();
			casted.Filters.Count.Should().Be(0);
		}
		#endregion

		#region << TucFilterOr >>
		{
			test = new List<TucFilterBase> {
				new TucFilterOr(){
					Id = columnId,
					//ColumnName = columnName,
					Filters = new()
				}
			};
			queryValue = NavigatorExt.SerializeFiltersForUrl(test);
			result = NavigatorExt.DeserializeFiltersFromUrl(queryValue);
			result.Should().NotBeNull();
			result.Count.Should().Be(1);
			result[0].Should().BeOfType<TucFilterOr>();
			//result[0].ColumnName.Should().Be(columnName);
			var casted = (TucFilterOr)result[0];
			casted.Filters.Should().NotBeNull();
			casted.Filters.Count.Should().Be(0);
		}
		#endregion

		#region << TucFilterBoolean >>
		{
			var method = TucFilterBooleanComparisonMethod.NotEqual;
			bool? value = false;
			test = new List<TucFilterBase> {
				new TucFilterBoolean(){
					Id = columnId,
					ColumnName = columnName,
					ComparisonMethod = method,
					Value = value.ToString()
				}
			};
			queryValue = NavigatorExt.SerializeFiltersForUrl(test);
			result = NavigatorExt.DeserializeFiltersFromUrl(queryValue);
			result.Should().NotBeNull();
			result.Count.Should().Be(1);
			result[0].Should().BeOfType<TucFilterBoolean>();
			result[0].ColumnName.Should().Be(columnName);
			var casted = (TucFilterBoolean)result[0];
			casted.ComparisonMethod.Should().Be(method);
			casted.Value.Should().NotBeNull();
			casted.Value.Should().Be(value.ToString());
		}
		#endregion

		#region << TucFilterDateTime >>
		{
			var method = TucFilterDateTimeComparisonMethod.Lower;
			DateTime? value = DateTime.Now;
			test = new List<TucFilterBase> {
				new TucFilterDateTime(){
					Id = columnId,
					ColumnName = columnName,
					ComparisonMethod = method,
					Value = value?.ToString(TfConstants.DateTimeFormat)
				}
			};
			queryValue = NavigatorExt.SerializeFiltersForUrl(test);
			result = NavigatorExt.DeserializeFiltersFromUrl(queryValue);
			result.Should().NotBeNull();
			result.Count.Should().Be(1);
			result[0].Should().BeOfType<TucFilterDateTime>();
			result[0].ColumnName.Should().Be(columnName);
			var casted = (TucFilterDateTime)result[0];
			casted.ComparisonMethod.Should().Be(method);
			casted.Value.Should().NotBeNull();
			if(casted.Value is not null)
				casted.Value.Should().Be(value.Value.ToString(TfConstants.DateTimeFormat));
		}
		#endregion

		#region << TucFilterGuid >>
		{
			var method = TucFilterGuidComparisonMethod.IsNotEmpty;
			Guid? value = Guid.NewGuid();
			test = new List<TucFilterBase> {
				new TucFilterGuid(){
					Id = columnId,
					ColumnName = columnName,
					ComparisonMethod = method,
					Value = value.ToString()
				}
			};
			queryValue = NavigatorExt.SerializeFiltersForUrl(test);
			result = NavigatorExt.DeserializeFiltersFromUrl(queryValue);
			result.Should().NotBeNull();
			result.Count.Should().Be(1);
			result[0].Should().BeOfType<TucFilterGuid>();
			result[0].ColumnName.Should().Be(columnName);
			var casted = (TucFilterGuid)result[0];
			casted.ComparisonMethod.Should().Be(method);
			casted.Value.Should().NotBeNull();
			casted.Value.Should().Be(value.ToString());
		}
		#endregion

		#region << TucFilterNumeric >>
		{
			var method = TucFilterNumericComparisonMethod.Lower;
			decimal? value = (decimal)0.235;
			test = new List<TucFilterBase> {
				new TucFilterNumeric(){
					Id = columnId,
					ColumnName = columnName,
					ComparisonMethod = method,
					Value = value.ToString()
				}
			};
			queryValue = NavigatorExt.SerializeFiltersForUrl(test);
			result = NavigatorExt.DeserializeFiltersFromUrl(queryValue);
			result.Should().NotBeNull();
			result.Count.Should().Be(1);
			result[0].Should().BeOfType<TucFilterNumeric>();
			result[0].ColumnName.Should().Be(columnName);
			var casted = (TucFilterNumeric)result[0];
			casted.ComparisonMethod.Should().Be(method);
			casted.Value.Should().NotBeNull();
			casted.Value.Should().Be(value.ToString());
		}
		#endregion

		#region << TucFilterText >>
		{
			var method = TucFilterTextComparisonMethod.Contains;
			string value = Guid.NewGuid().ToString();
			test = new List<TucFilterBase> {
				new TucFilterText(){
					Id = columnId,
					ColumnName = columnName,
					ComparisonMethod = method,
					Value = value
				}
			};
			queryValue = NavigatorExt.SerializeFiltersForUrl(test);
			result = NavigatorExt.DeserializeFiltersFromUrl(queryValue);
			result.Should().NotBeNull();
			result.Count.Should().Be(1);
			result[0].Should().BeOfType<TucFilterText>();
			result[0].ColumnName.Should().Be(columnName);
			var casted = (TucFilterText)result[0];
			casted.ComparisonMethod.Should().Be(method);
			casted.Value.Should().NotBeNull();
			casted.Value.Should().Be(value);

			//test with special symbols
			value = "//$~(&&)";
			test = new List<TucFilterBase> {
				new TucFilterText(){
					Id = columnId,
					ColumnName = columnName,
					ComparisonMethod = method,
					Value = value
				}
			};
			queryValue = NavigatorExt.SerializeFiltersForUrl(test);
			result = NavigatorExt.DeserializeFiltersFromUrl(queryValue);
			result.Should().NotBeNull();
			result.Count.Should().Be(1);
			result[0].Should().BeOfType<TucFilterText>();
			result[0].ColumnName.Should().Be(columnName);
			casted = (TucFilterText)result[0];
			casted.ComparisonMethod.Should().Be(method);
			casted.Value.Should().NotBeNull();
			casted.Value.Should().Be(value);
		}
		#endregion

		#region << With child filters >>
		{
			var boolColumnName = Guid.NewGuid().ToString().Split("-", StringSplitOptions.RemoveEmptyEntries)[0];
			var boolMethod = TucFilterBooleanComparisonMethod.NotEqual;
			bool? boolValue = false;

			var textColumnName = Guid.NewGuid().ToString().Split("-", StringSplitOptions.RemoveEmptyEntries)[0];
			var textMethod = TucFilterTextComparisonMethod.Contains;
			var textValue = "//$~(&&)";

			test = new List<TucFilterBase> {
				new TucFilterAnd(){
					Id = columnId,
					Filters = new List<TucFilterBase>{
						new TucFilterOr(){
							Id = Guid.NewGuid(),
							Filters = new List<TucFilterBase>{
								new TucFilterBoolean{
									ColumnName = boolColumnName,
									ComparisonMethod = boolMethod,
									Value = boolValue.ToString(),
								},
								new TucFilterText{
									ColumnName = textColumnName,
									ComparisonMethod = textMethod,
									Value = textValue,
								},
							}
						}
					}
				}
			};
			queryValue = NavigatorExt.SerializeFiltersForUrl(test);
			result = NavigatorExt.DeserializeFiltersFromUrl(queryValue);
			result.Should().NotBeNull();
			result.Count.Should().Be(1);
			result[0].Should().BeOfType<TucFilterAnd>();
			var lvl1Casted = (TucFilterAnd)result[0];
			lvl1Casted.Filters.Should().NotBeNull();
			lvl1Casted.Filters.Count.Should().Be(1);

			lvl1Casted.Filters[0].Should().BeOfType<TucFilterOr>();
			var lvl2Casted = (TucFilterOr)lvl1Casted.Filters[0];
			lvl2Casted.Filters.Should().NotBeNull();
			lvl2Casted.Filters.Count.Should().Be(2);

			lvl2Casted.Filters[0].Should().BeOfType<TucFilterBoolean>();
			lvl2Casted.Filters[1].Should().BeOfType<TucFilterText>();

			var lvl31Casted = (TucFilterBoolean)lvl2Casted.Filters[0];
			var lvl33Casted = (TucFilterText)lvl2Casted.Filters[1];

			lvl31Casted.ColumnName.Should().Be(boolColumnName);
			lvl31Casted.ComparisonMethod.Should().Be(boolMethod);
			lvl31Casted.Value.Should().Be(boolValue.ToString());

			lvl33Casted.ColumnName.Should().Be(textColumnName);
			lvl33Casted.ComparisonMethod.Should().Be(textMethod);
			lvl33Casted.Value.Should().Be(textValue);

		}
		#endregion
	}

	[Fact]
	public void SpaceViewSortUrlSerializationTests()
	{
		var test = new List<TucSort>();
		var result = new List<TucSort>();
		var columnName1 = Guid.NewGuid().ToString().Split("-", StringSplitOptions.RemoveEmptyEntries)[0];
		var columnOrder1 = TucSortDirection.ASC;
		var columnName2 = Guid.NewGuid().ToString().Split("-", StringSplitOptions.RemoveEmptyEntries)[0];
		var columnOrder2 = TucSortDirection.DESC;
		string queryValue = null;

		#region << One >>
		{
			test = new List<TucSort>(){
				new TucSort{
					DbName = columnName1,
					Direction = columnOrder1,
				}
			};
			queryValue = NavigatorExt.SerializeSortsForUrl(test);
			result = NavigatorExt.DeserializeSortsFromUrl(queryValue);
			result.Should().NotBeNull();
			result.Count.Should().Be(1);
			result[0].Should().BeOfType<TucSort>();
			result[0].DbName.Should().Be(columnName1);
			result[0].Direction.Should().Be(columnOrder1);
		}
		#endregion

		#region << Many >>
		{
			test = new List<TucSort>(){
				new TucSort{
					DbName = columnName1,
					Direction = columnOrder1,
				},
				new TucSort{
					DbName = columnName2,
					Direction = columnOrder2,
				},
			};
			queryValue = NavigatorExt.SerializeSortsForUrl(test);
			result = NavigatorExt.DeserializeSortsFromUrl(queryValue);
			result.Should().NotBeNull();
			result.Count.Should().Be(2);
			result[0].Should().BeOfType<TucSort>();
			result[0].DbName.Should().Be(columnName1);
			result[0].Direction.Should().Be(columnOrder1);
			result[1].Should().BeOfType<TucSort>();
			result[1].DbName.Should().Be(columnName2);
			result[1].Direction.Should().Be(columnOrder2);
		}
		#endregion


	}
}
