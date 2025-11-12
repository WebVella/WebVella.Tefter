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
			Message = "Checking for suitable Data Processor type...", Type = TfProgressStreamItemType.Normal
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
				//Create Provider, Dateset
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
				//Create space page
				var (pageId, pages) = CreateSpacePage(new TfSpacePage()
				{
					Id = Guid.NewGuid(),
					Name = item.ProcessContext.DataProvider.Name,
					SpaceId = item.Space.Id,
					ComponentId = new TucSpaceViewSpacePageAddon().AddonId,
					ComponentOptionsJson = JsonSerializer.Serialize(new TfSpaceViewSpacePageAddonOptions()
					{
						DatasetId = providerDataSets[0].Id
					}),
					Type = TfSpacePageType.Page,
					FluentIconName = "Table"
				});
				item.ProcessContext.SpacePage = pages.FirstOrDefault(x => x.Id == pageId);

				scope.Complete();

				item.IsProcessed = true;
				item.IsSuccess = true;
				item.ProcessStream.ReportProgress(new TfProgressStreamItem
				{
					Message = $"Page successfully created!", Type = TfProgressStreamItemType.Success
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