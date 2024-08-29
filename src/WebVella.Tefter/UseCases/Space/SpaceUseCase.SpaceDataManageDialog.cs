namespace WebVella.Tefter.UseCases.Space;
public partial class SpaceUseCase
{
	internal TucSpaceData SpaceDataManageForm { get; set; }
	internal Task InitSpaceDataManageDialog(Guid spaceId)
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
		SpaceDataManageForm = new();
		return Task.CompletedTask;
	}

}