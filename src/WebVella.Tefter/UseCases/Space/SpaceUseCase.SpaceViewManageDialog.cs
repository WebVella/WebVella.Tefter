namespace WebVella.Tefter.UseCases.Space;
public partial class SpaceUseCase
{
	internal TucSpaceView SpaceViewManageForm { get; set; }
	internal List<TucSpaceData> SpaceDataList { get; set; }
	internal string SpaceName { get; set; }
	internal int GeneratedColumnCountLimit { get; set; } = 20;
	internal Task InitSpaceViewManageDialog(Guid spaceId)
	{
		var space = GetSpace(spaceId);
		if(space is null){ 
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("Space not found")),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return Task.CompletedTask;	
		}
		SpaceName = space.Name;
		AllDataProviders = GetDataProviderList();
		SpaceDataList = GetSpaceDataList(spaceId);
		SpaceViewManageForm = new();
		return Task.CompletedTask;
	}

}