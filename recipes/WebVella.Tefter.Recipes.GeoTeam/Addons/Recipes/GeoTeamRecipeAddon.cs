using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebVella.Tefter.Addons;
using WebVella.Tefter.Assets.Addons;
using WebVella.Tefter.DataProviders.Csv;
using WebVella.Tefter.DataProviders.Csv.Addons;
using WebVella.Tefter.Models;
using WebVella.Tefter.Talk.Addons;
using WebVella.Tefter.TemplateProcessors.ExcelFile.Addons;
using WebVella.Tefter.TemplateProcessors.ExcelFile.Addons.RecipeSteps;
using WebVella.Tefter.TemplateProcessors.ExcelFile.Models;
using WebVella.Tefter.Web.Addons;
using WebVella.Tefter.Web.Addons.RecipeSteps;

namespace WebVella.Tefter.Recipes.GeoTeam;
public class GeoTalkRecipeAddon : ITfRecipeAddon
{
	public Guid AddonId { get; init; } = new Guid("b8f2c8ae-cc42-4c90-9f22-289498b4dffb");
	public string AddonName { get; init; } = "Geo Team";
	public string AddonDescription { get; init; } = "team collaboration around world cities";
	public string AddonFluentIconName { get; init; } = "Globe";
	public int SortIndex { get; init; } = 100;
	public string Author { get; init; } = "WebVella";
	public string Website { get; init; } = "https://tefter.webvella.com";
	public List<ITfRecipeStepAddon> Steps { get; init; } = new();

	public GeoTalkRecipeAddon()
	{
		var regularRoleId = new Guid("26dfd164-f1d7-4078-85e7-7f3f852f6577");
		var dataProviderId = new Guid("9ba6d290-8de0-475e-92f7-bda8d03667c7");
		int dataProviderIndex = 1;
		string dataProviderPrefix = $"dp{dataProviderIndex}_";
		var spaceId = new Guid("116a3baa-cd02-4926-8a76-639a2e15a011");
		var spaceDataId = new Guid("1a363406-6cb8-418b-95b2-9c72fb5d5fe9");
		var spaceViewId = new Guid("52363483-1d9b-4e7d-afe1-1602b7a7697f");
		var spacePageId = new Guid("831ce409-febb-40c3-a9b3-74af0c3bb8d3");
		var template1Id = new Guid("3ec7ec15-e3bb-4854-8688-0227e5efa646");
		var template1BlobId = new Guid("18716b34-b123-4366-be4d-c0bc967da59b");
		var nameSpace = "WebVella.Tefter.Recipes.GeoTeam";
		var template1FileName = "worldcities-template1.xlsx";
		var csvFileName = "worldcities.csv";
		var joinKey = "city_id";
		var talkChannelId = new Guid("36a26f87-2900-49ce-9757-c3e9da90f3b0");
		var talkChannelName = "general";
		var talkCountSharedColumnId = new Guid("2bb01d12-76bd-4247-8478-9ed5eb8a1707");
		var talkCountSharedColumnName = "sc_talk_count";
		var assetFolderId = new Guid("49ab08a6-e318-44e9-88ea-6441575043ed");
		var assetFolderName = "general";
		var assetsCountSharedColumnId = new Guid("801b1b8d-d7d1-44c6-9530-978a43fd2884");
		var assetsCountSharedColumnName = "sc_asset_count";

		var step11 = new TfInfoRecipeStep
		{
			Instance = new TfRecipeStepInstance
			{
				Visible = true,
				StepId = new Guid("0ea1122d-1377-4160-b6b6-2874ee9bbf9a"),
				StepMenuTitle = "Get started",
				StepContentTitle = "Geo Talk Sample application",
			},
			Data = new TfInfoRecipeStepData
			{
				HtmlContent =
			@"<p>This is a sample application that shows some of the tools of Tefter.bg in a practical environment. The use case is the need for a team to collaborate around the data of the world cities, with tools as discussions and export to templates</p>
			"
			}
		};

		var step12 = new TfInfoRecipeStep
		{
			Instance = new TfRecipeStepInstance
			{
				Visible = true,
				StepId = new Guid("f6197980-7a29-4a24-9c30-3886b59b456c"),
				StepContentTitle = "User accounts",
			},
			Data = new TfInfoRecipeStepData
			{
				HtmlContent =
			@"<p>The recipe will create 3 user accounts. One administrator with access to all pages and two regular users with access only to the 'World Cities' space.</p>
			<p>NOTE: Write down these details as you will need them.</p>
			<table>
				<thead>
					<tr>
						<th>email</th>
						<th>password</th>
						<th>role</th>
					</tr>
				</thead>
				<tbody>
					<tr>
						<td>admin@test.com</td>
						<td>1</td>
						<td>administrator</td>
					</tr>
					<tr>
						<td>user1@test.com</td>
						<td>1</td>
						<td>first regular user</td>
					</tr>
					<tr>
						<td>user2@test.com</td>
						<td>1</td>
						<td>second regular user</td>
					</tr>
				</tbody>
			</table>
			"
			}
		};

		var step13 = new TfInfoRecipeStep
		{
			Instance = new TfRecipeStepInstance
			{
				Visible = true,
				StepId = new Guid("7a93a020-fd7b-4f2b-8178-a0a9c1080522"),
				StepContentTitle = "Data source",
			},
			Data = new TfInfoRecipeStepData
			{
				HtmlContent =
			@"<p>City data will be imported from the uploaded <strong>world-cities.csv</strong> file. This file's data will be accessible through a CSV Data provider named <strong>WorldCities</strong>.</p>
			"
			}
		};

		var step14 = new TfInfoRecipeStep
		{
			Instance = new TfRecipeStepInstance
			{
				Visible = true,
				StepId = new Guid("301e0e07-21fe-484e-9c59-8599c4f53b32"),
				StepContentTitle = "Space",
			},
			Data = new TfInfoRecipeStepData
			{
				HtmlContent =
			@"<p>Regular users can interact with the data through the space <strong>World Cities</strong>. To achieve this, we will create the required a dataset, view and page.</p>
			"
			}
		};

		var step15 = new TfInfoRecipeStep
		{
			Instance = new TfRecipeStepInstance
			{
				Visible = true,
				StepId = new Guid("c65d1532-93f0-4029-8b31-c7bce26cf1e7"),
				StepContentTitle = "Data discussions",
			},
			Data = new TfInfoRecipeStepData
			{
				HtmlContent =
			@"<p>Discussions related to the data will be managed by the <strong>Talk addon applications</strong>, for which we will configure specific discussion channels.</p>
			"
			}
		};

		var step16 = new TfInfoRecipeStep
		{
			Instance = new TfRecipeStepInstance
			{
				Visible = true,
				StepId = new Guid("8a73d43f-a580-40fe-a441-d007141cda6b"),
				StepContentTitle = "Data export",
			},
			Data = new TfInfoRecipeStepData
			{
				HtmlContent =
			@"<p>Data and data selection will be exportable in a generic spreadsheet file as well as in several templates we will create for the purpose.</p>
			"
			}
		};

		Steps.Add(new TfGroupRecipeStep
		{
			Instance = new TfRecipeStepInstance
			{
				Visible = true,
				StepId = new Guid("5d8318bb-8427-425d-b4ee-9d4fe29977a5"),
				StepMenuTitle = "Get started",
				StepContentTitle = "Geo Talk Sample application",
			},
			Data = new TfGroupRecipeStepData
			{
				Steps = new List<ITfRecipeStepAddon>() {
				step11,step12,step13,step14,step15, step16
			}
			}
		});

		Steps.Add(new TfGroupRecipeStep
		{
			Instance = new TfRecipeStepInstance
			{
				Visible = false,
				StepId = new Guid("a0c78375-3a1d-4707-b04a-94ccf3a4ab14"),
				StepMenuTitle = "Roles and Users",
			},
			Data = new TfGroupRecipeStepData
			{
				Steps = new List<ITfRecipeStepAddon>()
			{
				new TfCreateUserRecipeStep
				{
							Instance = new TfRecipeStepInstance
			{
					Visible = true,
					StepId = new Guid("241ae707-0b8f-4b1c-88d3-5f8d438ab3f4"),
					StepMenuTitle = "System Administrator",
					StepContentTitle = "Create administrative account",
					StepContentDescription = "This user will be designated as the system superuser, automatically assigned the administrator role, and granted access to all system areas.",
					},
					Data = new TfCreateUserRecipeStepData{
					Email = "admin@test.com",
					Password = "1",
					FirstName = "System",
					LastName = "Administrator",
					Roles = new List<Guid> { TfConstants.ADMIN_ROLE_ID }
					}
				},
				new TfCreateRoleRecipeStep
				{
							Instance = new TfRecipeStepInstance
			{
					Visible = true,
					StepId = new Guid("89759f9b-062a-4a96-9b45-9b7007078e18"),
					StepMenuTitle = "Regular Role",
					StepContentTitle = "Regular Role",
					StepContentDescription = "created the 'Regular' role",
					},
					Data = new TfCreateRoleRecipeStepData{
					RoleId = regularRoleId,
					Name = "regular"
					}
				},
				new TfCreateUserRecipeStep
				{
							Instance = new TfRecipeStepInstance
			{
					Visible = true,
					StepId = new Guid("7ab6070d-46a7-4bb0-8538-3637898bb69b"),
					StepMenuTitle = "Regular User 1",
					StepContentTitle = "Regular User 1",
					StepContentDescription = "Has access only to the created user space.",
					},
					Data = new TfCreateUserRecipeStepData{
					Email = "user1@test.com",
					Password = "1",
					FirstName = "User",
					LastName = "1",
					Roles = new List<Guid> { regularRoleId }
					}
				},
				new TfCreateUserRecipeStep
				{
							Instance = new TfRecipeStepInstance
			{
					Visible = true,
					StepId = new Guid("e5b36d26-c9ab-4efe-9d2d-6399d501bc89"),
					StepMenuTitle = "Regular User 2",
					StepContentTitle = "Regular User 2",
					StepContentDescription = "Has access only to the created user space.",
					},
					Data = new TfCreateUserRecipeStepData{
					Email = "user2@test.com",
					Password = "1",
					FirstName = "User",
					LastName = "2",
					Roles = new List<Guid> { regularRoleId }
					}
				},
			}
			}
		});

		Steps.Add(new TfCreateRepositoryFileRecipeStep
		{
			Instance = new TfRecipeStepInstance
			{
				Visible = false,
				StepId = new Guid("ad312b7d-d2a4-446b-8913-2fa8a8a8f44a"),
				StepMenuTitle = "Upload file",
			},
			Data = new TfCreateRepositoryFileRecipeStepData
			{
				FileName = csvFileName,
				EmbeddedResourceName = $"{nameSpace}.Files.{csvFileName}",
				Assembly = this.GetType().Assembly,
			}
		});

		Steps.Add(new TfCreateDataProviderRecipeStep
		{
			Instance = new TfRecipeStepInstance
			{
				Visible = false,
				StepId = new Guid("c86fcc0a-980f-4c2f-b527-e74d467a6f1e"),
				StepMenuTitle = "Data Provider",
				StepContentTitle = "Creating the Data provider",
			},
			Data = new TfCreateDataProviderRecipeStepData
			{
				DataProviderId = dataProviderId,
				DataProviderIndex = dataProviderIndex,
				Name = "World Cities",
				Type = new CsvDataProvider(),
				SettingsJson = JsonSerializer.Serialize(new CsvDataProviderSettings
				{
					Filepath = $"tefter://fs/repository/{csvFileName}"
				}),
				Columns = new List<TfDataProviderColumn>()
			{
				new TfDataProviderColumn{
				Id = new Guid("b706b5b1-c296-4adb-a799-0f40993f6cfb"),
				CreatedOn = DateTime.Now,
				DataProviderId = dataProviderId,
				SourceName = "city",
				SourceType = "TEXT",
				DbName = $"{dataProviderPrefix}city",
				DbType = Database.TfDatabaseColumnType.Text,
				DefaultValue = "city",
				IsNullable = false,
				IsSearchable = true,
				IncludeInTableSearch = true,
				PreferredSearchType = TfDataProviderColumnSearchType.Contains,
				IsSortable = true,
				IsUnique = false
				},
				new TfDataProviderColumn{
				Id = new Guid("0dbc9155-3f1c-4c69-8af8-4b0d91f1301d"),
				CreatedOn = DateTime.Now,
				DataProviderId = dataProviderId,
				SourceName = "country",
				SourceType = "TEXT",
				DbName = $"{dataProviderPrefix}country",
				DbType = Database.TfDatabaseColumnType.Text,
				DefaultValue = "country",
				IsNullable = false,
				IsSearchable = true,
				IncludeInTableSearch = true,
				PreferredSearchType = TfDataProviderColumnSearchType.Contains,
				IsSortable = true,
				IsUnique = false
				},
				new TfDataProviderColumn{
				Id = new Guid("af549f23-0a3a-4cfc-9033-7a4cbdbdce16"),
				CreatedOn = DateTime.Now,
				DataProviderId = dataProviderId,
				SourceName = "population",
				SourceType = "LONG_INTEGER",
				DbName = $"{dataProviderPrefix}population",
				DbType = Database.TfDatabaseColumnType.LongInteger,
				DefaultValue = "0",
				IsNullable = false,
				IsSearchable = false,
				IncludeInTableSearch = false,
				PreferredSearchType = TfDataProviderColumnSearchType.Equals,
				IsSortable = true,
				IsUnique = false
				},
				new TfDataProviderColumn{
				Id = new Guid("22e9ed31-fe26-49d6-84e0-2c7f8fd13344"),
				CreatedOn = DateTime.Now,
				DataProviderId = dataProviderId,
				SourceName = "id",
				SourceType = "LONG_INTEGER",
				DbName = $"{dataProviderPrefix}id",
				DbType = Database.TfDatabaseColumnType.LongInteger,
				DefaultValue = "0",
				IsNullable = false,
				IsSearchable = false,
				IncludeInTableSearch = false,
				PreferredSearchType = TfDataProviderColumnSearchType.Equals,
				IsSortable = false,
				IsUnique = true
				}
			},
				TriggerDataSynchronization = true,
				SynchPrimaryKeyColumns = new List<string> { $"{dataProviderPrefix}id" },
				JoinKeys = new List<TfRecipeStepDataProviderJoinKey>{
				new TfRecipeStepDataProviderJoinKey(
					id:new Guid("81b6b073-f4f2-44b9-a29b-d8024a6e910c"),
					dbName:joinKey,
					columns:new List<string>{$"{dataProviderPrefix}id"})
			}
			}
		});

		Steps.Add(new TfGroupRecipeStep
		{
			Instance = new TfRecipeStepInstance
			{
				Visible = false,
				StepId = new Guid("a0c78375-3a1d-4707-b04a-94ccf3a4ab14"),
				StepMenuTitle = "Setup Addons",
			},
			Data = new TfGroupRecipeStepData
			{
				Steps = new List<ITfRecipeStepAddon>(){
				new TfCreateSharedColumnRecipeStep
				{
							Instance = new TfRecipeStepInstance
			{
					Visible = false,
					StepId = new Guid("6f510bd2-32ab-4510-b965-7d044a6f813a"),
					StepMenuTitle = "Talk Count Shared Column",
					},
					Data = new TfCreateSharedColumnRecipeStepData {
					SharedColumnId = talkCountSharedColumnId,
					DbName = talkCountSharedColumnName,
					ColumnType = Database.TfDatabaseColumnType.LongInteger,
					IncludeInTableSearch = false,
					JoinKey = joinKey,
					}
				},
				new TfCreateSharedColumnRecipeStep
				{
							Instance = new TfRecipeStepInstance
			{
					Visible = false,
					StepId = new Guid("e0c68dfb-7773-42bd-b17e-7c768366cb4c"),
					StepMenuTitle = "Asset Count Shared Column",
					},
					Data = new TfCreateSharedColumnRecipeStepData{
					SharedColumnId = assetsCountSharedColumnId,
					DbName = assetsCountSharedColumnName,
					ColumnType = Database.TfDatabaseColumnType.LongInteger,
					IncludeInTableSearch = false,
					JoinKey = joinKey,
					}
				},
			}
			}
		});

		Steps.Add(new TfCreateTalkChannelRecipeStep
		{
			Instance = new TfRecipeStepInstance
			{
				Visible = false,
				StepId = new Guid("16cd166b-75a2-458f-8423-b00e170cefe5"),
				StepMenuTitle = "Talk channel",
			},
			Data = new TfCreateTalkChannelRecipeStepData
			{
				ChannelId = talkChannelId,
				Name = talkChannelName,
				CountSharedColumnName = talkCountSharedColumnName,
				JoinKey = joinKey,
			}
		});

		Steps.Add(new TfCreateAssetFolderRecipeStep
		{
			Instance = new TfRecipeStepInstance
			{
				Visible = false,
				StepId = new Guid("d8726839-678d-48ae-8c22-e8e451aad453"),
				StepMenuTitle = "Asset Folder",
			},
			Data = new TfCreateAssetFolderRecipeStepData
			{
				FolderId = assetFolderId,
				Name = assetFolderName,
				CountSharedColumnName = assetsCountSharedColumnName,
				JoinKey = joinKey,
			}
		});

		Steps.Add(new TfCreateSpaceRecipeStep
		{
			Instance = new TfRecipeStepInstance
			{
				Visible = false,
				StepId = new Guid("b0efcbc7-0157-4d35-8221-54dacfd3aa91"),
				StepMenuTitle = "Create space",
			},
			Data = new TfCreateSpaceRecipeStepData
			{
				SpaceId = spaceId,
				Name = "World Cities",
				IsPrivate = true,
				Roles = new List<Guid> { regularRoleId },
				Color = Web.Models.TfColor.Teal500,
				FluentIconName = "Globe",
				Position = 100
			}
		});

		Steps.Add(new TfCreateSpaceDataRecipeStep
		{
			Instance = new TfRecipeStepInstance
			{
				Visible = false,
				StepId = new Guid("1e8bec9f-e4ea-452c-9bb2-04dbb5f43e98"),
				StepMenuTitle = "Create space data",
			},
			Data = new TfCreateSpaceDataRecipeStepData
			{
				SpaceDataId = spaceDataId,
				DataProviderId = dataProviderId,
				SpaceId = spaceId,
				Name = "World Cities",
				Position = 100,
				Columns = new(),
				Filters = new(),
				SortOrders = new List<TfSort> { new TfSort { ColumnName = $"{dataProviderPrefix}city", Direction = TfSortDirection.ASC } }
			}
		});

		Steps.Add(new TfCreateSpaceViewRecipeStep
		{
			Instance = new TfRecipeStepInstance
			{
				Visible = false,
				StepId = new Guid("34d0cfe8-852e-460a-acbf-c74b13482673"),
				StepMenuTitle = "Create space view",
			},
			Data = new TfCreateSpaceViewRecipeStepData
			{
				SpaceDataId = spaceDataId,
				SpaceViewId = spaceViewId,
				SpaceId = spaceId,
				Name = "World Cities",
				Position = 100,
				Type = TfSpaceViewType.DataGrid,
				Settings = new Web.Models.TucSpaceViewSettings
				{
					CanCreateRows = false,
					CanDeleteRows = false,
					FreezeFinalNColumns = 0,
					FreezeStartingNColumns = 0,
				},
				Presets = new(){
				new TfSpaceViewPreset{
					Id = new Guid("8598ced6-55ff-4b7a-a51e-d83957aa2084"),
					IsGroup = false,
					Color = Web.Models.TfColor.Amber500,
					Name = "1M+",
					Filters = new List<TfFilterBase>(){
						new TfFilterNumeric($"{dataProviderPrefix}population",TfFilterNumericComparisonMethod.GreaterOrEqual,"1000000")
					},
					SortOrders = new List<TfSort>{
						new TfSort($"{dataProviderPrefix}population",TfSortDirection.DESC)
					}
				},
				new TfSpaceViewPreset{
					Id = new Guid("5431e7f8-e871-407b-8ade-58baa1ad69ca"),
					IsGroup = false,
					Name = "100K+",
					Color = Web.Models.TfColor.Blue500,
					Filters = new List<TfFilterBase>(){
						new TfFilterNumeric($"{dataProviderPrefix}population",TfFilterNumericComparisonMethod.Lower,"1000000"),
						new TfFilterNumeric($"{dataProviderPrefix}population",TfFilterNumericComparisonMethod.GreaterOrEqual,"100000")
					},
					SortOrders = new List<TfSort>{
						new TfSort($"{dataProviderPrefix}population",TfSortDirection.DESC)
					}
				},
				new TfSpaceViewPreset{
					Id = new Guid("a7ca2eb7-e800-4880-a2a7-e177a2d10850"),
					IsGroup = false,
					Name = "<100K",
					Color = Web.Models.TfColor.Cyan500,
					Filters = new List<TfFilterBase>(){
						new TfFilterNumeric($"{dataProviderPrefix}population",TfFilterNumericComparisonMethod.Lower,"100000"),
					},
					SortOrders = new List<TfSort>{
						new TfSort($"{dataProviderPrefix}population",TfSortDirection.DESC)
					}
				},
			},
				Columns = new(){
				new TfSpaceViewColumn{
					Id = new Guid("7b9bed20-529c-4b53-b886-5d92cf317304"),
					Position = 1,
					QueryName = "city",
					Title = "city",
					DataMapping = new Dictionary<string, string> { {"Value",$"{dataProviderPrefix}city"}},
					SpaceViewId = spaceViewId,
					TypeId = new Guid(TfTextViewColumnType.ID),
					ComponentId = new Guid(TfTextDisplayColumnComponent.ID),
				},
				new TfSpaceViewColumn{
					Id = new Guid("80aa74a1-2453-4001-9af6-b4a537695d70"),
					Position = 2,
					QueryName = "country",
					Title = "country",
					DataMapping = new Dictionary<string, string> { {"Value",$"{dataProviderPrefix}country"}},
					SpaceViewId = spaceViewId,
					TypeId = new Guid(TfTextViewColumnType.ID),
					ComponentId = new Guid(TfTextDisplayColumnComponent.ID),
				},
				new TfSpaceViewColumn{
					Id = new Guid("c3f77a8c-9336-4bee-8412-7e193ee64882"),
					Position = 3,
					QueryName = "population",
					Title = "population",
					DataMapping = new Dictionary<string, string> { {"Value",$"{dataProviderPrefix}population"}},
					SpaceViewId = spaceViewId,
					TypeId = new Guid(TfLongIntegerViewColumnType.ID),
					ComponentId = new Guid(TfLongIntegerDisplayColumnComponent.ID),
				},
				new TfSpaceViewColumn{
					Id = new Guid("6a0eee60-6d6e-4fd8-9fed-e2462f1922ea"),
					Position = 4,
					QueryName = "id",
					Title = "id",
					DataMapping = new Dictionary<string, string> { {"Value",$"{dataProviderPrefix}id"}},
					SpaceViewId = spaceViewId,
					TypeId = new Guid(TfLongIntegerViewColumnType.ID),
					ComponentId = new Guid(TfLongIntegerDisplayColumnComponent.ID),
				}
			}
			}
		});

		Steps.Add(new TfCreateSpacePageRecipeStep
		{
			Instance = new TfRecipeStepInstance
			{
				Visible = false,
				StepId = new Guid("12cb625a-5a9a-43a1-b3ba-683576278578"),
				StepMenuTitle = "Create space page",
			},
			Data = new TfCreateSpacePageRecipeStepData
			{
				SpacePageId = spacePageId,
				SpaceId = spaceId,
				Name = "World Cities",
				FluentIconName = "Globe",
				Position = 100,
				Type = TfSpacePageType.Page,
				ChildPages = new(),
				ComponentType = new TfSpaceViewSpacePageAddon(),
				ComponentId = new Guid(TfSpaceViewSpacePageAddon.ID),
				ComponentOptionsJson = JsonSerializer.Serialize(new TfSpaceViewSpacePageAddonOptions
				{
					SetType = Web.Models.TucSpaceViewSetType.Existing,
					SpaceViewId = spaceViewId,
					Name = "World Cities",
					Type = Web.Models.TucSpaceViewType.DataGrid,
					DataProviderId = dataProviderId,
					DataSetType = Web.Models.TucSpaceViewDataSetType.Existing,
					SpaceDataId = spaceDataId,
				})
			}
		});

		Steps.Add(new TfCreateBlobRecipeStep
		{
			Instance = new TfRecipeStepInstance
			{
				Visible = false,
				StepId = new Guid("fb72375f-f891-4220-b821-e9500756ed6f"),
				StepMenuTitle = "Create blob",
			},
			Data = new TfCreateBlobRecipeStepData
			{
				BlobId = template1BlobId,
				EmbeddedResourceName = $"{nameSpace}.Files.{template1FileName}",
				Assembly = this.GetType().Assembly,
				IsTemporary = false,
			}
		});

		Steps.Add(new TfCreateExcelFileTemplateRecipeStep
		{
			Instance = new TfRecipeStepInstance
			{
				Visible = false,
				StepId = new Guid("d519db63-b138-46b3-b044-8c8ecd8cc806"),
				StepMenuTitle = "Create template",
				StepContentTitle = "",
				StepContentDescription = "",
			},
			Data = new TfCreateExcelFileTemplateRecipeStepData
			{
				TemplateId = template1Id,
				Name = "Export Cities by Template",
				FluentIconName = "DocumentTable",
				Description = "generates the selected cities in an excel template",
				ContentProcessorType = new ExcelFileTemplateProcessor(),
				IsEnabled = true,
				IsSelectable = true,
				SpaceDataList = new() { spaceDataId },
				EmbeddedResourceName = $"{nameSpace}.Files.{csvFileName}",
				Assembly = this.GetType().Assembly,
				SettingsJson = JsonSerializer.Serialize(new ExcelFileTemplateSettings()
				{
					FileName = template1FileName,
					GroupBy = new(),
					TemplateFileBlobId = template1BlobId,
				})
			}
		});
	}

}
