using Microsoft.FluentUI.AspNetCore.Components.DesignTokens;
using Nito.AsyncEx.Synchronous;
using WebVella.Tefter.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	Task SpacePageCreateFromFileAsync(
		TfSpacePageCreateFromFileContextItem item);
}

public partial class TfService
{
	public async Task SpacePageCreateFromFileAsync(
		TfSpacePageCreateFromFileContextItem item)
	{
		var dataProviderTypes = _metaService.GetDataProviderTypes();
		item.ProcessStream.ReportProgress(new TfProgressStreamItem
		{
			Message = "Checking for suitable Data Processor type...",
			Type = TfProgressStreamItemType.Normal
		});
		//Check providers for data provider creation request
		foreach (var providerType in dataProviderTypes)
		{
			item.ProcessStream.ReportProgress(new TfProgressStreamItem
			{
				Message = $"Checking Data Provider type \"{providerType.AddonName}\"",
				Type = TfProgressStreamItemType.Debug
			});
			try
			{
				await providerType.GenerateDataProviderCreationRequest(item, this);
			}
			catch (Exception ex)
			{
				item.ProcessStream.ReportProgress(new TfProgressStreamItem
				{
					Message =
						$"Data Provider type \"{providerType.AddonName}\" request generation failed with error: {ex.Message}.",
					Type = TfProgressStreamItemType.Debug
				});
			}

			if (item.ProcessContext.DataProviderCreationRequest is null)
			{
				item.ProcessStream.ReportProgress(new TfProgressStreamItem
				{
					Message = $"Data Provider type \"{providerType.AddonName}\" did not request creation.",
					Type = TfProgressStreamItemType.Debug
				});
				continue;
			}

			item.ProcessContext.UsedDataProviderAddon = providerType;
			item.ProcessStream.ReportProgress(new TfProgressStreamItem
			{
				Message = $"Data Provider type \"{providerType.AddonName}\" selected to process the file.",
				Type = TfProgressStreamItemType.Debug
			});

			break;
		}

		//Exit if no data provider creation request
		if (item.ProcessContext.DataProviderCreationRequest is null)
		{
			item.IsProcessed = true;
			item.ProcessStream.ReportProgress(new TfProgressStreamItem
			{
				Message = "No suitable Data Provider that can process the file was found!",
				Type = TfProgressStreamItemType.Warning
			});
			return;
		}

		using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
		{
			try
			{
				#region << Provider, Dateset >>
				var providerName = item.ProcessContext.DataProviderCreationRequest.Name;
				if (!String.IsNullOrWhiteSpace(providerName))
				{
					providerName = providerName.ConvertFileNameToDataProviderName();
					var dataProviders = GetDataProviders();
					for (int i = 0; i < Int32.MaxValue; i++)
					{
						var checkedName = providerName;
						if (i > 0)
							checkedName = $"{checkedName}-{i}";

						if (!dataProviders.Any(x => x.Name == checkedName))
						{
							providerName = checkedName;
							break;
						}
					}
				}
				else
				{
					providerName = $"Data provider {Guid.NewGuid()}".ConvertFileNameToDataProviderName();
				}

				item.ProcessContext.DataProvider = CreateDataProvider(new TfCreateDataProvider()
				{
					Id = Guid.NewGuid(),
					Name = providerName,
					Index = -1,
					ProviderType = item.ProcessContext.UsedDataProviderAddon!,
					SettingsJson = item.ProcessContext.DataProviderCreationRequest.SettingsJson ?? "{}",
					SynchPrimaryKeyColumns =
						item.ProcessContext.DataProviderCreationRequest.SynchPrimaryKeyColumns ?? new(),
					SynchScheduleMinutes = item.ProcessContext.DataProviderCreationRequest.SynchScheduleMinutes,
					SynchScheduleEnabled = false,
					AutoInitialize = true,
				});
				var providerDataSets = GetDatasets(providerId: item.ProcessContext.DataProvider.Id);
				if (providerDataSets.Count > 0)
					item.ProcessContext.Dataset = providerDataSets[0];
				#endregion

				#region << Space page and view >>
				item.ProcessContext.Space = item.Space;

				var (pageId, _) = CreateSpacePage(new TfSpacePage()
				{
					Id = Guid.NewGuid(),
					Name = item.ProcessContext.DataProvider.Name,
					SpaceId = item.ProcessContext.Space!.Id,
					ComponentId = new TucSpaceViewSpacePageAddon().AddonId,
					ComponentOptionsJson = JsonSerializer.Serialize(new TfSpaceViewSpacePageAddonOptions()
					{
						DatasetId = providerDataSets[0].Id
					}),
					Type = TfSpacePageType.Page,
					FluentIconName = "Table"
				});
				item.ProcessContext.SpacePage = GetSpacePage(pageId);
				if (item.ProcessContext.SpacePage is null)
				{
					item.ProcessStream.ReportProgress(new TfProgressStreamItem
					{
						Message = $"Page was not created, due to unknown error!",
						Type = TfProgressStreamItemType.Error
					});
					return;
				}
				var options = JsonSerializer.Deserialize<TfSpaceViewSpacePageAddonOptions>(item.ProcessContext.SpacePage.ComponentOptionsJson);
				if (options is null || options.SpaceViewId is null)
				{
					item.ProcessStream.ReportProgress(new TfProgressStreamItem
					{
						Message = $"View was not created, due to unknown error!",
						Type = TfProgressStreamItemType.Error
					});
					return;
				}

				item.ProcessContext.SpaceView = GetSpaceView(options.SpaceViewId.Value);
				if (item.ProcessContext.SpaceView is null)
				{
					item.ProcessStream.ReportProgress(new TfProgressStreamItem
					{
						Message = $"View was not created, due to unknown error!",
						Type = TfProgressStreamItemType.Error
					});
					return;
				}
				#endregion


				#region << Prepend Asset column >>
				if (item.CreateAssetsColumn)
				{
					await CreateSpaceViewColumn(new TfSpaceViewColumn
					{
						Id = Guid.NewGuid(),
						SpaceViewId = item.ProcessContext.SpaceView!.Id,
						QueryName = NavigatorExt.GenerateQueryName(),
						Title = "assets",
						Icon = null,
						OnlyIcon = false,
						Position = 1,
						TypeId = new Guid("aafd5f8a-95d0-4f6b-8b43-c75a80316504"),//TfFolderAssetsCountViewColumnType
						TypeOptionsJson = "{\"FolderId\":\"5d229b2b-5c78-48fb-b91f-6e853f24aaf2\"}", //asset appId is the default folder id
						Settings = new() { Width = 40 },
						DataMapping = new Dictionary<string, string?>() { { "Value", "default.sc_default_assets_counter" } }

					});
				}
				#endregion

				#region << Prepend Talk column >>
				if (item.CreateTalkColumn)
				{
					await CreateSpaceViewColumn(new TfSpaceViewColumn
					{
						Id = Guid.NewGuid(),
						SpaceViewId = item.ProcessContext.SpaceView!.Id,
						QueryName = NavigatorExt.GenerateQueryName(),
						Title = "talk",
						Icon = null,
						OnlyIcon = false,
						Position = 1,
						TypeId = new Guid("60ab4197-be46-4ebd-a6de-02e8d25450d3"),//TfTalkCommentsCountViewColumnType
						TypeOptionsJson = "{\"ChannelId\":\"27a7703a-8fe8-4363-aee1-64a219d7520e\"}",//talk appId is the default folder id
						Settings = new() { Width = 40},
						DataMapping = new Dictionary<string, string?>() { { "Value", "default.sc_default_talk_counter" } }

					});
				}
				#endregion

				#region << ReInit Dataset columns to include addons >>
				if (item.ProcessContext.Dataset is not null
					&& (item.CreateTalkColumn || item.CreateAssetsColumn)
					)
				{
					var datasetColumnOptions = GetDatasetColumnOptions(item.ProcessContext.Dataset.Id);
					foreach (var column in datasetColumnOptions)
					{
						if (column.SourceType == TfAuxDataSourceType.AuxDataProvider)
							continue;
						AddDatasetColumn(item.ProcessContext.Dataset.Id, column);
					}
				}
				#endregion

				scope.Complete();

				item.IsProcessed = true;
				item.IsSuccess = true;
				item.ProcessStream.ReportProgress(new TfProgressStreamItem
				{
					Message = $"Page successfully created!",
					Type = TfProgressStreamItemType.Success
				});
			}
			catch (Exception ex)
			{
				item.IsProcessed = true;
				item.IsSuccess = false;
				item.ProcessStream.ReportProgress(new TfProgressStreamItem
				{
					Message = ex.InnerException is not null && !String.IsNullOrWhiteSpace(ex.Message)
						? $"Space page creation failed with error: {ex.InnerException.Message}!"
						: $"Space page creation failed with error: {ex.Message}!",
					Type = TfProgressStreamItemType.Error
				});
			}
		}

		if (!item.IsSuccess)
		{
			//Delete created repository files
			foreach (var fileName in item.ProcessContext.CreatedRepositoryFiles)
			{
				try
				{
					DeleteRepositoryFile(fileName);
				}
				catch (Exception) { }
			}
		}
	}
}